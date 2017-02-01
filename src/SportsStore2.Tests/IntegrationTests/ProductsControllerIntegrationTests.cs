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
    [TestFixture()]
    public class ProductsControllerIntegrationTests
    {
        private HttpClient _client;
        private Brand testBrand;
        private Image testImageBrand;
        private Image testImageProduct;
        private Category testCategory;
        private Product testProduct;
        private string requestProduct;
        private string requestBrand;
        private string requestImage;
        private string requestCategory;


        [SetUp]
        public void Setup()
        {
            var basePath = PlatformServices.Default.Application.ApplicationBasePath;
            var projectPath = Path.GetFullPath(Path.Combine(basePath, "../../../../SportsStore2.Tests"));

            var server = new TestServer(Utils.GetHostBuilder(new string[] { }).UseContentRoot(projectPath).UseEnvironment("Development").UseStartup<Startup>());
            _client = server.CreateClient();

            requestProduct = "api/Products/";
            requestBrand = "api/Brands/";
            requestImage = "api/Images/";
            requestCategory = "api/Categories/";

            testImageBrand = new Image { Name = "testImageBrand", Url = "/Brands/adidas_logo_test.png" };
            testImageProduct = new Image { Name = "testImageProduct", Url = "/Brands/adidas_logo_test.png" };

            testBrand = new Brand
            {
                Name = "testBrand",
                Image = testImageBrand,
                ImageId = testImageBrand.Id
            };

            testCategory = new Category { Name = "testCategory" };

            testProduct = new Product
            {
                Category = testCategory,
                Name = "testProduct",
                Brand = testBrand,
                BrandId = testBrand.Id,
                CategoryId = testCategory.Id,
                Deal = false,
                Description = "testDescription",
                Discount = "50% Discount",
                Price = new decimal(50.00),
                Image = testImageProduct,
                ImageId = testImageProduct.Id,
                Stock = 5
            };
        }

        [Test]
        public async Task Get_ReturnsAListOfProducts_ProductsController()
        {
            var response = await _client.GetAsync(requestProduct + "Get");
            response.EnsureSuccessStatusCode();

            Assert.IsTrue(true);
        }

        [Test]
        public async Task GetById_GetOneProduct_ProductsController()
        {
            //Arrange 
            Product selectedProduct = null;

            //Act
            var getResponse = await _client.GetAsync(requestProduct + "Get");
            var all = getResponse.Content.ReadAsStringAsync();
            var allProducts = JsonConvert.DeserializeObject<List<Product>>(all.Result);
            if (allProducts.Count > 0)
            {
                selectedProduct = allProducts.FirstOrDefault();
            }
            else
            {
                //TODO edit the insert procedure below
                var postResponse = await _client.PostAsJsonAsync(requestProduct, testProduct);
                var created = await postResponse.Content.ReadAsStringAsync();
                selectedProduct = JsonConvert.DeserializeObject<Product>(created);

                testProduct.Id = selectedProduct.Id;
            }

            var getResponseOneProduct = await _client.GetAsync(requestProduct + "Get/" + selectedProduct.Id);
            var fetched = await getResponseOneProduct.Content.ReadAsStringAsync();
            var fetchedProduct = JsonConvert.DeserializeObject<Product>(fetched);

            Assert.IsTrue(getResponse.IsSuccessStatusCode);
            Assert.IsTrue(getResponseOneProduct.IsSuccessStatusCode);
            Assert.AreEqual(selectedProduct.Id, fetchedProduct.Id);
            Assert.AreEqual(fetchedProduct.Name, fetchedProduct.Name);
        }

        [Test]
        public async Task Create_CreateAProduct_NewBrandNewCategoryNewImage_ProductsController()
        {
            //Arrange 

            //Act
            var postResponseProduct = await _client.PostAsJsonAsync(requestProduct, testProduct);
            var createdProduct = await postResponseProduct.Content.ReadAsStringAsync();
            var createdProductObj = JsonConvert.DeserializeObject<Product>(createdProduct);

            //for cleanup
            testBrand.Id = createdProductObj.BrandId;
            testImageBrand.Id = createdProductObj.Brand.ImageId;
            testCategory.Id = createdProductObj.CategoryId;
            testProduct.Id = createdProductObj.Id;
            testImageProduct.Id = createdProductObj.ImageId;

            var getResponse = await _client.GetAsync(requestProduct + "Get/" + createdProductObj.Id);
            var fetched = await getResponse.Content.ReadAsStringAsync();
            var fetchedProduct = JsonConvert.DeserializeObject<Product>(fetched);

            // Assert
            Assert.IsTrue(postResponseProduct.IsSuccessStatusCode);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual(testProduct.Name, createdProductObj.Name);
            Assert.AreEqual(testProduct.Name, fetchedProduct.Name);

            Assert.AreNotEqual(Guid.Empty, createdProductObj.Id);
            Assert.AreEqual(createdProductObj.Id, fetchedProduct.Id);
        }

        [Test]
        public async Task Create_CreateAProduct_NewBrandNewCategoryNewImageProductsExisitingImage_ProductsController()
        {
            //Arrange 
            var requestImage = "api/Images/";
            //we have to assign the testBrandImage Id to the Brand
            //check if there is an existing image in the Images Table, if not create one.
            var getResponseImage = await _client.GetAsync(requestImage + "Get");
            var all = getResponseImage.Content.ReadAsStringAsync();
            var allImages = JsonConvert.DeserializeObject<List<Image>>(all.Result);

            Image selectedImage;

            if (allImages.Any())
            {
                selectedImage = allImages.FirstOrDefault();
            }
            else
            {
                //create new image
                var postResponseBrandImage = await _client.PostAsJsonAsync(requestImage, testImageBrand);
                var createdBrandImage = await postResponseBrandImage.Content.ReadAsStringAsync();
                selectedImage = JsonConvert.DeserializeObject<Image>(createdBrandImage);
            }

            testBrand.Image = selectedImage;
            testBrand.ImageId = selectedImage.Id;
            testImageBrand = selectedImage;

            //create new image
            var postResponseImage = await _client.PostAsJsonAsync(requestImage, testImageProduct);
            var created = await postResponseImage.Content.ReadAsStringAsync();
            var createdImage = JsonConvert.DeserializeObject<Image>(created);

            //Act
            testProduct.Image = createdImage;
            testProduct.ImageId = createdImage.Id;
            testImageProduct.Id = createdImage.Id;

            var postResponseProduct = await _client.PostAsJsonAsync(requestProduct, testProduct);
            var createdProduct = await postResponseProduct.Content.ReadAsStringAsync();
            var createdProductObj = JsonConvert.DeserializeObject<Product>(createdProduct);

            //for cleanup
            testBrand.Id = createdProductObj.BrandId;
            testImageProduct.Id = createdProductObj.ImageId;
            testCategory.Id = createdProductObj.CategoryId;
            testProduct.Id = createdProductObj.Id;

            var getResponse = await _client.GetAsync(requestProduct + "Get/" + createdProductObj.Id);
            var fetched = await getResponse.Content.ReadAsStringAsync();
            var fetchedProduct = JsonConvert.DeserializeObject<Product>(fetched);

            // Assert
            Assert.IsTrue(postResponseProduct.IsSuccessStatusCode);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual(testProduct.Name, createdProductObj.Name);
            Assert.AreEqual(testProduct.Name, fetchedProduct.Name);

            Assert.AreNotEqual(Guid.Empty, createdProductObj.Id);
            Assert.AreEqual(createdProductObj.Id, fetchedProduct.Id);
        }

        [Test]
        public async Task Create_CreateAProduct_NewBrandNewCategoryExistingImageProductsNewImage_ProductsController()
        {
            //Arrange 
            var requestImage = "api/Images/";
            //we have to assign the testBrandImage Id to the Brand
            //check if there is an existing image in the Images Table, if not create one.
            var getResponseImage = await _client.GetAsync(requestImage + "Get");
            var all = getResponseImage.Content.ReadAsStringAsync();
            var allImages = JsonConvert.DeserializeObject<List<Image>>(all.Result);

            Image selectedImage;

            if (allImages.Any())
            {
                selectedImage = allImages.FirstOrDefault();
            }
            else
            {
                //create new image (exisiting image for this product)
                var postResponseProductImage = await _client.PostAsJsonAsync(requestImage, testImageProduct);
                var createdProductImage = await postResponseProductImage.Content.ReadAsStringAsync();
                selectedImage = JsonConvert.DeserializeObject<Image>(createdProductImage);
            }

            testProduct.Image = selectedImage;
            testProduct.ImageId = selectedImage.Id;
            testImageProduct = selectedImage;

            //create a new brand image
            var postResponseImage = await _client.PostAsJsonAsync(requestImage, testImageBrand);
            var created = await postResponseImage.Content.ReadAsStringAsync();
            var createdImage = JsonConvert.DeserializeObject<Image>(created);

            //Act
            testBrand.Image = createdImage;
            testBrand.ImageId = createdImage.Id;
            testImageBrand.Id = createdImage.Id;

            var postResponseProduct = await _client.PostAsJsonAsync(requestProduct, testProduct);
            var createdProduct = await postResponseProduct.Content.ReadAsStringAsync();
            var createdProductObj = JsonConvert.DeserializeObject<Product>(createdProduct);

            //for cleanup
            testBrand.Id = createdProductObj.BrandId;
            testImageProduct.Id = createdProductObj.ImageId;
            testCategory.Id = createdProductObj.CategoryId;
            testProduct.Id = createdProductObj.Id;

            var getResponse = await _client.GetAsync(requestProduct + "Get/" + createdProductObj.Id);
            var fetched = await getResponse.Content.ReadAsStringAsync();
            var fetchedProduct = JsonConvert.DeserializeObject<Product>(fetched);

            // Assert
            Assert.IsTrue(postResponseProduct.IsSuccessStatusCode);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual(testProduct.Name, createdProductObj.Name);
            Assert.AreEqual(testProduct.Name, fetchedProduct.Name);

            Assert.AreNotEqual(Guid.Empty, createdProductObj.Id);
            Assert.AreEqual(createdProductObj.Id, fetchedProduct.Id);
        }

        [Test]
        public async Task Update_UpdateAProductViaPUT_ProductsController()
        {
            //Arrange
            //POST(Crete)
            var postResponse = await _client.PostAsJsonAsync(requestProduct, testProduct);
            var created = await postResponse.Content.ReadAsStringAsync();
            var createdProduct = JsonConvert.DeserializeObject<Product>(created);

            //get the image for the created product 
            var selectedImage = createdProduct.Image;

            //get a brand for the product to update to
            var selectedBrand = createdProduct.Brand;

            //get a category for the product to update to
            var selectedCategory = createdProduct.Category;

            var updatedProduct = new Product
            {
                Id = createdProduct.Id,
                Brand = selectedBrand,
                Category = selectedCategory,
                BrandId = selectedBrand.Id,
                CategoryId = selectedCategory.Id,
                Deal = true,
                Description = "test Description updated",
                Discount = "10%",
                Image = selectedImage,
                ImageId = selectedImage.Id,
                Name = "test Product Updated",
                Price = new decimal(20.00),
                Stock = 100
            };

            var putResponse = await _client.PutAsJsonAsync(requestProduct + createdProduct.Id, updatedProduct);

            //GET
            var getResponse = await _client.GetAsync(requestProduct + "Get/" + updatedProduct.Id);
            var fetched = await getResponse.Content.ReadAsStringAsync();
            var fetchedProduct = JsonConvert.DeserializeObject<Product>(fetched);

            //for cleanup
            testBrand = updatedProduct.Brand;
            testImageProduct = updatedProduct.Image;
            testImageBrand = updatedProduct.Brand.Image;
            testCategory = updatedProduct.Category;
            testProduct = updatedProduct;

            // Assert
            Assert.IsTrue(postResponse.IsSuccessStatusCode);
            Assert.IsTrue(putResponse.IsSuccessStatusCode);
            Assert.IsTrue(putResponse.StatusCode == HttpStatusCode.NoContent);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual("testProduct", createdProduct.Name);
            Assert.AreEqual("test Product Updated", fetchedProduct.Name);
            Assert.AreEqual("testDescription", createdProduct.Description);
            Assert.AreEqual("test Description updated", fetchedProduct.Description);
            Assert.AreEqual("50% Discount", createdProduct.Discount);
            Assert.AreEqual("10%", fetchedProduct.Discount);
            Assert.AreEqual("50% Discount", createdProduct.Discount);
            Assert.AreEqual("10%", fetchedProduct.Discount);

            Assert.AreNotEqual(Guid.Empty, createdProduct.Id);
            Assert.AreEqual(createdProduct.Id, fetchedProduct.Id);

        }

        [Test]
        public async Task Delete_DeleteAProduct_ProductsController()
        {
            //Arrange 

            //Act
            //POST(Crete)
            var postResponse = await _client.PostAsJsonAsync(requestProduct, testProduct);
            var created = await postResponse.Content.ReadAsStringAsync();
            var createdProduct = JsonConvert.DeserializeObject<Product>(created);

            //DELETE
            var deleteResponse = await _client.DeleteAsync(requestProduct + createdProduct.Id);
            var getResponse = await _client.GetAsync(requestProduct + "Get");
            var all = getResponse.Content.ReadAsStringAsync();
            var allProducts = JsonConvert.DeserializeObject<List<Product>>(all.Result);

            //for cleanup
            testBrand = createdProduct.Brand;
            testImageBrand = createdProduct.Brand.Image;
            testImageProduct = createdProduct.Image;
            testCategory = createdProduct.Category;

            //Assert
            Assert.IsTrue(postResponse.IsSuccessStatusCode);
            Assert.IsTrue(deleteResponse.IsSuccessStatusCode);
            Assert.IsTrue(deleteResponse.StatusCode == HttpStatusCode.NoContent);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual(testProduct.Name, createdProduct.Name);
            Assert.AreNotEqual(Guid.Empty, createdProduct.Id);

            Assert.IsTrue(allProducts.Any(x => x.Id == createdProduct.Id == false));
        }


        [TearDown]
        public async Task DeleteImagesAndBrands()
        {
            //Cleanup
            if (testProduct != null && testProduct.Id > 0)
            {
                await _client.DeleteAsync(requestProduct + testProduct.Id);
                testProduct = null;
            }

            if (testBrand != null && testBrand.Id > 0)
            {
                await _client.DeleteAsync(requestBrand + testBrand.Id);
                testBrand = null;
            }

            if (testImageBrand != null && testImageBrand.Id > 0)
            {
                await _client.DeleteAsync(requestImage + testImageBrand.Id);
                testImageBrand = null;
            }

            if (testImageProduct != null && testImageProduct.Id > 0)
            {
                await _client.DeleteAsync(requestImage + testImageProduct.Id);
                testImageProduct = null;
            }

            if (testCategory != null && testCategory.Id > 0)
            {
                await _client.DeleteAsync(requestCategory + testCategory.Id);
                testCategory = null;
            }

        }

    }

}
