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
    [Route("api/Products")]
    public class ProductsController : Controller
    {
        private readonly IProductsService _productsService;
        //private readonly IGenericService<Product> _productsService;

        public ProductsController(IProductsService productsService)
        {
            _productsService = productsService;
        }

        [HttpGet("/api/Products/Get", Name = "GetProducts")]
        public async Task<IActionResult> Get()
        {
            var products = await _productsService.GetAll();
            return Json(products);
        }

        [HttpGet("/api/Products/Get/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var products = await _productsService.GetById<Product>(m => m.Id == id);
            return Json(products);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Product product)
        {
            if (product == null)
                return BadRequest();

            var result = _productsService.Add(product, m => m.Name == product.Name);
            if (result)
            {
                return CreatedAtRoute("GetProducts", new { id = product.Id }, product);

            }
            return BadRequest("Item not added");
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Product product)
        {
            if (product == null || product.Id != id)
            {
                return BadRequest();
            }

            _productsService.Update(product);
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var product = _productsService.GetById<Product>(m => m.Id == id);
            _productsService.Delete(product.Result);
            return new NoContentResult();
        }



    }
}