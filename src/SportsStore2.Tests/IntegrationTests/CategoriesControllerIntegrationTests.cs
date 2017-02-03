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
    public class CategoriesControllerIntegrationTests
    {
        private HttpClient _client;
        private Category _testCategory;
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
            _testCategory = new Category
            {
                Name = Enums.GetEnumDescription(Enums.CategoryTestData.Name)
            };
            _request = Enums.GetEnumDescription(Enums.Requests.Categories);
        }

        [Test]
        public async Task Get_ReturnsAListOfCategories_CategoriesController()
        {
            var response = await _client.GetAsync(_request + "Get");
            response.EnsureSuccessStatusCode();

            Assert.IsTrue(true);
        }

        [Test]
        public async Task GetById_GetOneCategory_CategoriesController()
        {
            //Arrange 

            //Act
            _testCategory = await InsertIfNotAny();

            var getResponseOneCategory = await _client.GetAsync(_request + "Get/" + _testCategory.Id);
            var fetched = await getResponseOneCategory.Content.ReadAsStringAsync();
            var fetchedCategory = JsonConvert.DeserializeObject<Category>(fetched);

            Assert.IsTrue(getResponseOneCategory.IsSuccessStatusCode);
            Assert.AreEqual(_testCategory.Id, fetchedCategory.Id);
            Assert.AreEqual(_testCategory.Name, fetchedCategory.Name);
        }


        [Test]
        public async Task Create_CreateACategory_CategoriesController()
        {
            //Arrange 

            //Act
            var postResponse = await _client.PostAsJsonAsync(_request, _testCategory);
            var created = await postResponse.Content.ReadAsStringAsync();
            var createdCategory = JsonConvert.DeserializeObject<Category>(created);

            _testCategory.Id = createdCategory.Id;

            var getResponse = await _client.GetAsync(_request + "Get/" + createdCategory.Id);
            var fetched = await getResponse.Content.ReadAsStringAsync();
            var fetchedCategory = JsonConvert.DeserializeObject<Category>(fetched);

            // Assert
            Assert.IsTrue(postResponse.IsSuccessStatusCode);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual(_testCategory.Name, createdCategory.Name);
            Assert.AreEqual(_testCategory.Name, fetchedCategory.Name);

            Assert.AreNotEqual(Guid.Empty, createdCategory.Id);
            Assert.AreEqual(_testCategory.Id, fetchedCategory.Id);
        }

        [Test]
        public async Task Update_UpdateACategoryViaPUT_CategoriesController()
        {
            //Arrange 

            //Act
            //POST(Crete)
            var postResponse = await _client.PostAsJsonAsync(_request, _testCategory);
            var created = await postResponse.Content.ReadAsStringAsync();
            var createdCategory = JsonConvert.DeserializeObject<Category>(created);

            //PUT(Update)
            _testCategory.Id = createdCategory.Id;
            _testCategory.Name = Enums.GetEnumDescription(Enums.CategoryUpdatedTestData.Name);
            var putResponse = await _client.PutAsJsonAsync(_request + createdCategory.Id, _testCategory);

            //GET
            var getResponse = await _client.GetAsync(_request + "Get/" + _testCategory.Id);
            var fetched = await getResponse.Content.ReadAsStringAsync();
            var fetchedCategory = JsonConvert.DeserializeObject<Category>(fetched);

            _testCategory.Id = fetchedCategory.Id;

            // Assert
            Assert.IsTrue(postResponse.IsSuccessStatusCode);
            Assert.IsTrue(putResponse.IsSuccessStatusCode);
            Assert.IsTrue(putResponse.StatusCode == HttpStatusCode.NoContent);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual(Enums.GetEnumDescription(Enums.CategoryUpdatedTestData.Name), fetchedCategory.Name);

            Assert.AreNotEqual(Guid.Empty, createdCategory.Id);
            Assert.AreEqual(createdCategory.Id, fetchedCategory.Id);

        }

        [Test]
        public async Task Delete_DeleteACategory_CategoriesController()
        {
            //Arrange 

            //Act
            //POST(Crete)
            var postResponse = await _client.PostAsJsonAsync(_request, _testCategory);
            var created = await postResponse.Content.ReadAsStringAsync();
            var createdCategory = JsonConvert.DeserializeObject<Category>(created);

            //DELETE
            var deleteResponse = await _client.DeleteAsync(_request + createdCategory.Id);
            var getResponse = await _client.GetAsync(_request + "Get");
            var all = getResponse.Content.ReadAsStringAsync();
            var allCategories = JsonConvert.DeserializeObject<List<Category>>(all.Result);

            //Assert
            Assert.IsTrue(postResponse.IsSuccessStatusCode);
            Assert.IsTrue(deleteResponse.IsSuccessStatusCode);
            Assert.IsTrue(deleteResponse.StatusCode == HttpStatusCode.NoContent);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual(_testCategory.Name, createdCategory.Name);
            Assert.AreNotEqual(Guid.Empty, createdCategory.Id);

            if(allCategories.Count > 0)
                Assert.IsTrue(allCategories.Any(x => x.Id == createdCategory.Id == false));
        }

        private async Task<Category> InsertIfNotAny()
        {
            var getResponse = await _client.GetAsync(_request + "Get");
            var all = getResponse.Content.ReadAsStringAsync();
            var allCategories = JsonConvert.DeserializeObject<List<Category>>(all.Result);
            if (allCategories.Count > 0)
            {
                _testCategory = allCategories.FirstOrDefault();
            }
            else
            {
                var postResponse = await _client.PostAsJsonAsync(_request, _testCategory);
                var created = await postResponse.Content.ReadAsStringAsync();
                _testCategory = JsonConvert.DeserializeObject<Category>(created);
            }

            return _testCategory;
        }

        [TearDown]
        public async Task DeleteCategory()
        {
            //Cleanup
            if (_testCategory != null && _testCategory.Id > 0
                && (_testCategory.Name == Enums.GetEnumDescription(Enums.CategoryTestData.Name)
                || _testCategory.Name == Enums.GetEnumDescription(Enums.CategoryUpdatedTestData.Name)))
            {
                await _client.DeleteAsync(_request + _testCategory.Id);
                _testCategory = null;
            }
        }

    }
}
