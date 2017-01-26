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
        private Category testCategory;
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
            testCategory = new Category { Name = "testCategory"};
            request = "api/Categories/";
        }

        [Test]
        public async Task Get_ReturnsAListOfCategories_CategoriesController()
        {
            var response = await _client.GetAsync(request + "Get");
            response.EnsureSuccessStatusCode();

            Assert.IsTrue(true);
        }

        [Test]
        public async Task GetById_GetOneCategory_CategoriesController()
        {
            //Arrange 
            Category selectedCategory = null;

            //Act
            var getResponse = await _client.GetAsync(request + "Get");
            var all = getResponse.Content.ReadAsStringAsync();
            var allCategories = JsonConvert.DeserializeObject<List<Category>>(all.Result);
            if (allCategories.Count > 0)
            {
                selectedCategory = allCategories.FirstOrDefault();
            }
            else
            {
                var postResponse = await _client.PostAsJsonAsync(request, testCategory);
                var created = await postResponse.Content.ReadAsStringAsync();
                selectedCategory = JsonConvert.DeserializeObject<Category>(created);

                testCategory.Id = selectedCategory.Id;
            }

            var getResponseOneCategory = await _client.GetAsync(request + "Get/" + selectedCategory.Id);
            var fetched = await getResponseOneCategory.Content.ReadAsStringAsync();
            var fetchedCategory = JsonConvert.DeserializeObject<Category>(fetched);

            Assert.IsTrue(getResponse.IsSuccessStatusCode);
            Assert.IsTrue(getResponseOneCategory.IsSuccessStatusCode);
            Assert.AreEqual(selectedCategory.Id, fetchedCategory.Id);
            Assert.AreEqual(selectedCategory.Name, fetchedCategory.Name);
        }

        [Test]
        public async Task Create_CreateACategory_CategoriesController()
        {
            //Arrange 

            //Act
            var postResponse = await _client.PostAsJsonAsync(request, testCategory);
            var created = await postResponse.Content.ReadAsStringAsync();
            var createdCategory = JsonConvert.DeserializeObject<Category>(created);

            testCategory.Id = createdCategory.Id;

            var getResponse = await _client.GetAsync(request + "Get/" + createdCategory.Id);
            var fetched = await getResponse.Content.ReadAsStringAsync();
            var fetchedCategory = JsonConvert.DeserializeObject<Category>(fetched);

            // Assert
            Assert.IsTrue(postResponse.IsSuccessStatusCode);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual(testCategory.Name, createdCategory.Name);
            Assert.AreEqual(testCategory.Name, fetchedCategory.Name);

            Assert.AreNotEqual(Guid.Empty, createdCategory.Id);
            Assert.AreEqual(testCategory.Id, fetchedCategory.Id);
        }

        [Test]
        public async Task Update_UpdateACategoryViaPUT_CategoriesController()
        {
            //Arrange 

            //Act
            //POST(Crete)
            var postResponse = await _client.PostAsJsonAsync(request, testCategory);
            var created = await postResponse.Content.ReadAsStringAsync();
            var createdCategory = JsonConvert.DeserializeObject<Category>(created);

            //PUT(Update)
            testCategory.Id = createdCategory.Id;
            testCategory.Name = "testCategoryUpdated";
            var putResponse = await _client.PutAsJsonAsync(request + createdCategory.Id, testCategory);

            //GET
            var getResponse = await _client.GetAsync(request + "Get/" + testCategory.Id);
            var fetched = await getResponse.Content.ReadAsStringAsync();
            var fetchedCategory = JsonConvert.DeserializeObject<Category>(fetched);

            testCategory.Id = fetchedCategory.Id;

            // Assert
            Assert.IsTrue(postResponse.IsSuccessStatusCode);
            Assert.IsTrue(putResponse.IsSuccessStatusCode);
            Assert.IsTrue(putResponse.StatusCode == HttpStatusCode.NoContent);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual("testCategoryUpdated", fetchedCategory.Name);

            Assert.AreNotEqual(Guid.Empty, createdCategory.Id);
            Assert.AreEqual(createdCategory.Id, fetchedCategory.Id);

        }

        [Test]
        public async Task Delete_DeleteACategory_CategoriesController()
        {
            //Arrange 

            //Act
            //POST(Crete)
            var postResponse = await _client.PostAsJsonAsync(request, testCategory);
            var created = await postResponse.Content.ReadAsStringAsync();
            var createdCategory = JsonConvert.DeserializeObject<Category>(created);

            //DELETE
            var deleteResponse = await _client.DeleteAsync(request + createdCategory.Id);
            var getResponse = await _client.GetAsync(request + "Get");
            var all = getResponse.Content.ReadAsStringAsync();
            var allCountries = JsonConvert.DeserializeObject<List<Category>>(all.Result);

            //Assert
            Assert.IsTrue(postResponse.IsSuccessStatusCode);
            Assert.IsTrue(deleteResponse.IsSuccessStatusCode);
            Assert.IsTrue(deleteResponse.StatusCode == HttpStatusCode.NoContent);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual(testCategory.Name, createdCategory.Name);
            Assert.AreNotEqual(Guid.Empty, createdCategory.Id);

            Assert.IsTrue(allCountries.Any(x => x.Id == createdCategory.Id == false));
        }

        [TearDown]
        public async Task DeleteCategory()
        {
            //Cleanup
            if (testCategory != null && testCategory.Id > 0)
            {
                await _client.DeleteAsync(request + testCategory.Id);
                testCategory = null;
            }
        }

    }
}
