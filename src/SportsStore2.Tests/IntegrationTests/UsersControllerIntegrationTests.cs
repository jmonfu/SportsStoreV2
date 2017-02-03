using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SportsStore2.API;
using SportsStore2.API.Models;
using SportsStore2.API.Models.AccountViewModels;

namespace SportsStore2.Tests.IntegrationTests
{
    [TestFixture]
    public class UsersControllerIntegrationTests
    {
        private HttpClient _client;
        private User _testUser;
        private User _testUserUpdated;
        private string _request;

        [SetUp]
        public void Setup()
        {
            var basePath = PlatformServices.Default.Application.ApplicationBasePath;
            var projectPath = Path.GetFullPath(Path.Combine(basePath, "../../../../SportsStore2.Tests"));

            var server = new TestServer(Utils.GetHostBuilder(new string[] { })
                .UseContentRoot(projectPath)
                .UseEnvironment("Development")
                .UseStartup<Startup>());

            _client = server.CreateClient();

            _testUser = new User {
                Name = Enums.GetEnumDescription(Enums.UserTestData.Name),
                Surname = Enums.GetEnumDescription(Enums.UserTestData.Surname),
                Email = Enums.GetEnumDescription(Enums.UserTestData.Email),
                HomeNo = Enums.GetEnumDescription(Enums.UserTestData.HomeNo),
                MobNo = Enums.GetEnumDescription(Enums.UserTestData.MobNo)
            };

            _testUserUpdated = new User
            {
                Name = Enums.GetEnumDescription(Enums.UserUpatedTestData.Name),
                Surname = Enums.GetEnumDescription(Enums.UserUpatedTestData.Surname),
                Email = Enums.GetEnumDescription(Enums.UserUpatedTestData.Email),
                HomeNo = Enums.GetEnumDescription(Enums.UserUpatedTestData.HomeNo),
                MobNo = Enums.GetEnumDescription(Enums.UserUpatedTestData.MobNo)
            };

            _request = Enums.GetEnumDescription(Enums.Requests.User);
        }

        [Test]
        public async Task Get_ReturnsAListOfUsers_UsersController()
        {
            _request = _request + "Get";
            var response = await _client.GetAsync(_request);
            response.EnsureSuccessStatusCode();

            Assert.IsTrue(true);
        }

        [Test]
        public async Task GetById_GetOneUser_UsersController()
        {
            //Arrange 
            _testUser = await InsertIfNotAny(_testUser);

            //Act
            var getResponseOneUser = await _client.GetAsync(_request + "Get/" + _testUser.Id);
            var fetched = await getResponseOneUser.Content.ReadAsStringAsync();
            var fetchedUser = JsonConvert.DeserializeObject<User>(fetched);

            Assert.IsTrue(getResponseOneUser.IsSuccessStatusCode);
            Assert.AreEqual(_testUser.Id, fetchedUser.Id);
            Assert.AreEqual(_testUser.Name, fetchedUser.Name);

        }

