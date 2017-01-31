using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SportsStore2.API;
using SportsStore2.API.Models;

namespace SportsStore2.Tests.IntegrationTests
{
    [TestFixture]
    public class UsersControllerIntegrationTests
    {
        private HttpClient _client;
        private User testUser;
        private User testUserUpdated;
        private string request;

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

            testUser = new User {
                Name = "Johann",
                Surname = "Montfort",
                Email = "jmonfu@gmail.com",
                HomeNo = "07460634348",
                MobNo = "02460634348"
            };

            testUserUpdated = new User
            {
                Name = "testUserNameUpdated",
                Surname = "testUserSurnameUpdated",
                Email = "testEmailUpdated@gmail.com",
                HomeNo = "000000000",
                MobNo = "000000000"
            };

            request = "api/Users/";
        }

        [Test]
        public async Task Get_ReturnsAListOfUsers_UsersController()
        {
            request = request + "Get";
            var response = await _client.GetAsync(request);
            response.EnsureSuccessStatusCode();

            Assert.IsTrue(true);
        }

        [Test]
        public async Task GetById_GetOneUser_UsersController()
        {
            //Arrange 
            User selectedUser = null;

            //Act
            var getResponse = await _client.GetAsync(request + "Get");
            var all = getResponse.Content.ReadAsStringAsync();
            var allUsers = JsonConvert.DeserializeObject<List<User>>(all.Result);
            if (allUsers.Count > 0)
            {
                selectedUser = allUsers.FirstOrDefault();
            }
            else
            {
                var postResponse = await _client.PostAsJsonAsync(request, testUser);
                var created = await postResponse.Content.ReadAsStringAsync();
                selectedUser = JsonConvert.DeserializeObject<User>(created);

                testUser.Id = selectedUser.Id;
            }

            var getResponseOneUser = await _client.GetAsync(request + "Get/" + selectedUser.Id);
            var fetched = await getResponseOneUser.Content.ReadAsStringAsync();
            var fetchedUser = JsonConvert.DeserializeObject<User>(fetched);

            Assert.IsTrue(getResponse.IsSuccessStatusCode);
            Assert.IsTrue(getResponseOneUser.IsSuccessStatusCode);
            Assert.AreEqual(selectedUser.Id, fetchedUser.Id);
            Assert.AreEqual(selectedUser.Name, fetchedUser.Name);

        }

