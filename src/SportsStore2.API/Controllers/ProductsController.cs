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
        private readonly IGenericService<Product> _productsService;

        public ProductsController(IGenericService<Product> productsService)
        {
            _productsService = productsService;
        }

        [HttpGet("/api/products/Get", Name = "Getproducts")]
        public async Task<IActionResult> Get()
        {
            var products = await _productsService.GetAll(null, "Image,Brand,Category");
            return Json(products);
        }

        [HttpGet("/api/products/Get/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var product = await _productsService.GetById<Product>(m => m.Id == id, "Image,Brand,Category");
            return Json(product);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Product product)
        {
            if (product == null)
                return BadRequest();

            if (_productsService.Add(product, m => m.Name == product.Name).Result)
            {
                return CreatedAtRoute("Getproducts", new { id = product.Id }, product);
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
            var product = _productsService.GetById<Product>(m => m.Id == id);
            _productsService.Delete(product.Result);
            return new NoContentResult();
        }
    }
}