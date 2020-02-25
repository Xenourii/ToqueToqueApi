using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToqueToqueApi.Databases;
using ToqueToqueApi.Models;

namespace ToqueToqueApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class DifficultiesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ToqueToqueContext _dbContext;

        public DifficultiesController(IMapper mapper, ToqueToqueContext dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        /// <summary>
        /// Récupère toutes les difficultés
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll()
        {
            var difficultiesDb = _dbContext.Difficulties;
            var difficulties = _mapper.Map<IList<Difficulty>>(difficultiesDb);
            return Ok(difficulties);
        }
    }
}