        [Test]
        public async Task Create_CreateAUser_NonExistingEmailInAspNetUsers_UsersController()
        {
            //Arrange 

            //Act
            testUser.Email = "test@test.com";
            var postResponse = await _client.PostAsJsonAsync(request, testUser);

            // Assert
            Assert.IsTrue(postResponse.StatusCode == HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task Create_CreateAUser_ExistingEmailInAspNetUsers_UsersController()
        {
            //Arrange 

            //Act

            var postResponse = await _client.PostAsJsonAsync(request, testUser);
            var created = await postResponse.Content.ReadAsStringAsync();
            var createdUser = JsonConvert.DeserializeObject<User>(created);

            testUser.Id = createdUser.Id;

            var getResponse = await _client.GetAsync(request + "Get/" + createdUser.Id);
            var fetched = await getResponse.Content.ReadAsStringAsync();
            var fetchedUser = JsonConvert.DeserializeObject<User>(fetched);

            // Assert
            Assert.IsTrue(postResponse.IsSuccessStatusCode);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual(testUser.Name, createdUser.Name);
            Assert.AreEqual(testUser.Name, fetchedUser.Name);

            Assert.AreNotEqual(Guid.Empty, createdUser.Id);
            Assert.AreEqual(createdUser.Id, fetchedUser.Id);
        }

        [Test]
        public async Task Create_CreateAUser_ExistingEmailInAspNetUsers_SameEmailInUsersTable_UsersController()
        {
            //Arrange 

            //Act
            //insert Test User in the User Table
            var postResponse = await _client.PostAsJsonAsync(request, testUser);
            var created = await postResponse.Content.ReadAsStringAsync();
            var createdUser = JsonConvert.DeserializeObject<User>(created);

            testUser.Id = createdUser.Id;

            //try to insert the same user in the User Table, should throw an error
            var postResponseSameUser = await _client.PostAsJsonAsync(request, testUser);

            // Assert
            Assert.IsTrue(postResponseSameUser.StatusCode == HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task Update_UpdateAUserViaPUT_UsersController()
        {
            //Arrange 

            //Act
            //POST(Crete)
            var postResponse = await _client.PostAsJsonAsync(request, testUser);
            var created = await postResponse.Content.ReadAsStringAsync();
            var createdUser = JsonConvert.DeserializeObject<User>(created);

            //PUT(Update)
            testUserUpdated.Id = createdUser.Id;
            testUserUpdated.ASPNETUsersId = createdUser.ASPNETUsersId;

            var putResponse = await _client.PutAsJsonAsync(request + createdUser.Id, testUserUpdated);

            //GET
            var getResponse = await _client.GetAsync(request + "Get/" + testUserUpdated.Id);
            var fetched = await getResponse.Content.ReadAsStringAsync();
            var fetchedUser = JsonConvert.DeserializeObject<User>(fetched);

            testUserUpdated.Id = fetchedUser.Id;

            // Assert
            Assert.IsTrue(postResponse.IsSuccessStatusCode);
            Assert.IsTrue(putResponse.IsSuccessStatusCode);
            Assert.IsTrue(putResponse.StatusCode == HttpStatusCode.NoContent);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual("Johann", createdUser.Name);
            Assert.AreEqual("testUserNameUpdated", fetchedUser.Name);

            Assert.AreEqual("Montfort", createdUser.Surname);
            Assert.AreEqual("testUserSurnameUpdated", fetchedUser.Surname);

            Assert.AreEqual("jmonfu@gmail.com", createdUser.Email);
            Assert.AreEqual("testEmailUpdated@gmail.com", fetchedUser.Email);

            Assert.AreEqual("07460634348", createdUser.HomeNo);
            Assert.AreEqual("000000000", fetchedUser.HomeNo);

            Assert.AreEqual("02460634348", createdUser.MobNo);
            Assert.AreEqual("000000000", fetchedUser.MobNo);
        }

        [Test]
        public async Task Delete_DeleteAUser_UsersController()
        {
            //Arrange 

            //Act
            //POST(Crete)
            var postResponse = await _client.PostAsJsonAsync(request, testUser);
            var created = await postResponse.Content.ReadAsStringAsync();
            var createdUser = JsonConvert.DeserializeObject<User>(created);

            //DELETE
            var deleteResponse = await _client.DeleteAsync(request + createdUser.Id);
            var getResponse = await _client.GetAsync(request + "Get");
            var all = getResponse.Content.ReadAsStringAsync();
            var allUsers = JsonConvert.DeserializeObject<List<User>>(all.Result);

            //Assert
            Assert.IsTrue(postResponse.IsSuccessStatusCode);
            Assert.IsTrue(deleteResponse.IsSuccessStatusCode);
            Assert.IsTrue(deleteResponse.StatusCode == HttpStatusCode.NoContent);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual(testUser.Name, createdUser.Name);
            Assert.AreNotEqual(Guid.Empty, createdUser.Id);

            if(allUsers.Count > 0)
                Assert.IsTrue(allUsers.Any(x => x.Id == createdUser.Id == false));
        }

        [TearDown]
        public async Task DeleteUsers()
        {
            //Cleanup
            if (testUserUpdated != null && testUserUpdated.Id > 0)
            {
                //update the ASPNET User back to the original state
                testUser.Id = testUserUpdated.Id;
                testUser.ASPNETUsersId = testUserUpdated.ASPNETUsersId;
                await _client.PutAsJsonAsync(request + testUserUpdated.Id, testUser);
            }
            if (testUser != null && testUser.Id > 0)
            {
                await _client.DeleteAsync(request + testUser.Id);
                testUser = null;
            }

        }

    }
}
