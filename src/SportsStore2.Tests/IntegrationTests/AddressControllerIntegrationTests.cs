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
using SportsStore2.API;
using SportsStore2.API.Models;

namespace SportsStore2.Tests.IntegrationTests
{
    [TestFixture]
    public class AddressControllerIntegrationTests
    {
        private HttpClient _client;
        private Address testAddress;
        private Country testCountry;
        private User testUser;
        private string request;
        private string requestUser;
        private string requestCountry;

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
            testCountry = new Country
            {
                Name = "testCountry",
                Code = "TT",
                Type = "1 Tier"
            };

            testUser = new User
            {
                Name = "Johann",
                Surname = "Montfort",
                Email = "jmonfu@gmail.com",
                HomeNo = "07460634348",
                MobNo = "02460634348"
            };

            testAddress = new Address
            {
                Address1 = "TestAddress1",
                Address2 = "TestAddress2",
                Address3 = "TestAddress3",
                AddressTypeId = 1,
                City = "TestCity",
                PostCode = "E14 2DA"
            };

            request = "api/Address/";
            requestUser = "api/Users/";
            requestCountry = "api/Countries/";
        }


        [Test]
        public async Task Get_ReturnsAListOfAddresses_AddressController()
        {
            var response = await _client.GetAsync(request + "Get");
            response.EnsureSuccessStatusCode();

            Assert.IsTrue(true);
        }

        [Test]
        public async Task GetById_GetOneAddress_AddressController()
        {
            //Arrange 
            Address selectedAddress = null;

            //Act
            var getResponse = await _client.GetAsync(request + "Get");
            var all = getResponse.Content.ReadAsStringAsync();
            var allAddresses = JsonConvert.DeserializeObject<List<Address>>(all.Result);
            if (allAddresses.Count > 0)
            {
                selectedAddress = allAddresses.FirstOrDefault();
            }
            else
            {
                //insert new country
                var newCountry = InsertNewCountry().Result;
                testCountry.Id = newCountry.Id;

                //insert new user
                var newUser = InsertNewUser().Result;
                testUser.Id = newUser.Id;

                testAddress.CountryId = testCountry.Id;
                testAddress.UserId = testUser.Id;

                var postResponse = await _client.PostAsJsonAsync(request, testAddress);
                var created = await postResponse.Content.ReadAsStringAsync();
                selectedAddress= JsonConvert.DeserializeObject<Address>(created);

                testAddress.Id = selectedAddress.Id;
            }

            var getResponseOneAddress = await _client.GetAsync(request + "Get/" + selectedAddress.Id);
            var fetched = await getResponseOneAddress.Content.ReadAsStringAsync();
            var fetchedAddress = JsonConvert.DeserializeObject<Address>(fetched);

            Assert.IsTrue(getResponse.IsSuccessStatusCode);
            Assert.IsTrue(getResponseOneAddress.IsSuccessStatusCode);
            Assert.AreEqual(selectedAddress.Id, fetchedAddress.Id);
            Assert.AreEqual(selectedAddress.Address1, fetchedAddress.Address1);
            Assert.AreEqual(selectedAddress.City, fetchedAddress.City);
            Assert.AreEqual(selectedAddress.Country, fetchedAddress.Country);
            Assert.AreEqual(selectedAddress.PostCode, fetchedAddress.PostCode);

        }

        [Test]
        public async Task Create_CreateAnAddress_NewCountryNewUser_AddressController()
        {
            //Arrange 
            //insert new country
            var newCountry = InsertNewCountry().Result;
            testCountry.Id = newCountry.Id;

            //insert new user
            var newUser = InsertNewUser().Result;
            testUser.Id = newUser.Id;

            testAddress.CountryId = testCountry.Id;
            testAddress.UserId = testUser.Id;

            //Act
            var postResponse = await _client.PostAsJsonAsync(request, testAddress);
            var created = await postResponse.Content.ReadAsStringAsync();
            var createdAddress = JsonConvert.DeserializeObject<Address>(created);

            testAddress.Id = createdAddress.Id;

            //for cleanup
            var getResponse = await _client.GetAsync(request + "Get/" + createdAddress.Id);
            var fetched = await getResponse.Content.ReadAsStringAsync();
            var fetchedAddress = JsonConvert.DeserializeObject<Address>(fetched);

            // Assert
            Assert.IsTrue(postResponse.IsSuccessStatusCode);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual(testAddress.Address1, createdAddress.Address1);
            Assert.AreEqual(testAddress.Address1, fetchedAddress.Address1);
            Assert.AreEqual(testAddress.Address2, createdAddress.Address2);
            Assert.AreEqual(testAddress.Address2, fetchedAddress.Address2);
            Assert.AreEqual(testAddress.Address3, createdAddress.Address3);
            Assert.AreEqual(testAddress.Address3, fetchedAddress.Address3);
            Assert.AreEqual(testAddress.City, createdAddress.City);
            Assert.AreEqual(testAddress.City, fetchedAddress.City);
            Assert.AreEqual(testAddress.Country, createdAddress.Country);
            Assert.AreEqual(testAddress.Country, fetchedAddress.Country);
            Assert.AreEqual(testAddress.User, createdAddress.User);
            Assert.AreEqual(testAddress.User, fetchedAddress.User);

            Assert.AreNotEqual(Guid.Empty, createdAddress.Id);
            Assert.AreEqual(createdAddress.Id, createdAddress.Id);
        }

