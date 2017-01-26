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
    public class CountriesControllerIntegrationTests
    {
        private HttpClient _client;
        private Country testCountry;
        private string request ;

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
            testCountry = new Country { Name = "testCountry", Code = "TC", Type = "General" };
            request = "api/Countries/";
        }

        [Test]
        public async Task Get_ReturnsAListOfCountries_CountriesController()
        {
            var response = await _client.GetAsync(request + "Get");
            response.EnsureSuccessStatusCode();

            Assert.IsTrue(true);
        }

        [Test]
        public async Task GetById_GetOneCountry_CountriesController()
        {
            //Arrange 
            Country selectedCountry = null;

            //Act
            var getResponse = await _client.GetAsync(request + "Get");
            var all = getResponse.Content.ReadAsStringAsync();
            var allCountries = JsonConvert.DeserializeObject<List<Country>>(all.Result);
            if (allCountries.Count > 0)
            {
                selectedCountry = allCountries.FirstOrDefault();
            }
            else
            {
                var postResponse = await _client.PostAsJsonAsync(request, testCountry);
                var created = await postResponse.Content.ReadAsStringAsync();
                selectedCountry = JsonConvert.DeserializeObject<Country>(created);

                testCountry.Id = selectedCountry.Id;
            }

            var getResponseOneCountry = await _client.GetAsync(request + "Get/" + selectedCountry.Id);
            var fetched = await getResponseOneCountry.Content.ReadAsStringAsync();
            var fetchedCountry = JsonConvert.DeserializeObject<Country>(fetched);

            Assert.IsTrue(getResponse.IsSuccessStatusCode);
            Assert.IsTrue(getResponseOneCountry.IsSuccessStatusCode);
            Assert.AreEqual(selectedCountry.Id, fetchedCountry.Id);
            Assert.AreEqual(selectedCountry.Name, fetchedCountry.Name);

        }

        [Test]
        public async Task Create_CreateACountry_CountriesController()
        {
            //Arrange 

            //Act
            var postResponse = await _client.PostAsJsonAsync(request, testCountry);
            var created = await postResponse.Content.ReadAsStringAsync();
            var createdCountry = JsonConvert.DeserializeObject<Country>(created);

            testCountry.Id = createdCountry.Id;

            var getResponse = await _client.GetAsync(request + "Get/" + createdCountry.Id);
            var fetched = await getResponse.Content.ReadAsStringAsync();
            var fetchedCountry = JsonConvert.DeserializeObject<Country>(fetched);

            // Assert
            Assert.IsTrue(postResponse.IsSuccessStatusCode);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual(testCountry.Name, createdCountry.Name);
            Assert.AreEqual(testCountry.Name, fetchedCountry.Name);

            Assert.AreNotEqual(Guid.Empty, createdCountry.Id);
            Assert.AreEqual(testCountry.Id, fetchedCountry.Id);
        }

        [Test]
        public async Task Update_UpdateACountryViaPUT_CountriesController()
        {
            //Arrange 

            //Act
            //POST(Crete)
            var postResponse = await _client.PostAsJsonAsync(request, testCountry);
            var created = await postResponse.Content.ReadAsStringAsync();
            var createdCountry = JsonConvert.DeserializeObject<Country>(created);

            //PUT(Update)
            testCountry.Id = createdCountry.Id;
            testCountry.Name = "testCountryUpdated";
            testCountry.Code = "UP";
            testCountry.Type = "Updated";
            var putResponse = await _client.PutAsJsonAsync(request + createdCountry.Id, testCountry);

            //GET
            var getResponse = await _client.GetAsync(request + "Get/" + testCountry.Id);
            var fetched = await getResponse.Content.ReadAsStringAsync();
            var fetchedCountry = JsonConvert.DeserializeObject<Country>(fetched);

            testCountry.Id = fetchedCountry.Id;

            // Assert
            Assert.IsTrue(postResponse.IsSuccessStatusCode);
            Assert.IsTrue(putResponse.IsSuccessStatusCode);
            Assert.IsTrue(putResponse.StatusCode == HttpStatusCode.NoContent);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual("testCountryUpdated", fetchedCountry.Name);
            Assert.AreEqual("UP", fetchedCountry.Code);
            Assert.AreEqual("Updated", fetchedCountry.Type);

            Assert.AreNotEqual(Guid.Empty, createdCountry.Id);
            Assert.AreEqual(createdCountry.Id, fetchedCountry.Id);

        }

        [Test]
        public async Task Delete_DeleteACountry_CountriesController()
        {
            //Arrange 

            //Act
            //POST(Crete)
            var postResponse = await _client.PostAsJsonAsync(request, testCountry);
            var created = await postResponse.Content.ReadAsStringAsync();
            var createdCountry = JsonConvert.DeserializeObject<Country>(created);

            //DELETE
            var deleteResponse = await _client.DeleteAsync(request + createdCountry.Id);
            var getResponse = await _client.GetAsync(request + "Get");
            var all = getResponse.Content.ReadAsStringAsync();
            var allCountries = JsonConvert.DeserializeObject<List<Country>>(all.Result);

            //Assert
            Assert.IsTrue(postResponse.IsSuccessStatusCode);
            Assert.IsTrue(deleteResponse.IsSuccessStatusCode);
            Assert.IsTrue(deleteResponse.StatusCode == HttpStatusCode.NoContent);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual(testCountry.Name, createdCountry.Name);
            Assert.AreNotEqual(Guid.Empty, createdCountry.Id);

            Assert.IsTrue(allCountries.Any(x => x.Id == createdCountry.Id == false));
        }


        [TearDown]
        public async Task DeleteCountry()
        {
            //Cleanup
            if (testCountry != null && testCountry.Id > 0)
            {
                await _client.DeleteAsync(request + testCountry.Id);
                testCountry = null;
            }
        }

    }
}
