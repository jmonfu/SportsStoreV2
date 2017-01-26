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
        private AddressType testAddressType;
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
            testAddressType = new AddressType{ Name = "testAddressType" };
            request = "api/AddressType/";
        }

        [Test]
        public async Task Get_ReturnsAListOfAddressTypes_AddressTypeController()
        {
            var response = await _client.GetAsync(request + "Get");
            response.EnsureSuccessStatusCode();

            Assert.IsTrue(true);
        }

        [Test]
        public async Task GetById_GetOneAddressType_AddressTypeController()
        {
            //Arrange 
            AddressType selectedAddressType = null;

            //Act
            var getResponse = await _client.GetAsync(request + "Get");
            var all = getResponse.Content.ReadAsStringAsync();
            var allAddressType = JsonConvert.DeserializeObject<List<AddressType>>(all.Result);
            if (allAddressType.Count > 0)
            {
                selectedAddressType = allAddressType.FirstOrDefault();
            }
            else
            {
                var postResponse = await _client.PostAsJsonAsync(request, testAddressType);
                var created = await postResponse.Content.ReadAsStringAsync();
                selectedAddressType = JsonConvert.DeserializeObject<AddressType>(created);

                testAddressType.Id = selectedAddressType.Id;
            }

            var getResponseOneAddressType = await _client.GetAsync(request + "Get/" + selectedAddressType.Id);
            var fetched = await getResponseOneAddressType.Content.ReadAsStringAsync();
            var fetchedAddressType = JsonConvert.DeserializeObject<AddressType>(fetched);

            Assert.IsTrue(getResponse.IsSuccessStatusCode);
            Assert.IsTrue(getResponseOneAddressType.IsSuccessStatusCode);
            Assert.AreEqual(selectedAddressType.Id, fetchedAddressType.Id);
            Assert.AreEqual(selectedAddressType.Name, fetchedAddressType.Name);
        }

        [Test]
        public async Task Create_CreateAAddressType_AddressTypeController()
        {
            //Arrange 

            //Act
            var postResponse = await _client.PostAsJsonAsync(request, testAddressType);
            var created = await postResponse.Content.ReadAsStringAsync();
            var createdAddressType = JsonConvert.DeserializeObject<AddressType>(created);

            testAddressType.Id = createdAddressType.Id;

            var getResponse = await _client.GetAsync(request + "Get/" + createdAddressType.Id);
            var fetched = await getResponse.Content.ReadAsStringAsync();
            var fetchedAddressType = JsonConvert.DeserializeObject<AddressType>(fetched);

            // Assert
            Assert.IsTrue(postResponse.IsSuccessStatusCode);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual(testAddressType.Name, createdAddressType.Name);
            Assert.AreEqual(testAddressType.Name, fetchedAddressType.Name);

            Assert.AreNotEqual(Guid.Empty, createdAddressType.Id);
            Assert.AreEqual(testAddressType.Id, fetchedAddressType.Id);
        }

        [Test]
        public async Task Update_UpdateAAddressTypeViaPUT_AddressTypeController()
        {
            //Arrange 

            //Act
            //POST(Crete)
            var postResponse = await _client.PostAsJsonAsync(request, testAddressType);
            var created = await postResponse.Content.ReadAsStringAsync();
            var createdAddressType = JsonConvert.DeserializeObject<AddressType>(created);

            //PUT(Update)
            testAddressType.Id = createdAddressType.Id;
            testAddressType.Name = "testAddressTypeUpdated";
            var putResponse = await _client.PutAsJsonAsync(request + createdAddressType.Id, testAddressType);

            //GET
            var getResponse = await _client.GetAsync(request + "Get/" + testAddressType.Id);
            var fetched = await getResponse.Content.ReadAsStringAsync();
            var fetchedAddressType = JsonConvert.DeserializeObject<AddressType>(fetched);

            testAddressType.Id = fetchedAddressType.Id;

            // Assert
            Assert.IsTrue(postResponse.IsSuccessStatusCode);
            Assert.IsTrue(putResponse.IsSuccessStatusCode);
            Assert.IsTrue(putResponse.StatusCode == HttpStatusCode.NoContent);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual("testAddressTypeUpdated", fetchedAddressType.Name);

            Assert.AreNotEqual(Guid.Empty, createdAddressType.Id);
            Assert.AreEqual(createdAddressType.Id, fetchedAddressType.Id);

        }

        [Test]
        public async Task Delete_DeleteAAddressType_AddressTypeController()
        {
            //Arrange 

            //Act
            //POST(Crete)
            var postResponse = await _client.PostAsJsonAsync(request, testAddressType);
            var created = await postResponse.Content.ReadAsStringAsync();
            var createdAddressType = JsonConvert.DeserializeObject<AddressType>(created);

            //DELETE
            var deleteResponse = await _client.DeleteAsync(request + createdAddressType.Id);
            var getResponse = await _client.GetAsync(request + "Get");
            var all = getResponse.Content.ReadAsStringAsync();
            var allCountries = JsonConvert.DeserializeObject<List<AddressType>>(all.Result);

            //Assert
            Assert.IsTrue(postResponse.IsSuccessStatusCode);
            Assert.IsTrue(deleteResponse.IsSuccessStatusCode);
            Assert.IsTrue(deleteResponse.StatusCode == HttpStatusCode.NoContent);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual(testAddressType.Name, createdAddressType.Name);
            Assert.AreNotEqual(Guid.Empty, createdAddressType.Id);

            Assert.IsTrue(allCountries.Any(x => x.Id == createdAddressType.Id == false));
        }

        [TearDown]
        public async Task DeleteAddressType()
        {
            //Cleanup
            if (testAddressType != null && testAddressType.Id > 0)
            {
                await _client.DeleteAsync(request + testAddressType.Id);
                testAddressType = null;
            }
        }


    }
}
