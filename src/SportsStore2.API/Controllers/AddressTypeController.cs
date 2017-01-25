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
    [Route("api/AddressType")]
    public class AddressTypeController : Controller
    {
        private readonly IGenericService<AddressType> _addressTypeService;

        public AddressTypeController(IGenericService<AddressType> addressTypeService)
        {
            _addressTypeService = addressTypeService;
        }

        [HttpGet("/api/AddressType/Get", Name = "GetAddressType")]
        public async Task<IActionResult> Get()
        {
            var addressType = await _addressTypeService.GetAll();
            return Json(addressType);
        }

        [HttpGet("/api/AddressType/Get/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var addressType = await _addressTypeService.GetById<AddressType>(m => m.Id == id);
            return Json(addressType);
        }

        [HttpPost]
        public IActionResult Create([FromBody] AddressType addressType)
        {
            if (addressType == null)
                return BadRequest();

            var result = _addressTypeService.Add(addressType, m => m.Name == addressType.Name);
            if (result)
            {
                return CreatedAtRoute("GetAddressType", new { id = addressType.Id }, addressType);

            }
            return BadRequest("Item not added");
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] AddressType addressType)
        {
            if (addressType == null || addressType.Id != id)
            {
                return BadRequest();
            }

            _addressTypeService.Update(addressType);
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var addressType = _addressTypeService.GetById<AddressType>(m => m.Id == id);
            _addressTypeService.Delete(addressType.Result);
            return new NoContentResult();
        }

    }
}