using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportsStore2.API.Models;
using SportsStore2.API.Services;

namespace SportsStore2.API.Controllers
{
    [Produces("application/json")]
    [Route("api/Countries")]
    public class CountriesController : Controller
    {
        private readonly IGenericService<Country> _countriesService;

        public CountriesController(IGenericService<Country> countriesService)
        {
            _countriesService = countriesService;
        }

        [HttpGet("/api/Countries/Get", Name = "GetCountries")]
        public async Task<IActionResult> Get()
        {
            var countries = await _countriesService.GetAll();
            return Json(countries);
        }

        [HttpGet("/api/Countries/Get/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var country = await _countriesService.GetById<Country>(m => m.Id == id);
            return Json(country);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Country country)
        {
            if (country == null)
                return BadRequest();

            var result = _countriesService.Add(country, m => m.Name == country.Name);
            if (result)
            {
                return CreatedAtRoute("GetCountries", new { id = country.Id }, country);

            }
            return BadRequest("Item not added");
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Country country)
        {
            if (country == null || country.Id != id)
            {
                return BadRequest();
            }

            _countriesService.Update(country);
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var country = _countriesService.GetById<Country>(m => m.Id == id);
            _countriesService.Delete(country.Result);
            return new NoContentResult();
        }



    }
}