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
    [Route("api/Images")]
    public class ImagesController : Controller
    {
        private readonly IGenericService<Image> _imagesService;

        public ImagesController(IGenericService<Image> imagesService)
        {
            _imagesService = imagesService;
        }

        [HttpGet("/api/Images/Get", Name = "GetImages")]
        public async Task<IActionResult> Get()
        {
            var images = await _imagesService.GetAll();
            return Json(images);
        }

        [HttpGet("/api/Images/Get/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var image = await _imagesService.GetById<Image>(m => m.Id == id);
            return Json(image);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Image image)
        {
            if (image == null)
                return BadRequest();

            var result = _imagesService.Add(image, m => m.Name == image.Name);
            if (result)
            {
                return CreatedAtRoute("GetImages", new { id = image.Id }, image);

            }
            return BadRequest("Item not added");
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Image image)
        {
            if (image == null || image.Id != id)
            {
                return BadRequest();
            }

            _imagesService.Update(image);
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var image = _imagesService.GetById<Image>(m => m.Id == id);
            _imagesService.Delete(image.Result);
            return new NoContentResult();
        }

    }
}