        [Test]
        public async Task Create_CreateAnAddress_ExisitingCountryExisitingUser_AddressController()
        {
            //Arrange 
            //get country, if there aren't any, create one
            var selectedCountry = await GetCountry();

            //get user, if there aren't any, create one
            var selectedUser = GetUser();

            testAddress.Country = selectedCountry;
            testAddress.CountryId = selectedCountry.Id;
            testAddress.User = selectedUser.Result;
            testAddress.UserId = selectedUser.Result.Id;

            //Act
            var postResponse = await _client.PostAsJsonAsync(request, testAddress);
            var created = await postResponse.Content.ReadAsStringAsync();
            var createdAddress = JsonConvert.DeserializeObject<Address>(created);

            testAddress.Id = createdAddress.Id;

            //for cleanup
            var getResponse = await _client.GetAsync(request + "Get/" + createdAddress.Id);
            var fetched = await getResponse.Content.ReadAsStringAsync();
            var fetchedAddress = JsonConvert.DeserializeObject<Address>(fetched);

            // Assert
            Assert.IsTrue(postResponse.IsSuccessStatusCode);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual(testAddress.Address1, createdAddress.Address1);
            Assert.AreEqual(testAddress.Address1, fetchedAddress.Address1);
            Assert.AreEqual(testAddress.Address2, createdAddress.Address2);
            Assert.AreEqual(testAddress.Address2, fetchedAddress.Address2);
            Assert.AreEqual(testAddress.Address3, createdAddress.Address3);
            Assert.AreEqual(testAddress.Address3, fetchedAddress.Address3);
            Assert.AreEqual(testAddress.City, createdAddress.City);
            Assert.AreEqual(testAddress.City, fetchedAddress.City);
            Assert.AreEqual(testAddress.CountryId, createdAddress.CountryId);
            Assert.AreEqual(testAddress.CountryId, fetchedAddress.CountryId);
            Assert.AreEqual(testAddress.UserId, createdAddress.UserId);
            Assert.AreEqual(testAddress.UserId, fetchedAddress.UserId);

            Assert.AreNotEqual(Guid.Empty, createdAddress.Id);
            Assert.AreEqual(createdAddress.Id, createdAddress.Id);
        }

        [Test]
        public async Task Update_UpdateAnAddressViaPUT_AddressController()
        {
            //Arrange
            //POST(Crete)

            //get country, if there aren't any, create one
            var selectedCountry = await GetCountry();
            
            //get user, if there aren't any, create one
            var selectedUser = await GetUser();

            testAddress.CountryId = selectedCountry.Id;
            testAddress.UserId = selectedUser.Id;

            var postResponse = await _client.PostAsJsonAsync(request, testAddress);
            var created = await postResponse.Content.ReadAsStringAsync();
            var createdAddress = JsonConvert.DeserializeObject<Address>(created);

            testAddress.Id = createdAddress.Id;

            var updatedAddress = new Address
            {
                Id = createdAddress.Id,
                CountryId = createdAddress.CountryId,
                UserId = createdAddress.UserId,
                Address1 = "TestAddress1Updated",
                Address2 = "TestAddress2Updated",
                Address3 = "TestAddress3Updated",
                AddressTypeId = 1,
                City = "TestCityUpdated",
                PostCode = "A12 3AA"
            };

            var putResponse = await _client.PutAsJsonAsync(request + createdAddress.Id, updatedAddress);

            //GET
            var getResponse = await _client.GetAsync(request + "Get/" + updatedAddress.Id);
            var fetched = await getResponse.Content.ReadAsStringAsync();
            var fetchedAddress = JsonConvert.DeserializeObject<Address>(fetched);

            // Assert
            Assert.IsTrue(postResponse.IsSuccessStatusCode);
            Assert.IsTrue(putResponse.IsSuccessStatusCode);
            Assert.IsTrue(putResponse.StatusCode == HttpStatusCode.NoContent);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual("TestAddress1", createdAddress.Address1);
            Assert.AreEqual("TestAddress1Updated", fetchedAddress.Address1);
            Assert.AreEqual("TestAddress2", createdAddress.Address2);
            Assert.AreEqual("TestAddress2Updated", fetchedAddress.Address2);
            Assert.AreEqual("TestAddress3", createdAddress.Address3);
            Assert.AreEqual("TestAddress3Updated", fetchedAddress.Address3);
            Assert.AreEqual(1, createdAddress.AddressTypeId);
            Assert.AreEqual(1, fetchedAddress.AddressTypeId);
            Assert.AreEqual("TestCity", createdAddress.City);
            Assert.AreEqual("TestCityUpdated", fetchedAddress.City);
            Assert.AreEqual("E14 2DA", createdAddress.PostCode);
            Assert.AreEqual("A12 3AA", fetchedAddress.PostCode);

            Assert.AreNotEqual(Guid.Empty, createdAddress.Id);
            Assert.AreEqual(createdAddress.Id, fetchedAddress.Id);

        }

