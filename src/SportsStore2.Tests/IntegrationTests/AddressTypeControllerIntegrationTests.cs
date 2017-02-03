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
    public class AddressTypeControllerIntegrationTests
    {
        private HttpClient _client;
        private AddressType _testAddressType;
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
            _testAddressType = new AddressType
            {
                Name = Enums.GetEnumDescription(Enums.AddressTypeTestData.Name)
            };
            _request = Enums.GetEnumDescription(Enums.Requests.AddressType);
        }

        [Test]
        public async Task Get_ReturnsAListOfAddressTypes_AddressTypeController()
        {
            var response = await _client.GetAsync(_request + "Get");
            response.EnsureSuccessStatusCode();

            Assert.IsTrue(true);
        }

        [Test]
        public async Task GetById_GetOneAddressType_AddressTypeController()
        {
            //Arrange 
            //Act
            _testAddressType = await InsertAddressTypeIfNotAny();

            var getResponseOneAddressType = await _client.GetAsync(_request + "Get/" + _testAddressType.Id);
            var fetched = await getResponseOneAddressType.Content.ReadAsStringAsync();
            var fetchedAddressType = JsonConvert.DeserializeObject<AddressType>(fetched);

            Assert.IsTrue(getResponseOneAddressType.IsSuccessStatusCode);
            Assert.AreEqual(_testAddressType.Id, fetchedAddressType.Id);
            Assert.AreEqual(_testAddressType.Name, fetchedAddressType.Name);
        }


        [Test]
        public async Task Create_CreateAAddressType_AddressTypeController()
        {
            //Arrange 

            //Act
            var postResponse = await _client.PostAsJsonAsync(_request, _testAddressType);
            var created = await postResponse.Content.ReadAsStringAsync();
            var createdAddressType = JsonConvert.DeserializeObject<AddressType>(created);

            _testAddressType.Id = createdAddressType.Id;

            var getResponse = await _client.GetAsync(_request + "Get/" + createdAddressType.Id);
            var fetched = await getResponse.Content.ReadAsStringAsync();
            var fetchedAddressType = JsonConvert.DeserializeObject<AddressType>(fetched);

            // Assert
            Assert.IsTrue(postResponse.IsSuccessStatusCode);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual(_testAddressType.Name, createdAddressType.Name);
            Assert.AreEqual(_testAddressType.Name, fetchedAddressType.Name);

            Assert.AreNotEqual(Guid.Empty, createdAddressType.Id);
            Assert.AreEqual(_testAddressType.Id, fetchedAddressType.Id);
        }

        [Test]
        public async Task Update_UpdateAAddressTypeViaPUT_AddressTypeController()
        {
            //Arrange 

            //Act
            //POST(Crete)
            var postResponse = await _client.PostAsJsonAsync(_request, _testAddressType);
            var created = await postResponse.Content.ReadAsStringAsync();
            var createdAddressType = JsonConvert.DeserializeObject<AddressType>(created);

            //PUT(Update)
            _testAddressType.Id = createdAddressType.Id;
            _testAddressType.Name = Enums.GetEnumDescription(Enums.AddressTypeUpdtedTestData.Name);
            var putResponse = await _client.PutAsJsonAsync(_request + createdAddressType.Id, _testAddressType);

            //GET
            var getResponse = await _client.GetAsync(_request + "Get/" + _testAddressType.Id);
            var fetched = await getResponse.Content.ReadAsStringAsync();
            var fetchedAddressType = JsonConvert.DeserializeObject<AddressType>(fetched);

            _testAddressType.Id = fetchedAddressType.Id;

            // Assert
            Assert.IsTrue(postResponse.IsSuccessStatusCode);
            Assert.IsTrue(putResponse.IsSuccessStatusCode);
            Assert.IsTrue(putResponse.StatusCode == HttpStatusCode.NoContent);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual(Enums.GetEnumDescription(Enums.AddressTypeUpdtedTestData.Name), fetchedAddressType.Name);

            Assert.AreNotEqual(Guid.Empty, createdAddressType.Id);
            Assert.AreEqual(createdAddressType.Id, fetchedAddressType.Id);

        }

        [Test]
        public async Task Delete_DeleteAAddressType_AddressTypeController()
        {
            //Arrange 

            //Act
            //POST(Crete)
            var postResponse = await _client.PostAsJsonAsync(_request, _testAddressType);
            var created = await postResponse.Content.ReadAsStringAsync();
            var createdAddressType = JsonConvert.DeserializeObject<AddressType>(created);

            //DELETE
            var deleteResponse = await _client.DeleteAsync(_request + createdAddressType.Id);
            var getResponse = await _client.GetAsync(_request + "Get");
            var all = getResponse.Content.ReadAsStringAsync();
            var allAddressTypes = JsonConvert.DeserializeObject<List<AddressType>>(all.Result);

            //Assert
            Assert.IsTrue(postResponse.IsSuccessStatusCode);
            Assert.IsTrue(deleteResponse.IsSuccessStatusCode);
            Assert.IsTrue(deleteResponse.StatusCode == HttpStatusCode.NoContent);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual(_testAddressType.Name, createdAddressType.Name);
            Assert.AreNotEqual(Guid.Empty, createdAddressType.Id);

            Assert.IsTrue(allAddressTypes.Any(x => x.Id == createdAddressType.Id == false));
        }

        private async Task<AddressType> InsertAddressTypeIfNotAny()
        {
            var getResponse = await _client.GetAsync(_request + "Get");
            var all = getResponse.Content.ReadAsStringAsync();
            var allAddressType = JsonConvert.DeserializeObject<List<AddressType>>(all.Result);
            if (allAddressType.Count > 0)
            {
                _testAddressType = allAddressType.FirstOrDefault();
            }
            else
            {
                var postResponse = await _client.PostAsJsonAsync(_request, _testAddressType);
                var created = await postResponse.Content.ReadAsStringAsync();
                _testAddressType = JsonConvert.DeserializeObject<AddressType>(created);
            }

            return _testAddressType;
        }

        [TearDown]
        public async Task DeleteAddressType()
        {
            //Cleanup
            if (_testAddressType != null && _testAddressType.Id > 0 
                && (_testAddressType.Name == Enums.GetEnumDescription(Enums.AddressTypeTestData.Name) 
                || _testAddressType.Name == Enums.GetEnumDescription(Enums.AddressTypeUpdtedTestData.Name)))
            {
                await _client.DeleteAsync(_request + _testAddressType.Id);
                _testAddressType = null;
            }
        }


    }
}
