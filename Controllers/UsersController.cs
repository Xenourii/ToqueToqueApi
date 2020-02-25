using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ToqueToqueApi.Databases.Models;
using ToqueToqueApi.Exceptions;
using ToqueToqueApi.Extensions;
using ToqueToqueApi.Helpers;
using ToqueToqueApi.Models.Users;
using ToqueToqueApi.Services;

namespace ToqueToqueApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;

        public UsersController(
            IUserService userService,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] AuthenticateUser authenticateUser)
        {
            var user = _userService.Authenticate(authenticateUser.Email, authenticateUser.Password);

            if (user == null)
                return Unauthorized(new {message = "Email or password is incorrect"});

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var claimIdentity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(nameof(user.Email).ToCamelCase(), user.Email),
                new Claim(nameof(user.FirstName).ToCamelCase(), user.FirstName),
                new Claim(nameof(user.LastName).ToCamelCase(), user.LastName)
            });

            if (user.ProfilePicture != null)
                claimIdentity.AddClaim(new Claim(nameof(user.ProfilePicture).ToCamelCase(), user.ProfilePicture));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimIdentity,
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return CreatedAtAction(nameof(Authenticate), new
            {
                Id = user.Id,
                Email = user.Email,
                Token = tokenString
            });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterUser registerUser)
        {
            var userToCreate = _mapper.Map<UserDb>(registerUser);

            try
            {
                var userDb = _userService.Create(userToCreate, registerUser.Password);
                var user = _mapper.Map<User>(userDb);
                return CreatedAtAction(nameof(Register), user);
            }
            catch (PasswordRequiredException ex)
            {
                return BadRequest(new {message = ex.Message});
            }
            catch (EmailAlreadyTakenException ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }

        //[HttpGet]
        //public IActionResult GetAll()
        //{
        //    var users = _userService.GetAll();
        //    var model = _mapper.Map<IList<User>>(users);
        //    return Ok(model);
        //}

        [HttpGet("me")]
        public IActionResult GetMyUser()
        {
            var userId = GetUserIdFromAuthentication();
            var userDb = _userService.Get(userId);
            var user = _mapper.Map<User>(userDb);
            return Ok(user);
        }

        //[HttpGet("{id}")]
        //public IActionResult GetById(int id)
        //{
        //    var user = _userService.Get(id);

        //    if (user == null)
        //        return NotFound();

        //    var model = _mapper.Map<User>(user);
        //    return Ok(model);
        //}

        [HttpPost("{id}/profilePicture")]
        [AllowAnonymous]
        public IActionResult Upload(int id, [FromForm] IFormCollection files)
        {
            try
            {
                var folderName = Path.Combine("Public", "Profiles");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                if (files.Files.Count == 0)
                    return BadRequest();

                var file = files.Files.First();
                if (file.Length <= 0)
                    return BadRequest();

                var extension = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"').Split('.')[1];
                var fileName = $"{Guid.NewGuid()}.{extension}";
                var fullPath = Path.Combine(pathToSave, fileName);
                var dbPath = Path.Combine(folderName, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                dbPath = dbPath.Replace(Path.DirectorySeparatorChar, '/');
                
                var userDb = _userService.Get(id);
                userDb.ProfilePicture = dbPath;

                _userService.Update(userDb);
                return Ok(new {ProfilePicturePath = dbPath});
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateUser updateUser)
        {
            var user = _mapper.Map<UserDb>(updateUser);
            user.Id = id;

            try
            {
                _userService.Update(user, updateUser.Password);
                return Ok();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new {message = ex.Message});
            }
            catch (EmailAlreadyTakenException ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }

        [HttpPatch("{id}")]
        public IActionResult Patch(int id, [FromBody] JsonPatchDocument<UpdateUser> userPatch)
        {
            if (userPatch is null)
                return BadRequest();

            // On récupère l'utilisateur en BDD
            var userDb = _userService.Get(id);

            // On map l'utilisateur BDD en updateUser
            var updateUser = _mapper.Map<UpdateUser>(userDb);

            // On effectue les modifications nécessaires
            userPatch.ApplyTo(updateUser);

            // On converti la modification en modèle de BDD
            var user = _mapper.Map<UserDb>(updateUser);

            try
            {
                _userService.Update(user, updateUser.Password);
                return Ok();
            }
            catch (NotFoundException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (EmailAlreadyTakenException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _userService.Delete(id);
            return Ok();
        }

        private int GetUserIdFromAuthentication()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null)
                throw new NotAuthorizedException("No identity token found");

            return JWTHelper.GetUserId(identity);
        }
    }
}