        [Test]
        public async Task Delete_DeleteAnAddress_AddressController()
        {
            //Arrange 

            //Act
            //POST(Crete)
            //get country, if there aren't any, create one
            var selectedCountry = await GetCountry();

            //get user, if there aren't any, create one
            var selectedUser = await GetUser();

            testAddress.CountryId = selectedCountry.Id;
            testAddress.UserId = selectedUser.Id;

            var postResponse = await _client.PostAsJsonAsync(request, testAddress);
            var created = await postResponse.Content.ReadAsStringAsync();
            var createdAddress = JsonConvert.DeserializeObject<Address>(created);

            testAddress.Id = createdAddress.Id;

            //DELETE
            var deleteResponse = await _client.DeleteAsync(request+ createdAddress.Id);
            var getResponse = await _client.GetAsync(request + "Get");
            var all = getResponse.Content.ReadAsStringAsync();
            var allAddresses = JsonConvert.DeserializeObject<List<Address>>(all.Result);

            //for cleanup

            //Assert
            Assert.IsTrue(postResponse.IsSuccessStatusCode);
            Assert.IsTrue(deleteResponse.IsSuccessStatusCode);
            Assert.IsTrue(deleteResponse.StatusCode == HttpStatusCode.NoContent);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual(testAddress.Address1, createdAddress.Address1);
            Assert.AreEqual(testAddress.Address2, createdAddress.Address2);
            Assert.AreEqual(testAddress.Address3, createdAddress.Address3);
            Assert.AreEqual(testAddress.City, createdAddress.City);
            Assert.AreEqual(testAddress.Country, createdAddress.Country);
            Assert.AreEqual(testAddress.User, createdAddress.User);
            Assert.AreNotEqual(Guid.Empty, createdAddress.Id);

            if(allAddresses.Count > 0)
                Assert.IsTrue(allAddresses.Any(x => x.Id == createdAddress.Id == false));
        }

        private async Task<Country> InsertNewCountry()
        {
            var postResponse = await _client.PostAsJsonAsync(requestCountry, testCountry);
            var created = await postResponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Country>(created);
        }

        private async Task<User> InsertNewUser()
        {
            var postResponse = await _client.PostAsJsonAsync(requestUser, testUser);
            var created = await postResponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<User>(created);
        }

        private async Task<Country> GetCountry()
        {
            var getResponseCountry = await _client.GetAsync(requestCountry + "Get");
            var all = getResponseCountry.Content.ReadAsStringAsync();
            var allCountries = JsonConvert.DeserializeObject<List<Country>>(all.Result);
            Country selectedCountry;
            if (allCountries.Count > 0)
            {
                selectedCountry = allCountries.FirstOrDefault();
            }
            else
            {
                selectedCountry = InsertNewCountry().Result;
                testCountry.Id = selectedCountry.Id;
            }

            return selectedCountry;
        }

        private async Task<User> GetUser()
        {
            var getResponseUser = await _client.GetAsync(requestUser + "Get");
            var allUsers = getResponseUser.Content.ReadAsStringAsync();
            var allUsersObj = JsonConvert.DeserializeObject<List<User>>(allUsers.Result);
            User selectedUser;
            if (allUsersObj.Count > 0)
            {
                selectedUser = allUsersObj.FirstOrDefault();
            }
            else
            {
                selectedUser = InsertNewUser().Result;
                testUser.Id = selectedUser.Id;
            }
            return selectedUser;

        }



        [TearDown]
        public async Task DeleteAddress()
        {
            //Cleanup
            if (testAddress != null && testAddress.Id > 0)
            {
                await _client.DeleteAsync(request + testAddress.Id);
                testAddress = null;
            }

            if (testCountry != null && testCountry.Id > 0)
            {
                await _client.DeleteAsync(requestCountry + testCountry.Id);
                testCountry = null;
            }

            if (testUser != null && testUser.Id > 0)
            {
                await _client.DeleteAsync(requestUser + testUser.Id);
                testUser = null;
            }

        }

    }
}
