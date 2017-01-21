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
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class BrandsController : Controller
    {
        private readonly IGenericService<Brand> _brandsService;
            
        public BrandsController(IGenericService<Brand> brandsService)
        {
            _brandsService = brandsService;
        }

        [HttpGet("/api/Brands/Get", Name = "GetBrands")]
        public async Task<IActionResult> Get()  
        {
            var brands = await _brandsService.GetAll(null, "Image");
            return Json(brands);
        }

        [HttpGet("/api/Brands/Get/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var brand = await _brandsService.GetById<Brand>(m => m.Id == id, "Image");
            return Json(brand);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Brand brand)
        {
            if (brand == null)
                return BadRequest();

            var result = _brandsService.Add(brand, m => m.Name == brand.Name);

            if (result.Status == TaskStatus.Created)
            {
                return CreatedAtRoute("GetBrands", new { id = brand.Id }, brand);
            }
            return BadRequest("Item not added");
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Brand brand)
        {
            if (brand == null || brand.Id != id)
            {
                return BadRequest();
            }

            _brandsService.Update(brand);
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (id <=0 )
            {
                return BadRequest();
            }

            var brand = _brandsService.GetById<Brand>(m => m.Id == id);
            _brandsService.Delete(brand.Result);
            return new NoContentResult();
        }
    }
}
