using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportsStore2.API.Models;
using SportsStore2.API.Services;

namespace SportsStore2.API.Controllers
{
    [Route("api/[controller]")]
    public class BrandsController : Controller
    {
        private readonly IBrandsService _brandsService;

        public BrandsController(IBrandsService brandsService)
        {
            _brandsService = brandsService;
        }

        [HttpGet("/api/Brands/Get", Name = "GetBrands")]
        public async Task<IActionResult> Get()
        {
            var brands = await _brandsService.GetBrands();
            return Json(brands);
        }

        [HttpGet("/api/Brands/Get/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var brand = await _brandsService.GetBrandById(id);
            return Json(brand);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Brand brand)
        {
            if(brand == null)
                return BadRequest();

            _brandsService.AddBrand(brand);
            return CreatedAtRoute("GetBrands", new { id = brand.Id }, brand);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Brand brand)
        {
            if(brand == null || brand.Id != id)
                return BadRequest();

            var result = _brandsService.UpdateBrand(brand);
            return result;
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            return _brandsService.DeleteBrand(id);
        }
    }
}