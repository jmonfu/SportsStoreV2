using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportsStore2.API.Models;
using SportsStore2.API.Models.AccountViewModels;
using SportsStore2.API.Services;

namespace SportsStore2.API.Controllers
{
    [Produces("application/json")]
    [Route("api/Users")]
    public class UsersController : Controller
    {
        private readonly IUsersService _usersService;
        private readonly IAspNetUsersService _aspNetUsersService;

        public UsersController(IUsersService usersService, IAspNetUsersService aspNetUsersService)
        {
            _usersService = usersService;
            _aspNetUsersService = aspNetUsersService;
        }

        [HttpGet("/api/Users/Get", Name = "GetUsers")]
        public async Task<IActionResult> Get()
        {
            var users = await _usersService.GetAll();
            return Json(users);
        }

        [HttpGet("/api/Users/Get/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = await _usersService.GetById<User>(m => m.Id == id);
            return Json(user);
        }

        [HttpPost("Create")]
        public IActionResult Create([FromBody] User user)
        {
            if (user == null)
                return BadRequest();

            var result = _usersService.Add(user, m => m.Name == user.Name);
            if (result)
            {
                return CreatedAtRoute("GetUsers", new { id = user.Id }, user);

            }
            return BadRequest("Item not added");
        }

        [HttpPost("CreateAspNetUsers")]
        public IActionResult CreateAspNetUsers([FromBody] RegisterViewModel registerViewModel)
        {
            if (registerViewModel == null)
                return BadRequest();

            var result = _aspNetUsersService.Add(registerViewModel, m => m.Name == registerViewModel.Email);
            if (result)
            {
                return CreatedAtRoute("GetUsers", registerViewModel);
            }
            return BadRequest("Item not added");
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] User user)
        {
            if (user == null || user.Id != id)
            {
                return BadRequest();
            }

            _usersService.Update(user);
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var user = _usersService.GetById<User>(m => m.Id == id);
            _usersService.Delete(user.Result);
            _aspNetUsersService.Delete(user.Result.Email);
            return new NoContentResult();
        }

    }
}