using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ToqueToqueApi.Databases.Models;
using ToqueToqueApi.Exceptions;
using ToqueToqueApi.Helpers;
using ToqueToqueApi.Models;
using ToqueToqueApi.Services;
using Profile = AutoMapper.Profile;

namespace ToqueToqueApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class SessionsController : ControllerBase
    {
        private readonly ISessionService _sessionService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly PagingOptions _defaultPagingOptions;

        public SessionsController(
            ISessionService sessionService,
            IMapper mapper,
            IOptions<PagingOptions> defaultPagingOptions, IUserService userService)
        {
            _sessionService = sessionService;
            _mapper = mapper;
            _userService = userService;
            _defaultPagingOptions = defaultPagingOptions.Value;
        }

        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] PagingOptions pagingOptions,
            [FromQuery] SortOptions<Session, SessionDb> sortOptions,
            [FromQuery] FilterOptions<Session, SessionDb> filterOptions,
            [FromQuery] DistanceOptions distanceOptions)
        {
            pagingOptions.Offset ??= _defaultPagingOptions.Offset;
            pagingOptions.Limit ??= _defaultPagingOptions.Limit;

            //var userId = GetUserIdFromAuthentication();
            //var sessions = await _sessionService.GetAll(userId, pagingOptions, sortOptions, filterOptions, distanceOptions);
            var sessions = await _sessionService.GetAll(pagingOptions, sortOptions, filterOptions);

            var collection = PagedCollection<Session>.Create(
                sessions.Items.ToArray(),
                sessions.TotalSize,
                pagingOptions
            );

            return Ok(collection);
        }

        /// <summary>
        /// Récupère une session selon son Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var userId = GetUserIdFromAuthentication();
            var session = _sessionService.GetById(id);

            if (session == null)
                return NotFound();

            var model = _mapper.Map<Session>(session);
            model.BookingState = _sessionService.GetBookingState(id, userId);
            return Ok(model);
        }


        [HttpPost]
        public IActionResult Create([FromBody] Session sessionToCreate)
        {
            var session = _mapper.Map<SessionDb>(sessionToCreate);
            session.Creator = GetUserIdFromAuthentication();

            try
            {
                var sessionDb = _sessionService.Create(session);
                var createdSession = _mapper.Map<Session>(sessionDb);
                return CreatedAtAction(nameof(Create), createdSession);
            }
            catch (SessionStartTimeDuplicateForUserException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _sessionService.Delete(id);
            return Ok();
        }


        [HttpPost("{id}/join")]
        public IActionResult Join(int id)
        {
            try
            {
                _sessionService.Join(id, HttpContext.User.Identity as ClaimsIdentity);
                return Ok();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (NotAuthorizedException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (JoinSessionException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPost("{id}/cancel")]
        public IActionResult Cancel(int id)
        {
            try
            {
                _sessionService.Cancel(id, HttpContext.User.Identity as ClaimsIdentity);
                return Ok();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (NotAuthorizedException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (JoinSessionException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPost("{sessionId}/accept")]
        public IActionResult AcceptUser(int sessionId, int userId)
        {
            try
            {
                _sessionService.AcceptUser(sessionId, userId, HttpContext.User.Identity as ClaimsIdentity);
                return Ok();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (NotAuthorizedException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (UserNotWaitingForSessionException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPost("{sessionId}/deny")]
        public IActionResult DenyUser(int sessionId, int userId)
        {
            try
            {
                _sessionService.DenyUser(sessionId, userId, HttpContext.User.Identity as ClaimsIdentity);
                return Ok();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (NotAuthorizedException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (UserNotWaitingForSessionException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}/participants")]
        public IActionResult GetParticipants(int id)
        {
            var userId = GetUserIdFromAuthentication();
            var session = _sessionService.GetById(id);
            if (session == null)
                return NotFound();

            var isTheSessionOwner = session.Creator == userId;
            var isUserAccepted = 0;

            var participants = new List<Participant>();
            foreach (var bookingStateSessionUserDb in session.BookingStateSessionUser)
            {
                var userDb = _userService.Get(bookingStateSessionUserDb.UserId);
                if (userDb == null)
                    continue;

                if (bookingStateSessionUserDb.UserId == userId)
                    isUserAccepted += 1;

                var participant = new Participant
                {
                    BookingState = bookingStateSessionUserDb.BookingStateId,
                    Profile = _mapper.Map<Models.Profile>(userDb)
                };
                participants.Add(participant);
            }

            if (isTheSessionOwner)
                return Ok(participants);

            if (isUserAccepted != 1)
                return Unauthorized();

            return Ok(participants.Where(p => p.BookingState == 2));
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