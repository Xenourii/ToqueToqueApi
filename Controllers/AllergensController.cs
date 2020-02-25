using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToqueToqueApi.Databases;
using ToqueToqueApi.Databases.Models;

namespace ToqueToqueApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AllergensController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ToqueToqueContext _dbContext;

        public AllergensController(IMapper mapper, ToqueToqueContext dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        /// <summary>
        /// Récupère tous les allergènes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll()
        {
            var allergensDb = _dbContext.Allergens;
            var allergens = _mapper.Map<IList<AllergenDb>>(allergensDb);
            return Ok(allergens);
        }
    }
}