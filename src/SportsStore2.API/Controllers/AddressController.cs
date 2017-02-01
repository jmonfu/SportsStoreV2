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
    [Route("api/Address")]
    public class AddressController : Controller
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet("/api/Address/Get", Name = "GetAddress")]
        public async Task<IActionResult> Get()
        {
            var address = await _addressService.GetAll();
            return Json(address);
        }

        [HttpGet("/api/Address/Get/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var address = await _addressService.GetById<Address>(m => m.Id == id);
            return Json(address);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Address address)
        {
            if (address == null)
                return BadRequest();

            var result = _addressService.Add(address, 
                m => m.Address1 == address.Address1 
                && m.Address2 == address.Address2
                && m.Address3 == address.Address3
                && m.AddressTypeId == address.AddressTypeId
                && m.CountryId == address.CountryId
                && m.City == address.City
                );

            if (result)
            {
                return CreatedAtRoute("GetAddress", new { id = address.Id }, address);

            }
            return BadRequest("Item not added");
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Address address)
        {
            if (address == null || address.Id != id)
            {
                return BadRequest();
            }

            _addressService.Update(address);
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var address = _addressService.GetById<Address>(m => m.Id == id);
            _addressService.Delete(address.Result);
            return new NoContentResult();
        }
    }
}