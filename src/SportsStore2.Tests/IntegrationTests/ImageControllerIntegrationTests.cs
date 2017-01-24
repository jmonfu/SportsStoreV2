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
    public class ImageControllerIntegrationTests
    {
        private HttpClient _client;
        private Image image;

        [SetUp]
        public void Setup()
        {
            var basePath = PlatformServices.Default.Application.ApplicationBasePath;
            var projectPath = Path.GetFullPath(Path.Combine(basePath, "../../../../SportsStore2.Tests"));

            var server = new TestServer(GetHostBuilder(new string[] { })
                .UseContentRoot(projectPath)
                .UseEnvironment("Development")
                .UseStartup<Startup>());

            _client = server.CreateClient();

        }

        private static IWebHostBuilder GetHostBuilder(string[] args)
        {
            var config = new ConfigurationBuilder()
                       .AddCommandLine(args)
                       .AddEnvironmentVariables(prefix: "ASPNETCORE_")
                       .Build();

            return new WebHostBuilder()
                .UseConfiguration(config)
                .UseKestrel()
                .UseStartup<Startup>();
        }

        [Test]
        public async Task Get_ReturnsAListOfImages_ImagesController()
        {
            var request = "api/Images/Get";
            var response = await _client.GetAsync(request);
            response.EnsureSuccessStatusCode();

            Assert.IsTrue(true);
        }

        [Test]
        public async Task GetById_GetOneImage_ImagesController()
        {
            //Arrange 
            var request = "api/Images";

            //Act
            var getResponse = await _client.GetAsync(request + "/Get");
            var all = getResponse.Content.ReadAsStringAsync();
            var allImages = JsonConvert.DeserializeObject<List<Image>>(all.Result);
            var selectedImage = allImages.FirstOrDefault();

            var getResponseOneImage = await _client.GetAsync(request + "/Get/" + selectedImage.Id);
            var fetched = await getResponseOneImage.Content.ReadAsStringAsync();
            var fetchedImage = JsonConvert.DeserializeObject<Image>(fetched);

            Assert.IsTrue(getResponse.IsSuccessStatusCode);
            Assert.IsTrue(getResponseOneImage.IsSuccessStatusCode);
            Assert.AreEqual(selectedImage.Id, fetchedImage.Id);
            Assert.AreEqual(selectedImage.Name, fetchedImage.Name);

        }

        [Test]
        public async Task Create_CreateAnImage_ImagesController()
        {
            //Arrange 
            var request = "api/Images";
            image = new Image { Name = "testImageCreate", Url = "/Brands/adidas_logo_test.png" };

            //Act
            var postResponse = await _client.PostAsJsonAsync(request, image);
            var created = await postResponse.Content.ReadAsStringAsync();
            var createdImage = JsonConvert.DeserializeObject<Image>(created);

            image.Id = createdImage.Id;

            var getResponse = await _client.GetAsync(request + "/Get/" + createdImage.Id);
            var fetched = await getResponse.Content.ReadAsStringAsync();
            var fetchedImage = JsonConvert.DeserializeObject<Image>(fetched);

            // Assert
            Assert.IsTrue(postResponse.IsSuccessStatusCode);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual(image.Name, createdImage.Name);
            Assert.AreEqual(image.Name, fetchedImage.Name);

            Assert.AreNotEqual(Guid.Empty, createdImage.Id);
            Assert.AreEqual(createdImage.Id, fetchedImage.Id);
        }

        [Test]
        public async Task Update_UpdateAnImageViaPUT_ImagesController()
        {
            //Arrange 
            var request = "api/Images/";
            image = new Image { Name = "testImageUpdate", Url = "/Brands/adidas_logo_test.png" };

            //Act
            //POST(Crete)
            var postResponse = await _client.PostAsJsonAsync(request, image);
            var created = await postResponse.Content.ReadAsStringAsync();
            var createdImage = JsonConvert.DeserializeObject<Image>(created);

            //PUT(Update)
            image.Id = createdImage.Id;
            image.Name = "testImageUpdated";
            image.Url = "/Brands/adidas_logo_test_Updated.png";
            var putResponse = await _client.PutAsJsonAsync(request + createdImage.Id, image);

            //GET
            var getResponse = await _client.GetAsync(request + "Get/" + image.Id);
            var fetched = await getResponse.Content.ReadAsStringAsync();
            var fetchedImage = JsonConvert.DeserializeObject<Image>(fetched);

            image.Id = fetchedImage.Id;

            // Assert
            Assert.IsTrue(postResponse.IsSuccessStatusCode);
            Assert.IsTrue(putResponse.IsSuccessStatusCode);
            Assert.IsTrue(putResponse.StatusCode == HttpStatusCode.NoContent);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual("testImageUpdate", createdImage.Name);
            Assert.AreEqual("testImageUpdated", fetchedImage.Name);

            Assert.AreEqual("/Brands/adidas_logo_test.png", createdImage.Url);
            Assert.AreEqual("/Brands/adidas_logo_test_Updated.png", fetchedImage.Url);

            Assert.AreNotEqual(Guid.Empty, createdImage.Id);
            Assert.AreEqual(createdImage.Id, fetchedImage.Id);

        }

        [Test]
        public async Task Delete_DeleteAnImage_ImagesController()
        {
            //Arrange 
            var request = "api/Images/";
            var image = new Image { Name = "testImageDel", Url = "/Brands/adidas_logo_test.png" };

            //Act
            //POST(Crete)
            var postResponse = await _client.PostAsJsonAsync(request, image);
            var created = await postResponse.Content.ReadAsStringAsync();
            var createdImage = JsonConvert.DeserializeObject<Image>(created);

            //DELETE
            var deleteResponse = await _client.DeleteAsync(request + createdImage.Id);
            var getResponse = await _client.GetAsync(request + "Get");
            var all = getResponse.Content.ReadAsStringAsync();
            var allImages = JsonConvert.DeserializeObject<List<Image>>(all.Result);

            //Assert
            Assert.IsTrue(postResponse.IsSuccessStatusCode);
            Assert.IsTrue(deleteResponse.IsSuccessStatusCode);
            Assert.IsTrue(deleteResponse.StatusCode == HttpStatusCode.NoContent);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual(image.Name, createdImage.Name);
            Assert.AreNotEqual(Guid.Empty, createdImage.Id);

            Assert.IsTrue(allImages.Any(x => x.Id == createdImage.Id == false));
        }

        [TearDown]
        public async Task DeleteImages()
        {
            //Cleanup
            var request = "api/Images/";

            if (image != null && image.Id > 0)
            {
                await _client.DeleteAsync(request + image.Id);
                image = null;
            }

        }
    }

}
