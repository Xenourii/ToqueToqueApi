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
    public class ParticularitiesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ToqueToqueContext _dbContext;

        public ParticularitiesController(IMapper mapper, ToqueToqueContext dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        /// <summary>
        /// Récupère toutes les particularités
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll()
        {
            var particularitiesDb = _dbContext.Particularities;
            var particularities = _mapper.Map<IList<Particularity>>(particularitiesDb);
            return Ok(particularities);
        }
    }
}