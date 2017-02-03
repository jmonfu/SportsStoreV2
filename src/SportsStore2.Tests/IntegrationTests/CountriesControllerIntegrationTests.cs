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
        private Country _testCountry;
        private string _request ;

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
            _testCountry = new Country
            {
                Name = Enums.GetEnumDescription(Enums.CountryTestData.Name),
                Code = Enums.GetEnumDescription(Enums.CountryTestData.Code),
                Type = Enums.GetEnumDescription(Enums.CountryTestData.Type)
            };
            _request = Enums.GetEnumDescription(Enums.Requests.Countries);
        }

        [Test]
        public async Task Get_ReturnsAListOfCountries_CountriesController()
        {
            var response = await _client.GetAsync(_request + "Get");
            response.EnsureSuccessStatusCode();

            Assert.IsTrue(true);
        }

        [Test]
        public async Task GetById_GetOneCountry_CountriesController()
        {
            //Arrange 
            _testCountry = await InsertIfNotAny();

            //Act
            var getResponseOneCountry = await _client.GetAsync(_request + "Get/" + _testCountry.Id);
            var fetched = await getResponseOneCountry.Content.ReadAsStringAsync();
            var fetchedCountry = JsonConvert.DeserializeObject<Country>(fetched);

            Assert.IsTrue(getResponseOneCountry.IsSuccessStatusCode);
            Assert.AreEqual(_testCountry.Id, fetchedCountry.Id);
            Assert.AreEqual(_testCountry.Name, fetchedCountry.Name);

        }

        [Test]
        public async Task Create_CreateACountry_CountriesController()
        {
            //Arrange 

            //Act
            var postResponse = await _client.PostAsJsonAsync(_request, _testCountry);
            var created = await postResponse.Content.ReadAsStringAsync();
            var createdCountry = JsonConvert.DeserializeObject<Country>(created);

            _testCountry.Id = createdCountry.Id;

            var getResponse = await _client.GetAsync(_request + "Get/" + createdCountry.Id);
            var fetched = await getResponse.Content.ReadAsStringAsync();
            var fetchedCountry = JsonConvert.DeserializeObject<Country>(fetched);

            // Assert
            Assert.IsTrue(postResponse.IsSuccessStatusCode);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual(_testCountry.Name, createdCountry.Name);
            Assert.AreEqual(_testCountry.Name, fetchedCountry.Name);

            Assert.AreNotEqual(Guid.Empty, createdCountry.Id);
            Assert.AreEqual(_testCountry.Id, fetchedCountry.Id);
        }

        [Test]
        public async Task Update_UpdateACountryViaPUT_CountriesController()
        {
            //Arrange 

            //Act
            //POST(Crete)
            var postResponse = await _client.PostAsJsonAsync(_request, _testCountry);
            var created = await postResponse.Content.ReadAsStringAsync();
            var createdCountry = JsonConvert.DeserializeObject<Country>(created);

            //PUT(Update)
            _testCountry.Id = createdCountry.Id;
            _testCountry.Name = Enums.GetEnumDescription(Enums.CountryUpdatedTestData.Name);
            _testCountry.Code = Enums.GetEnumDescription(Enums.CountryUpdatedTestData.Code);
            _testCountry.Type = Enums.GetEnumDescription(Enums.CountryUpdatedTestData.Type);
            var putResponse = await _client.PutAsJsonAsync(_request + createdCountry.Id, _testCountry);

            //GET
            var getResponse = await _client.GetAsync(_request + "Get/" + _testCountry.Id);
            var fetched = await getResponse.Content.ReadAsStringAsync();
            var fetchedCountry = JsonConvert.DeserializeObject<Country>(fetched);

            _testCountry.Id = fetchedCountry.Id;

            // Assert
            Assert.IsTrue(postResponse.IsSuccessStatusCode);
            Assert.IsTrue(putResponse.IsSuccessStatusCode);
            Assert.IsTrue(putResponse.StatusCode == HttpStatusCode.NoContent);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual(Enums.GetEnumDescription(Enums.CountryUpdatedTestData.Name), fetchedCountry.Name);
            Assert.AreEqual(Enums.GetEnumDescription(Enums.CountryUpdatedTestData.Code), fetchedCountry.Code);
            Assert.AreEqual(Enums.GetEnumDescription(Enums.CountryUpdatedTestData.Type), fetchedCountry.Type);

            Assert.AreNotEqual(Guid.Empty, createdCountry.Id);
            Assert.AreEqual(createdCountry.Id, fetchedCountry.Id);

        }

        [Test]
        public async Task Delete_DeleteACountry_CountriesController()
        {
            //Arrange 

            //Act
            //POST(Crete)
            var postResponse = await _client.PostAsJsonAsync(_request, _testCountry);
            var created = await postResponse.Content.ReadAsStringAsync();
            var createdCountry = JsonConvert.DeserializeObject<Country>(created);

            //DELETE
            var deleteResponse = await _client.DeleteAsync(_request + createdCountry.Id);
            var getResponse = await _client.GetAsync(_request + "Get");
            var all = getResponse.Content.ReadAsStringAsync();
            var allCountries = JsonConvert.DeserializeObject<List<Country>>(all.Result);

            //Assert
            Assert.IsTrue(postResponse.IsSuccessStatusCode);
            Assert.IsTrue(deleteResponse.IsSuccessStatusCode);
            Assert.IsTrue(deleteResponse.StatusCode == HttpStatusCode.NoContent);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual(_testCountry.Name, createdCountry.Name);
            Assert.AreNotEqual(Guid.Empty, createdCountry.Id);

            Assert.IsTrue(allCountries.Any(x => x.Id == createdCountry.Id == false));
        }

        private async Task<Country> InsertIfNotAny()
        {
            var getResponse = await _client.GetAsync(_request + "Get");
            var all = getResponse.Content.ReadAsStringAsync();
            var allCountries = JsonConvert.DeserializeObject<List<Country>>(all.Result);
            if (allCountries.Count > 0)
            {
                _testCountry = allCountries.FirstOrDefault();
            }
            else
            {
                var postResponse = await _client.PostAsJsonAsync(_request, _testCountry);
                var created = await postResponse.Content.ReadAsStringAsync();
                _testCountry = JsonConvert.DeserializeObject<Country>(created);

            }
            return _testCountry;
        }


        [TearDown]
        public async Task DeleteCountry()
        {
            //Cleanup
            if (_testCountry != null && _testCountry.Id > 0
                && (_testCountry.Name == Enums.GetEnumDescription(Enums.CountryTestData.Name)
                || _testCountry.Name == Enums.GetEnumDescription(Enums.CountryUpdatedTestData.Name)))
            {
                await _client.DeleteAsync(_request + _testCountry.Id);
                _testCountry = null;
            }
        }

    }
}
