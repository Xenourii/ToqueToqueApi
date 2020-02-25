using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToqueToqueApi.Services;

namespace ToqueToqueApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ProfilesController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public ProfilesController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var users = _userService.GetAll();

            if (users?.Any() != true)
                return NotFound();

            var model = _mapper.Map<IList<Models.Profile>>(users);
            return Ok(model);
        }

        [HttpGet("{userId}")]
        public IActionResult GetById(int userId)
        {
            var userDb = _userService.Get(userId);

            if (userDb == null)
                return NotFound();

            var model = _mapper.Map<Models.Profile>(userDb);
            return Ok(model);
        }
    }
}