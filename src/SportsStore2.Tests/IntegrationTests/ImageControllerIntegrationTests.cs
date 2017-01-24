﻿using System;
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
        private Image testImage;
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

            testImage = new Image { Name = "testImage", Url = "/Brands/adidas_logo_test.png" };
            request = "api/Images/";
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
            Image selectedImage = null;
            
            //Act
            var getResponse = await _client.GetAsync(request + "Get");
            var all = getResponse.Content.ReadAsStringAsync();
            var allImages = JsonConvert.DeserializeObject<List<Image>>(all.Result);
            if (allImages.Count > 0)
            {
                selectedImage = allImages.FirstOrDefault();
            }
            else
            {
                var postResponse = await _client.PostAsJsonAsync(request, testImage);
                var created = await postResponse.Content.ReadAsStringAsync();
                selectedImage = JsonConvert.DeserializeObject<Image>(created);

                testImage.Id = selectedImage.Id;
            }

            var getResponseOneImage = await _client.GetAsync(request + "Get/" + selectedImage.Id);
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

            //Act
            var postResponse = await _client.PostAsJsonAsync(request, testImage);
            var created = await postResponse.Content.ReadAsStringAsync();
            var createdImage = JsonConvert.DeserializeObject<Image>(created);

            testImage.Id = createdImage.Id;

            var getResponse = await _client.GetAsync(request + "Get/" + createdImage.Id);
            var fetched = await getResponse.Content.ReadAsStringAsync();
            var fetchedImage = JsonConvert.DeserializeObject<Image>(fetched);

            // Assert
            Assert.IsTrue(postResponse.IsSuccessStatusCode);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual(testImage.Name, createdImage.Name);
            Assert.AreEqual(testImage.Name, fetchedImage.Name);

            Assert.AreNotEqual(Guid.Empty, createdImage.Id);
            Assert.AreEqual(createdImage.Id, fetchedImage.Id);
        }

        [Test]
        public async Task Update_UpdateAnImageViaPUT_ImagesController()
        {
            //Arrange 

            //Act
            //POST(Crete)
            var postResponse = await _client.PostAsJsonAsync(request, testImage);
            var created = await postResponse.Content.ReadAsStringAsync();
            var createdImage = JsonConvert.DeserializeObject<Image>(created);

            //PUT(Update)
            testImage.Id = createdImage.Id;
            testImage.Name = "testImageUpdated";
            testImage.Url = "/Brands/adidas_logo_test_Updated.png";
            var putResponse = await _client.PutAsJsonAsync(request + createdImage.Id, testImage);

            //GET
            var getResponse = await _client.GetAsync(request + "Get/" + testImage.Id);
            var fetched = await getResponse.Content.ReadAsStringAsync();
            var fetchedImage = JsonConvert.DeserializeObject<Image>(fetched);

            testImage.Id = fetchedImage.Id;

            // Assert
            Assert.IsTrue(postResponse.IsSuccessStatusCode);
            Assert.IsTrue(putResponse.IsSuccessStatusCode);
            Assert.IsTrue(putResponse.StatusCode == HttpStatusCode.NoContent);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual("testImage", createdImage.Name);
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

            //Act
            //POST(Crete)
            var postResponse = await _client.PostAsJsonAsync(request, testImage);
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

            Assert.AreEqual(testImage.Name, createdImage.Name);
            Assert.AreNotEqual(Guid.Empty, createdImage.Id);

            Assert.IsTrue(allImages.Any(x => x.Id == createdImage.Id == false));
        }

        [TearDown]
        public async Task DeleteImages()
        {
            //Cleanup

            if (testImage != null && testImage.Id > 0)
            {
                await _client.DeleteAsync(request + testImage.Id);
                testImage = null;
            }

        }
    }

}