        [Test]
        public async Task Create_CreateAUser_NonExistingEmailInAspNetUsers_UsersController()
        {
            //Arrange 

            //Act
            _testUser.Email = Enums.GetEnumDescription(Enums.UserUpatedTestData.Email);
            var postResponse = await _client.PostAsJsonAsync(_request + "Create", _testUser);

            // Assert
            Assert.IsTrue(postResponse.StatusCode == HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task Create_CreateAUser_ExistingEmailInAspNetUsers_UsersController()
        {
            //Arrange 
            //Create a User in AspNetUsers table
            _testUser = await InsertNewUserAndAspNetUser(_testUser);

            var getResponse = await _client.GetAsync(_request + "Get/" + _testUser.Id);
            var fetched = await getResponse.Content.ReadAsStringAsync();
            var fetchedUser = JsonConvert.DeserializeObject<User>(fetched);

            // Assert
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual(_testUser.Name, _testUser.Name);
            Assert.AreEqual(_testUser.Name, fetchedUser.Name);

            Assert.AreNotEqual(Guid.Empty, _testUser.Id);
            Assert.AreEqual(_testUser.Id, fetchedUser.Id);
        }

        [Test]
        public async Task Create_CreateAUser_ExistingEmailInAspNetUsers_SameEmailInUsersTable_UsersController()
        {
            //Arrange 

            //Act
            //insert Test User in the User Table and AspNetUsers Table
            _testUser = await InsertNewUserAndAspNetUser(_testUser);
            //try to insert the same user in the User Table, should throw an error
            var postResponseSameUser = await _client.PostAsJsonAsync(_request + "Create", _testUser);

            // Assert
            Assert.IsTrue(postResponseSameUser.StatusCode == HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task Update_UpdateAUserViaPUT_UsersController()
        {
            //Arrange 

            //Act
            //POST(Crete)
            var createdUser = await InsertNewUserAndAspNetUser(_testUser);

            //PUT(Update)
            _testUserUpdated.Id = createdUser.Id;
            _testUserUpdated.ASPNETUsersId = createdUser.ASPNETUsersId;

            var putResponse = await _client.PutAsJsonAsync(_request + createdUser.Id, _testUserUpdated);

            //GET
            var getResponse = await _client.GetAsync(_request + "Get/" + _testUserUpdated.Id);
            var fetched = await getResponse.Content.ReadAsStringAsync();
            var fetchedUser = JsonConvert.DeserializeObject<User>(fetched);

            _testUserUpdated.Id = fetchedUser.Id;

            // Assert
            Assert.IsTrue(putResponse.IsSuccessStatusCode);
            Assert.IsTrue(putResponse.StatusCode == HttpStatusCode.NoContent);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual(Enums.GetEnumDescription(Enums.UserTestData.Name), createdUser.Name);
            Assert.AreEqual(Enums.GetEnumDescription(Enums.UserUpatedTestData.Name), fetchedUser.Name);

            Assert.AreEqual(Enums.GetEnumDescription(Enums.UserTestData.Surname), createdUser.Surname);
            Assert.AreEqual(Enums.GetEnumDescription(Enums.UserUpatedTestData.Surname), fetchedUser.Surname);

            Assert.AreEqual(Enums.GetEnumDescription(Enums.UserTestData.Email), createdUser.Email);
            Assert.AreEqual(Enums.GetEnumDescription(Enums.UserUpatedTestData.Email), fetchedUser.Email);

            Assert.AreEqual(Enums.GetEnumDescription(Enums.UserTestData.HomeNo), createdUser.HomeNo);
            Assert.AreEqual(Enums.GetEnumDescription(Enums.UserUpatedTestData.HomeNo), fetchedUser.HomeNo);

            Assert.AreEqual(Enums.GetEnumDescription(Enums.UserTestData.MobNo), createdUser.MobNo);
            Assert.AreEqual(Enums.GetEnumDescription(Enums.UserUpatedTestData.MobNo), fetchedUser.MobNo);
        }

        [Test]
        public async Task Delete_DeleteAUser_UsersController()
        {
            //Arrange 

            //Act
            //POST(Crete)
            var createdUser = await InsertNewUserAndAspNetUser(_testUser);

            //DELETE
            var deleteResponse = await _client.DeleteAsync(_request + createdUser.Id);
            var getResponse = await _client.GetAsync(_request + "Get");
            var all = getResponse.Content.ReadAsStringAsync();
            var allUsers = JsonConvert.DeserializeObject<List<User>>(all.Result);

            //Assert
            Assert.IsTrue(deleteResponse.IsSuccessStatusCode);
            Assert.IsTrue(deleteResponse.StatusCode == HttpStatusCode.NoContent);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual(_testUser.Name, createdUser.Name);
            Assert.AreNotEqual(Guid.Empty, createdUser.Id);

            if(allUsers.Count > 0)
                Assert.IsTrue(allUsers.Any(x => x.Id == createdUser.Id == false));
        }

        private async Task<User> InsertIfNotAny(User user)
        {
            var getResponse = await _client.GetAsync(_request + "Get");
            var all = getResponse.Content.ReadAsStringAsync();
            var allUsers = JsonConvert.DeserializeObject<List<User>>(all.Result);
            if (allUsers.Count > 0)
            {
                _testUser = allUsers.FirstOrDefault();
            }
            else
            {
                _testUser = await InsertNewUserAndAspNetUser(user);
            }
            return _testUser;
        }

        private async Task<User> InsertNewUserAndAspNetUser(User user)
        {
            var registerViewModel = new RegisterViewModel
            {
                ConfirmPassword = "Password@123",
                Email = user.Email,
                Password = "Password@123"
            };
            var postResponse = await _client.PostAsJsonAsync(_request + "CreateAspNetUsers", registerViewModel);
            var created = await postResponse.Content.ReadAsStringAsync();
            var createdUser = JsonConvert.DeserializeObject<AspNetUsers>(created);


            if (createdUser != null)
                return await InsertNewUser(user);

            return null;
        }

        private async Task<User> InsertNewUser(User user)
        {
            var postResponse = await _client.PostAsJsonAsync(_request + "Create", user);
            var created = await postResponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<User>(created);
        }

        [TearDown]
        public async Task DeleteUsers()
        {
            //Cleanup
            if (_testUserUpdated != null && _testUserUpdated.Id > 0
                && (_testUserUpdated.Name == Enums.GetEnumDescription(Enums.UserTestData.Name)
                || _testUserUpdated.Name == Enums.GetEnumDescription(Enums.UserUpatedTestData.Name)))
            {
                //update the ASPNET User back to the original state
                _testUser.Id = _testUserUpdated.Id;
                _testUser.ASPNETUsersId = _testUserUpdated.ASPNETUsersId;
                await _client.PutAsJsonAsync(_request + _testUserUpdated.Id, _testUser);
            }

            if (_testUser != null && _testUser.Id > 0

                && (_testUser.Name == Enums.GetEnumDescription(Enums.UserTestData.Name)
                || _testUser.Name == Enums.GetEnumDescription(Enums.UserUpatedTestData.Name)))
            {
                await _client.DeleteAsync(_request + _testUser.Id);
                _testUser = null;
            }

        }

    }
}
