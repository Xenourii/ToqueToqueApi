using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ToqueToqueApi.Databases.Models;
using ToqueToqueApi.Exceptions;
using ToqueToqueApi.Helpers;
using ToqueToqueApi.Models.Meals;
using ToqueToqueApi.Services;

namespace ToqueToqueApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class MealsController : ControllerBase
    {
        private readonly IMealService _mealService;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;

        public MealsController(
            IMealService mealService,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _mealService = mealService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        /// <summary>
        /// Récupère tous les plats
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAll()
        {
            var meals = _mealService.GetAll();
            var model = _mapper.Map<IList<Meal>>(meals);
            return Ok(model);
        }

        /// <summary>
        /// Récupère tous les plats d'un utilisateur authentifié
        /// </summary>
        /// <returns></returns>
        [HttpGet("me")]
        public IActionResult GetAllUser()
        {
            var userId = GetUserIdFromAuthentication();
            var meals = _mealService.GetAllMealsFromUser(userId);
            var model = _mapper.Map<IList<Meal>>(meals);
            return Ok(model);
        }

        /// <summary>
        /// Récupère un plat selon son Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var meal = _mealService.GetById(id);

            if (meal == null)
                return NotFound();

            var model = _mapper.Map<Meal>(meal);
            return Ok(model);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Meal mealToCreate)
        {
            mealToCreate.OwnerId = GetUserIdFromAuthentication();
            var meal = _mapper.Map<MealDb>(mealToCreate);

            try
            {
                var createdDbMeal = _mealService.Create(meal);
                var createdMeal = _mapper.Map<Meal>(createdDbMeal);
                return CreatedAtAction(nameof(Create), createdMeal);
            }
            catch (MealTitleDuplicateForUserException ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }

        /// <summary>
        /// Upload une image pour un plat
        /// </summary>
        /// <returns></returns>
        [HttpPost("{id}/pictures")]
        [AllowAnonymous]
        public IActionResult Upload(int id, [FromForm] IFormCollection files)
        {
            try
            {
                var folderName = Path.Combine("Public", "Pictures");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                var dbPaths = new List<string>();

                foreach (var file in files.Files)
                {
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
                    dbPaths.Add(dbPath);
                }

                var mealDb = _mealService.GetById(id);
                foreach (var path in dbPaths)
                    mealDb.Pictures.Add(path);

                _mealService.Update(mealDb);
                return Ok(new {Pictures = dbPaths});
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateMeal updateMeal)
        {
            var meal = _mapper.Map<MealDb>(updateMeal);
            meal.Id = id;

            try
            {
                _mealService.Update(meal);
                return Ok();
            }
            catch (MealIdNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (MealTitleDuplicateForUserException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPatch("{id}")]
        public IActionResult Patch(int id, [FromBody] JsonPatchDocument<UpdateMeal> mealPatch)
        {
            if (mealPatch is null)
                return BadRequest();

            var updateMeal = new UpdateMeal();
            mealPatch.ApplyTo(updateMeal);

            var meal = _mapper.Map<MealDb>(updateMeal);
            meal.Id = id;

            try
            {
                _mealService.Update(meal);
                return Ok();
            }
            catch (MealIdNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (MealTitleDuplicateForUserException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _mealService.Delete(id);
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