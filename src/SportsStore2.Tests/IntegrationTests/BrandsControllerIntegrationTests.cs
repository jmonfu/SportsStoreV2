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
        private Brand _testBrand;
        private Image _testBrandImage;
        private string _request;
        private string _requestImage;

        [SetUp]
        public void Setup()
        {
            var basePath = PlatformServices.Default.Application.ApplicationBasePath;
            var projectPath = Path.GetFullPath(Path.Combine(basePath, "../../../../SportsStore2.Tests"));

            var server = new TestServer(Utils.GetHostBuilder(new string[] { }).UseContentRoot(projectPath).UseEnvironment("Development").UseStartup<Startup>());
            _client = server.CreateClient();

            _request = Enums.GetEnumDescription(Enums.Requests.Brands);
            _requestImage = Enums.GetEnumDescription(Enums.Requests.Images);
            _testBrand = new Brand
            {
                Name = Enums.GetEnumDescription(Enums.BrandTestData.Name)
            };
            _testBrandImage = new Image
            {
                Name = Enums.GetEnumDescription(Enums.ImageBrandTestData.Name),
                Url = Enums.GetEnumDescription(Enums.ImageBrandTestData.Url)
            };
            _testBrand.Image = _testBrandImage;
        }

        [Test]
        public async Task Get_ReturnsAListOfBrands_BrandsController()
        {
            var response = await _client.GetAsync(_request + "Get");
            response.EnsureSuccessStatusCode();

            Assert.IsTrue(true);
        }

        [Test]
        public async Task GetById_GetOneBrand_BrandController()
        {
            //Arrange 

            //Act
            _testBrand = await InsertBrandIfNotAny();

            var getResponseOneBrand = await _client.GetAsync(_request + "Get/" + _testBrand.Id);
            var fetched = await getResponseOneBrand.Content.ReadAsStringAsync();
            var fetchedBrand = JsonConvert.DeserializeObject<Brand>(fetched);

            Assert.IsTrue(getResponseOneBrand.IsSuccessStatusCode);
            Assert.AreEqual(_testBrand.Id, fetchedBrand.Id);
            Assert.AreEqual(_testBrand.Name, fetchedBrand.Name);

        }

        [Test]
        public async Task Create_CreateABrand_NewImage_BrandsController()
        {
            //Arrange 

            //Act
            var postResponseBrand = await _client.PostAsJsonAsync(_request, _testBrand);
            var createdBrand = await postResponseBrand.Content.ReadAsStringAsync();
            var createdBrandObj = JsonConvert.DeserializeObject<Brand>(createdBrand);
            _testBrand.Id = createdBrandObj.Id;
            _testBrand.ImageId = createdBrandObj.Image.Id;
            _testBrandImage.Id = _testBrand.ImageId;

            var getResponse = await _client.GetAsync(_request + "Get/" + createdBrandObj.Id);
            var fetched = await getResponse.Content.ReadAsStringAsync();
            var fetchedBrand = JsonConvert.DeserializeObject<Brand>(fetched);

            // Assert
            Assert.IsTrue(postResponseBrand.IsSuccessStatusCode);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual(_testBrand.Name, createdBrandObj.Name);
            Assert.AreEqual(_testBrand.Name, fetchedBrand.Name);

            Assert.AreNotEqual(Guid.Empty, createdBrandObj.Id);
            Assert.AreEqual(createdBrandObj.Id, fetchedBrand.Id);
        }

        [Test]
        public async Task Create_CreateABrand_ExistingImage_BrandsController()
        {
            //Arrange 
            //create new image
            var createdImage = await InsertImageIfNotAny();

            //Act
            _testBrand.Image = createdImage;
            _testBrand.ImageId = createdImage.Id;
            _testBrandImage.Id = createdImage.Id;

            var postResponseBrand = await _client.PostAsJsonAsync(_request, _testBrand);
            var createdBrand = await postResponseBrand.Content.ReadAsStringAsync();
            var createdBrandObj = JsonConvert.DeserializeObject<Brand>(createdBrand);
            _testBrand.Id = createdBrandObj.Id;

            var getResponse = await _client.GetAsync(_request + "Get/" + createdBrandObj.Id);
            var fetched = await getResponse.Content.ReadAsStringAsync();
            var fetchedBrand = JsonConvert.DeserializeObject<Brand>(fetched);

            // Assert
            Assert.IsTrue(postResponseBrand.IsSuccessStatusCode);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual(_testBrand.Name, createdBrandObj.Name);
            Assert.AreEqual(_testBrand.Name, fetchedBrand.Name);

            Assert.AreNotEqual(Guid.Empty, createdBrandObj.Id);
            Assert.AreEqual(createdBrandObj.Id, fetchedBrand.Id);
        }

        [Test]
        public async Task Update_UpdateABrand_BrandsController()
        {
            //Arrange 

            //Act
            //POST(Crete)
            var postResponse = await _client.PostAsJsonAsync(_request, _testBrand);
            var createdBrand = await postResponse.Content.ReadAsStringAsync();
            var createdBrandObj = JsonConvert.DeserializeObject<Brand>(createdBrand);
            _testBrand.Id = createdBrandObj.Id;
            _testBrand.ImageId = createdBrandObj.Image.Id;
            _testBrandImage.Id = _testBrand.ImageId;

            //PUT(Update)
            _testBrand.Name = Enums.GetEnumDescription(Enums.BrandUpdatedTestData.Name);
            var putResponse = await _client.PutAsJsonAsync(_request + createdBrandObj.Id, _testBrand);

            //GET
            var getResponse = await _client.GetAsync(_request + "Get/" + _testBrand.Id);
            var fetched = await getResponse.Content.ReadAsStringAsync();
            var fetchedBrand = JsonConvert.DeserializeObject<Brand>(fetched);

            _testBrand.Id = fetchedBrand.Id;

            // Assert
            Assert.IsTrue(postResponse.IsSuccessStatusCode);
            Assert.IsTrue(putResponse.IsSuccessStatusCode);
            Assert.IsTrue(putResponse.StatusCode == HttpStatusCode.NoContent);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual(Enums.GetEnumDescription(Enums.BrandTestData.Name), createdBrandObj.Name);
            Assert.AreEqual(Enums.GetEnumDescription(Enums.BrandUpdatedTestData.Name), fetchedBrand.Name);

            Assert.AreNotEqual(Guid.Empty, createdBrandObj.Id);
            Assert.AreEqual(createdBrandObj.Id, fetchedBrand.Id);

        }

        [Test]
        public async Task Delete_DeleteABrand_BrandsController()
        {
            //Arrange 

            //Act
            //POST(Crete)
            var postResponse = await _client.PostAsJsonAsync(_request, _testBrand);
            var createdBrand = await postResponse.Content.ReadAsStringAsync();
            var createdBrandObj = JsonConvert.DeserializeObject<Brand>(createdBrand);
            _testBrand.Id = createdBrandObj.Id;
            _testBrand.ImageId = createdBrandObj.Image.Id;
            _testBrandImage.Id = _testBrand.ImageId;

            //DELETE
            var deleteResponse = await _client.DeleteAsync(_request + createdBrandObj.Id);
            var getResponse = await _client.GetAsync(_request + "Get");
            var all = getResponse.Content.ReadAsStringAsync();
            var allBrands = JsonConvert.DeserializeObject<List<Brand>>(all.Result);

            //Assert
            Assert.IsTrue(postResponse.IsSuccessStatusCode);
            Assert.IsTrue(deleteResponse.IsSuccessStatusCode);
            Assert.IsTrue(deleteResponse.StatusCode == HttpStatusCode.NoContent);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual(_testBrand.Name, createdBrandObj.Name);
            Assert.AreNotEqual(Guid.Empty, createdBrandObj.Id);

            if(allBrands.Count > 0)
                Assert.IsTrue(allBrands.Any(x => x.Id == createdBrandObj.Id == false));

        }

        private async Task<Brand> InsertBrandIfNotAny()
        {
            var getResponse = await _client.GetAsync(_request + "Get");
            var all = getResponse.Content.ReadAsStringAsync();
            var allBrands = JsonConvert.DeserializeObject<List<Brand>>(all.Result);
            if (allBrands.Count > 0)
            {
                _testBrand = allBrands.FirstOrDefault();
            }
            else
            {
                var postResponse = await _client.PostAsJsonAsync(_request, _testBrand);
                var created = await postResponse.Content.ReadAsStringAsync();
                _testBrand = JsonConvert.DeserializeObject<Brand>(created);
            }

            return _testBrand;
        }

        private async Task<Image> InsertImageIfNotAny()
        {
            var getResponse = await _client.GetAsync(_requestImage + "Get");
            var all = getResponse.Content.ReadAsStringAsync();
            var allImages = JsonConvert.DeserializeObject<List<Image>>(all.Result);
            if (allImages.Count > 0)
            {
                _testBrandImage = allImages.FirstOrDefault();
            }
            else
            {
                var postResponse = await _client.PostAsJsonAsync(_requestImage, _testBrand);
                var created = await postResponse.Content.ReadAsStringAsync();
                _testBrandImage = JsonConvert.DeserializeObject<Image>(created);
            }

            return _testBrandImage;
        }

        [TearDown]
        public async Task DeleteImagesAndBrands()
        {
            //Cleanup
            if (_testBrand != null && _testBrand.Id > 0
                && (_testBrand.Name == Enums.GetEnumDescription(Enums.BrandTestData.Name)
                || _testBrand.Name == Enums.GetEnumDescription(Enums.BrandUpdatedTestData.Name)))
            {
                await _client.DeleteAsync(Enums.GetEnumDescription(Enums.Requests.Brands) + _testBrand.Id);
                _testBrand = null;
            }

            if (_testBrandImage != null && _testBrandImage.Id > 0
                && (_testBrandImage.Name == Enums.GetEnumDescription(Enums.ImageBrandTestData.Name)
                || _testBrandImage.Name == Enums.GetEnumDescription(Enums.ImageProductUpdatedTestData.Name)))
            {
                await _client.DeleteAsync(Enums.GetEnumDescription(Enums.Requests.Images) + _testBrandImage.Id);
                _testBrandImage = null;
            }
        }


    }

}
