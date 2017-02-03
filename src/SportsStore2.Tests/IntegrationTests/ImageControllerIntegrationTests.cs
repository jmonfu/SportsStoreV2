using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json;
using NUnit.Framework;
using SportsStore2.API;
using SportsStore2.API.Models;

namespace SportsStore2.Tests.IntegrationTests
{
    [TestFixture]
    public class ImageControllerIntegrationTests
    {
        private HttpClient _client;
        private Image _testImage;
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

            _testImage = new Image
            {
                Name = Enums.GetEnumDescription(Enums.ImageBrandTestData.Name),
                Url = Enums.GetEnumDescription(Enums.ImageBrandTestData.Url)
            };
            _request = Enums.GetEnumDescription(Enums.Requests.Images);
        }

        [Test]
        public async Task Get_ReturnsAListOfImages_ImagesController()
        {
            var response = await _client.GetAsync(_request + "Get");
            response.EnsureSuccessStatusCode();

            Assert.IsTrue(true);
        }

        [Test]
        public async Task GetById_GetOneImage_ImagesController()
        {
            //Arrange 

            _testImage = await InsertIfNotAny();
           
            var getResponseOneImage = await _client.GetAsync(_request + "Get/" + _testImage.Id);
            var fetched = await getResponseOneImage.Content.ReadAsStringAsync();
            var fetchedImage = JsonConvert.DeserializeObject<Image>(fetched);

            Assert.IsTrue(getResponseOneImage.IsSuccessStatusCode);
            Assert.AreEqual(_testImage.Id, fetchedImage.Id);
            Assert.AreEqual(_testImage.Name, fetchedImage.Name);

        }

        private async Task<Image> InsertIfNotAny()
        {
            //Act
            var getResponse = await _client.GetAsync(_request + "Get");
            var all = getResponse.Content.ReadAsStringAsync();
            var allImages = JsonConvert.DeserializeObject<List<Image>>(all.Result);
            if (allImages.Count > 0)
            {
                _testImage = allImages.FirstOrDefault();
            }
            else
            {
                var postResponse = await _client.PostAsJsonAsync(_request, _testImage);
                var created = await postResponse.Content.ReadAsStringAsync();
                _testImage = JsonConvert.DeserializeObject<Image>(created);
            }
            return _testImage;
        }

        [Test]
        public async Task Create_CreateAnImage_ImagesController()
        {
            //Arrange 

            //Act
            var postResponse = await _client.PostAsJsonAsync(_request, _testImage);
            var created = await postResponse.Content.ReadAsStringAsync();
            var createdImage = JsonConvert.DeserializeObject<Image>(created);

            _testImage.Id = createdImage.Id;

            var getResponse = await _client.GetAsync(_request + "Get/" + createdImage.Id);
            var fetched = await getResponse.Content.ReadAsStringAsync();
            var fetchedImage = JsonConvert.DeserializeObject<Image>(fetched);

            // Assert
            Assert.IsTrue(postResponse.IsSuccessStatusCode);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual(_testImage.Name, createdImage.Name);
            Assert.AreEqual(_testImage.Name, fetchedImage.Name);

            Assert.AreNotEqual(Guid.Empty, createdImage.Id);
            Assert.AreEqual(createdImage.Id, fetchedImage.Id);
        }

        [Test]
        public async Task Update_UpdateAnImageViaPUT_ImagesController()
        {
            //Arrange 

            //Act
            //POST(Crete)
            var postResponse = await _client.PostAsJsonAsync(_request, _testImage);
            var created = await postResponse.Content.ReadAsStringAsync();
            var createdImage = JsonConvert.DeserializeObject<Image>(created);

            //PUT(Update)
            _testImage.Id = createdImage.Id;
            _testImage.Name = Enums.GetEnumDescription(Enums.ImageBrandUpdatedTestData.Name);
            _testImage.Url = Enums.GetEnumDescription(Enums.ImageBrandUpdatedTestData.Url);
            var putResponse = await _client.PutAsJsonAsync(_request + createdImage.Id, _testImage);

            //GET
            var getResponse = await _client.GetAsync(_request + "Get/" + _testImage.Id);
            var fetched = await getResponse.Content.ReadAsStringAsync();
            var fetchedImage = JsonConvert.DeserializeObject<Image>(fetched);

            _testImage.Id = fetchedImage.Id;

            // Assert
            Assert.IsTrue(postResponse.IsSuccessStatusCode);
            Assert.IsTrue(putResponse.IsSuccessStatusCode);
            Assert.IsTrue(putResponse.StatusCode == HttpStatusCode.NoContent);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual(Enums.GetEnumDescription(Enums.ImageBrandTestData.Name), createdImage.Name);
            Assert.AreEqual(Enums.GetEnumDescription(Enums.ImageBrandUpdatedTestData.Name), fetchedImage.Name);

            Assert.AreEqual(Enums.GetEnumDescription(Enums.ImageBrandTestData.Url), createdImage.Url);
            Assert.AreEqual(Enums.GetEnumDescription(Enums.ImageBrandUpdatedTestData.Url), fetchedImage.Url);

            Assert.AreNotEqual(Guid.Empty, createdImage.Id);
            Assert.AreEqual(createdImage.Id, fetchedImage.Id);

        }

        [Test]
        public async Task Delete_DeleteAnImage_ImagesController()
        {
            //Arrange 

            //Act
            //POST(Crete)
            var postResponse = await _client.PostAsJsonAsync(_request, _testImage);
            var created = await postResponse.Content.ReadAsStringAsync();
            var createdImage = JsonConvert.DeserializeObject<Image>(created);

            //DELETE
            var deleteResponse = await _client.DeleteAsync(_request + createdImage.Id);
            var getResponse = await _client.GetAsync(_request + "Get");
            var all = getResponse.Content.ReadAsStringAsync();
            var allImages = JsonConvert.DeserializeObject<List<Image>>(all.Result);

            //Assert
            Assert.IsTrue(postResponse.IsSuccessStatusCode);
            Assert.IsTrue(deleteResponse.IsSuccessStatusCode);
            Assert.IsTrue(deleteResponse.StatusCode == HttpStatusCode.NoContent);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual(_testImage.Name, createdImage.Name);
            Assert.AreNotEqual(Guid.Empty, createdImage.Id);

            Assert.IsTrue(allImages.Any(x => x.Id == createdImage.Id == false));
        }


        [TearDown]
        public async Task DeleteImages()
        {
            //Cleanup

            if (_testImage != null && _testImage.Id > 0
                && (_testImage.Name == Enums.GetEnumDescription(Enums.ImageBrandTestData.Name)
                || _testImage.Name == Enums.GetEnumDescription(Enums.ImageBrandUpdatedTestData.Name)))
            {
                await _client.DeleteAsync(_request + _testImage.Id);
                _testImage = null;
            }

        }
    }

}
