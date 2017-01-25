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
    [Route("api/Categories")]
    public class CategoriesController : Controller  
    {
        private readonly IGenericService<Category> _categoriesService;

        public CategoriesController(IGenericService<Category> categoriesService)
        {
            _categoriesService = categoriesService;
        }

        [HttpGet("/api/Categories/Get", Name = "GetCategories")]
        public async Task<IActionResult> Get()
        {
            var Categories = await _categoriesService.GetAll();
            return Json(Categories);
        }

        [HttpGet("/api/Categories/Get/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var category = await _categoriesService.GetById<Category>(m => m.Id == id);
            return Json(category);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Category category)
        {
            if (category == null)
                return BadRequest();

            var result = _categoriesService.Add(category, m => m.Name == category.Name);
            if (result)
            {
                return CreatedAtRoute("GetCategories", new { id = category.Id }, category);

            }
            return BadRequest("Item not added");
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Category category)
        {
            if (category == null || category.Id != id)
            {
                return BadRequest();
            }

            _categoriesService.Update(category);
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var category = _categoriesService.GetById<Category>(m => m.Id == id);
            _categoriesService.Delete(category.Result);
            return new NoContentResult();
        }

    }
}