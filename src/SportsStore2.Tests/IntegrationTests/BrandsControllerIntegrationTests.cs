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
    [TestFixture()]
    public class BrandsControllerIntegrationTests
    {
        private HttpClient _client;
        private Brand brand;
        private Image image;

        [SetUp]
        public void Setup()
        {
            var basePath = PlatformServices.Default.Application.ApplicationBasePath;
            var projectPath = Path.GetFullPath(Path.Combine(basePath, "../../../../SportsStore2.Tests"));

            var server = new TestServer(Utils.GetHostBuilder(new string[] { }).UseContentRoot(projectPath).UseEnvironment("Development").UseStartup<Startup>());
            _client = server.CreateClient();

        }

        [Test]
        public async Task Get_ReturnsAListOfBrands_BrandsController()
        {
            var request = "api/Brands/Get";
            var response = await _client.GetAsync(request);
            response.EnsureSuccessStatusCode();

            Assert.IsTrue(true);
        }

        [Test]
        public async Task GetById_GetOneBrand_BrandController()
        {
            //Arrange 
            var request = "api/Brands";
            Brand selectedBrand = null;

            //Act
            var getResponse = await _client.GetAsync(request + "/Get");
            var all = getResponse.Content.ReadAsStringAsync();
            var allBrands = JsonConvert.DeserializeObject<List<Brand>>(all.Result);
            if (allBrands.Count > 0)
            {
                selectedBrand = allBrands.FirstOrDefault();
            }
            else
            {
                brand = new Brand { Name = "testBrand" };
                var postResponse = await _client.PostAsJsonAsync(request, brand);
                var created = await postResponse.Content.ReadAsStringAsync();
                selectedBrand = JsonConvert.DeserializeObject<Brand>(created);

                brand.Id = selectedBrand.Id;
            }

            var getResponseOneBrand = await _client.GetAsync(request + "/Get/" + selectedBrand.Id);
            var fetched = await getResponseOneBrand.Content.ReadAsStringAsync();
            var fetchedBrand = JsonConvert.DeserializeObject<Brand>(fetched);

            Assert.IsTrue(getResponse.IsSuccessStatusCode);
            Assert.IsTrue(getResponseOneBrand.IsSuccessStatusCode);
            Assert.AreEqual(selectedBrand.Id, fetchedBrand.Id);
            Assert.AreEqual(selectedBrand.Name, fetchedBrand.Name);

        }

        [Test]
        public async Task Create_CreateABrand_BrandsController()
        {
            //Arrange 
            var requestBrand = "api/Brands/";
            brand = new Brand { Name = "testBrand" };
            image = new Image { Name = "testImage", Url = "/Brands/adidas_logo_test.png" };
            brand.Image = image;

            //Act
            var postResponseBrand = await _client.PostAsJsonAsync(requestBrand, brand);
            var createdBrand = await postResponseBrand.Content.ReadAsStringAsync();
            var createdBrandObj = JsonConvert.DeserializeObject<Brand>(createdBrand);
            brand.Id = createdBrandObj.Id;
            brand.ImageId = createdBrandObj.Image.Id;
            image.Id = brand.ImageId;

            var getResponse = await _client.GetAsync(requestBrand + "Get/" + createdBrandObj.Id);
            var fetched = await getResponse.Content.ReadAsStringAsync();
            var fetchedBrand = JsonConvert.DeserializeObject<Brand>(fetched);

            // Assert
            Assert.IsTrue(postResponseBrand.IsSuccessStatusCode);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual(brand.Name, createdBrandObj.Name);
            Assert.AreEqual(brand.Name, fetchedBrand.Name);

            Assert.AreNotEqual(Guid.Empty, createdBrandObj.Id);
            Assert.AreEqual(createdBrandObj.Id, fetchedBrand.Id);
        }

        [Test]
        public async Task Update_UpdateABrand_BrandsController()
        {
            //Arrange 
            var request = "api/Brands/";
            brand = new Brand { Name = "testBrand" };
            image = new Image { Name = "testImage", Url = "/Brands/adidas_logo_test.png" };
            brand.Image = image;

            //Act
            //POST(Crete)
            var postResponse = await _client.PostAsJsonAsync(request, brand);
            var createdBrand = await postResponse.Content.ReadAsStringAsync();
            var createdBrandObj = JsonConvert.DeserializeObject<Brand>(createdBrand);
            brand.Id = createdBrandObj.Id;
            brand.ImageId = createdBrandObj.Image.Id;
            image.Id = brand.ImageId;

            //PUT(Update)
            brand.Id = brand.Id;
            brand.Name = "testBrandUpdated";
            var putResponse = await _client.PutAsJsonAsync(request + createdBrandObj.Id, brand);

            //GET
            var getResponse = await _client.GetAsync(request + "Get/" + brand.Id);
            var fetched = await getResponse.Content.ReadAsStringAsync();
            var fetchedBrand = JsonConvert.DeserializeObject<Brand>(fetched);

            brand.Id = fetchedBrand.Id;

            // Assert
            Assert.IsTrue(postResponse.IsSuccessStatusCode);
            Assert.IsTrue(putResponse.IsSuccessStatusCode);
            Assert.IsTrue(putResponse.StatusCode == HttpStatusCode.NoContent);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual("testBrand", createdBrandObj.Name);
            Assert.AreEqual("testBrandUpdated", fetchedBrand.Name);

            Assert.AreNotEqual(Guid.Empty, createdBrandObj.Id);
            Assert.AreEqual(createdBrandObj.Id, fetchedBrand.Id);

        }

        [Test]
        public async Task Delete_DeleteABrand_BrandsController()
        {
            //Arrange 
            var request = "api/Brands/";
            brand = new Brand { Name = "testBrand" };
            image = new Image { Name = "testImage", Url = "/Brands/adidas_logo_test.png" };
            brand.Image = image;

            //Act
            //POST(Crete)
            var postResponse = await _client.PostAsJsonAsync(request, brand);
            var createdBrand = await postResponse.Content.ReadAsStringAsync();
            var createdBrandObj = JsonConvert.DeserializeObject<Brand>(createdBrand);
            brand.Id = createdBrandObj.Id;
            brand.ImageId = createdBrandObj.Image.Id;
            image.Id = brand.ImageId;

            //DELETE
            var deleteResponse = await _client.DeleteAsync(request + createdBrandObj.Id);
            var getResponse = await _client.GetAsync(request + "Get");
            var all = getResponse.Content.ReadAsStringAsync();
            var allBrands = JsonConvert.DeserializeObject<List<Brand>>(all.Result);

            //Assert
            Assert.IsTrue(postResponse.IsSuccessStatusCode);
            Assert.IsTrue(deleteResponse.IsSuccessStatusCode);
            Assert.IsTrue(deleteResponse.StatusCode == HttpStatusCode.NoContent);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual(brand.Name, createdBrandObj.Name);
            Assert.AreNotEqual(Guid.Empty, createdBrandObj.Id);

            Assert.IsTrue(allBrands.Any(x => x.Id == createdBrandObj.Id == false));

        }

        [TearDown]
        public async Task DeleteImages()
        {
            //Cleanup
            var requestImage = "api/Images/";
            var requestBrand = "api/Brands/";

            if (brand != null && brand.Id > 0)
            {
                await _client.DeleteAsync(requestBrand + brand.Id);
                brand = null;
            }

            if (image != null && image.Id > 0)
            {
                await _client.DeleteAsync(requestImage + image.Id);
                image = null;
            }
        }


    }

}
