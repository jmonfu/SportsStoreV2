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
        private Brand _testBrand;
        private Image _testImageBrand;
        private Image _testImageProduct;
        private Category _testCategory;
        private Product _testProduct;
        private string _requestProduct;
        private string _requestBrand;
        private string _requestImage;
        private string _requestCategory;


        [SetUp]
        public void Setup()
        {
            var basePath = PlatformServices.Default.Application.ApplicationBasePath;
            var projectPath = Path.GetFullPath(Path.Combine(basePath, "../../../../SportsStore2.Tests"));

            var server = new TestServer(Utils.GetHostBuilder(new string[] { }).UseContentRoot(projectPath).UseEnvironment("Development").UseStartup<Startup>());
            _client = server.CreateClient();

            _requestProduct = Enums.GetEnumDescription(Enums.Requests.Products);
            _requestBrand = Enums.GetEnumDescription(Enums.Requests.Brands);
            _requestImage = Enums.GetEnumDescription(Enums.Requests.Images);
            _requestCategory = Enums.GetEnumDescription(Enums.Requests.Categories);

            _testImageBrand = new Image
            {
                Name = Enums.GetEnumDescription(Enums.ImageBrandTestData.Name),
                Url = Enums.GetEnumDescription(Enums.ImageBrandTestData.Url)
            };
            _testImageProduct = new Image
            {
                Name = Enums.GetEnumDescription(Enums.ImageProductTestData.Name),
                Url = Enums.GetEnumDescription(Enums.ImageProductTestData.Url)
            };

            _testBrand = new Brand
            {
                Name = Enums.GetEnumDescription(Enums.BrandTestData.Name),
                Image = _testImageBrand,
                ImageId = _testImageBrand.Id
            };

            _testCategory = new Category
            {
                Name = Enums.GetEnumDescription(Enums.CategoryTestData.Name)
            };

            _testProduct = new Product
            {
                Category = _testCategory,
                Name = Enums.GetEnumDescription(Enums.ProductTestData.Name),
                Brand = _testBrand,
                BrandId = _testBrand.Id,
                CategoryId = _testCategory.Id,
                Deal = Convert.ToBoolean(Enums.GetEnumDescription(Enums.ProductTestData.Deal)) ,
                Description = Enums.GetEnumDescription(Enums.ProductTestData.Description),
                Discount = Enums.GetEnumDescription(Enums.ProductTestData.Discount),
                Price = new decimal((double) Convert.ToDecimal(Enums.GetEnumDescription(Enums.ProductTestData.Price)) ),
                Image = _testImageProduct,
                ImageId = _testImageProduct.Id,
                Stock = Convert.ToInt32(Enums.GetEnumDescription(Enums.ProductTestData.Stock))
            };
        }

        [Test]
        public async Task Get_ReturnsAListOfProducts_ProductsController()
        {
            var response = await _client.GetAsync(_requestProduct + "Get");
            response.EnsureSuccessStatusCode();

            Assert.IsTrue(true);
        }

        [Test]
        public async Task GetById_GetOneProduct_ProductsController()
        {
            //Arrange 
            _testProduct = await InsertIfNotAny();

            //Act
            var getResponseOneProduct = await _client.GetAsync(_requestProduct + "Get/" + _testProduct.Id);
            var fetched = await getResponseOneProduct.Content.ReadAsStringAsync();
            var fetchedProduct = JsonConvert.DeserializeObject<Product>(fetched);

            Assert.IsTrue(getResponseOneProduct.IsSuccessStatusCode);
            Assert.AreEqual(_testProduct.Id, fetchedProduct.Id);
            Assert.AreEqual(fetchedProduct.Name, fetchedProduct.Name);
        }


        [Test]
        public async Task Create_CreateAProduct_NewBrandNewCategoryNewImage_ProductsController()
        {
            //Arrange 

            //Act
            var postResponseProduct = await _client.PostAsJsonAsync(_requestProduct, _testProduct);
            var createdProduct = await postResponseProduct.Content.ReadAsStringAsync();
            var createdProductObj = JsonConvert.DeserializeObject<Product>(createdProduct);

            //for cleanup
            _testBrand.Id = createdProductObj.BrandId;
            _testImageBrand.Id = createdProductObj.Brand.ImageId;
            _testCategory.Id = createdProductObj.CategoryId;
            _testProduct.Id = createdProductObj.Id;
            _testImageProduct.Id = createdProductObj.ImageId;

            var getResponse = await _client.GetAsync(_requestProduct + "Get/" + createdProductObj.Id);
            var fetched = await getResponse.Content.ReadAsStringAsync();
            var fetchedProduct = JsonConvert.DeserializeObject<Product>(fetched);

            // Assert
            Assert.IsTrue(postResponseProduct.IsSuccessStatusCode);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual(_testProduct.Name, createdProductObj.Name);
            Assert.AreEqual(_testProduct.Name, fetchedProduct.Name);

            Assert.AreNotEqual(Guid.Empty, createdProductObj.Id);
            Assert.AreEqual(createdProductObj.Id, fetchedProduct.Id);
        }

        [Test]
        public async Task Create_CreateAProduct_NewBrandNewCategoryProductsExisitingImage_ProductsController()
        {
            //Arrange 
            _testBrand.Image = await InsertNewImageIfNotAny(_testImageBrand);
            _testImageBrand = _testBrand.Image;

            //create new image
            var createdImage = await InsertNewImage(_testImageBrand);

            //Act
            _testProduct.Image = createdImage;
            _testProduct.ImageId = createdImage.Id;
            _testImageProduct.Id = createdImage.Id;
            _testProduct.Brand.ImageId = _testImageBrand.Id; 

            var postResponseProduct = await _client.PostAsJsonAsync(_requestProduct, _testProduct);
            var createdProduct = await postResponseProduct.Content.ReadAsStringAsync();
            var createdProductObj = JsonConvert.DeserializeObject<Product>(createdProduct);

            //for cleanup
            _testBrand.Id = createdProductObj.BrandId;
            _testImageProduct.Id = createdProductObj.ImageId;
            _testCategory.Id = createdProductObj.CategoryId;
            _testProduct.Id = createdProductObj.Id;

            var getResponse = await _client.GetAsync(_requestProduct + "Get/" + createdProductObj.Id);
            var fetched = await getResponse.Content.ReadAsStringAsync();
            var fetchedProduct = JsonConvert.DeserializeObject<Product>(fetched);

            // Assert
            Assert.IsTrue(postResponseProduct.IsSuccessStatusCode);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual(_testProduct.Name, createdProductObj.Name);
            Assert.AreEqual(_testProduct.Name, fetchedProduct.Name);

            Assert.AreNotEqual(Guid.Empty, createdProductObj.Id);
            Assert.AreEqual(createdProductObj.Id, fetchedProduct.Id);
        }

        [Test]
        public async Task Create_CreateAProduct_NewBrandNewCategoryExistingImageProductsNewImage_ProductsController()
        {
            //Arrange 
            var selectedImage = await InsertNewImageIfNotAny(_testImageProduct);

            _testProduct.Image = selectedImage;
            _testProduct.ImageId = selectedImage.Id;
            _testImageProduct = selectedImage;

            //create a new brand image
            var createdImage = await InsertNewImage(_testImageBrand);

            //Act
            _testBrand.Image = createdImage;
            _testBrand.ImageId = createdImage.Id;
            _testImageBrand.Id = createdImage.Id;

            var postResponseProduct = await _client.PostAsJsonAsync(_requestProduct, _testProduct);
            var createdProduct = await postResponseProduct.Content.ReadAsStringAsync();
            var createdProductObj = JsonConvert.DeserializeObject<Product>(createdProduct);

            //for cleanup
            _testBrand.Id = createdProductObj.BrandId;
            _testImageProduct.Id = createdProductObj.ImageId;
            _testCategory.Id = createdProductObj.CategoryId;
            _testProduct.Id = createdProductObj.Id;

            var getResponse = await _client.GetAsync(_requestProduct + "Get/" + createdProductObj.Id);
            var fetched = await getResponse.Content.ReadAsStringAsync();
            var fetchedProduct = JsonConvert.DeserializeObject<Product>(fetched);

            // Assert
            Assert.IsTrue(postResponseProduct.IsSuccessStatusCode);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual(_testProduct.Name, createdProductObj.Name);
            Assert.AreEqual(_testProduct.Name, fetchedProduct.Name);

            Assert.AreNotEqual(Guid.Empty, createdProductObj.Id);
            Assert.AreEqual(createdProductObj.Id, fetchedProduct.Id);
        }

        [Test]
        public async Task Update_UpdateAProductViaPUT_ProductsController()
        {
            //Arrange
            //POST(Crete)
            var postResponse = await _client.PostAsJsonAsync(_requestProduct, _testProduct);
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
                Deal = Convert.ToBoolean(Enums.GetEnumDescription(Enums.ProductUpdatedTestData.Deal)),
                Description = Enums.GetEnumDescription(Enums.ProductUpdatedTestData.Description),
                Discount = Enums.GetEnumDescription(Enums.ProductUpdatedTestData.Discount),
                Image = selectedImage,
                ImageId = selectedImage.Id,
                Name = Enums.GetEnumDescription(Enums.ProductUpdatedTestData.Name),
                Price = new decimal(Convert.ToDouble(Enums.GetEnumDescription(Enums.ProductUpdatedTestData.Price))),
                Stock = Convert.ToInt32(Enums.GetEnumDescription(Enums.ProductUpdatedTestData.Stock))
            };

            var putResponse = await _client.PutAsJsonAsync(_requestProduct + createdProduct.Id, updatedProduct);

            //GET
            var getResponse = await _client.GetAsync(_requestProduct + "Get/" + updatedProduct.Id);
            var fetched = await getResponse.Content.ReadAsStringAsync();
            var fetchedProduct = JsonConvert.DeserializeObject<Product>(fetched);

            //for cleanup
            _testBrand = updatedProduct.Brand;
            _testImageProduct = updatedProduct.Image;
            _testImageBrand = updatedProduct.Brand.Image;
            _testCategory = updatedProduct.Category;
            _testProduct = updatedProduct;

            // Assert
            Assert.IsTrue(postResponse.IsSuccessStatusCode);
            Assert.IsTrue(putResponse.IsSuccessStatusCode);
            Assert.IsTrue(putResponse.StatusCode == HttpStatusCode.NoContent);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual(Enums.GetEnumDescription(Enums.ProductTestData.Name), createdProduct.Name);
            Assert.AreEqual(Enums.GetEnumDescription(Enums.ProductUpdatedTestData.Name), fetchedProduct.Name);
            Assert.AreEqual(Enums.GetEnumDescription(Enums.ProductTestData.Description), createdProduct.Description);
            Assert.AreEqual(Enums.GetEnumDescription(Enums.ProductUpdatedTestData.Description), fetchedProduct.Description);
            Assert.AreEqual(Enums.GetEnumDescription(Enums.ProductTestData.Discount), createdProduct.Discount);
            Assert.AreEqual(Enums.GetEnumDescription(Enums.ProductUpdatedTestData.Discount), fetchedProduct.Discount);
            Assert.AreEqual(Convert.ToBoolean(Enums.GetEnumDescription(Enums.ProductTestData.Deal)), createdProduct.Deal);
            Assert.AreEqual(Convert.ToBoolean(Enums.GetEnumDescription(Enums.ProductUpdatedTestData.Deal)), fetchedProduct.Deal);

            Assert.AreNotEqual(Guid.Empty, createdProduct.Id);
            Assert.AreEqual(createdProduct.Id, fetchedProduct.Id);

        }

        [Test]
        public async Task Delete_DeleteAProduct_ProductsController()
        {
            //Arrange 

            //Act
            //POST(Crete)
            var postResponse = await _client.PostAsJsonAsync(_requestProduct, _testProduct);
            var created = await postResponse.Content.ReadAsStringAsync();
            var createdProduct = JsonConvert.DeserializeObject<Product>(created);

            //DELETE
            var deleteResponse = await _client.DeleteAsync(_requestProduct + createdProduct.Id);
            var getResponse = await _client.GetAsync(_requestProduct + "Get");
            var all = getResponse.Content.ReadAsStringAsync();
            var allProducts = JsonConvert.DeserializeObject<List<Product>>(all.Result);

            //for cleanup
            _testBrand = createdProduct.Brand;
            _testImageBrand = createdProduct.Brand.Image;
            _testImageProduct = createdProduct.Image;
            _testCategory = createdProduct.Category;

            //Assert
            Assert.IsTrue(postResponse.IsSuccessStatusCode);
            Assert.IsTrue(deleteResponse.IsSuccessStatusCode);
            Assert.IsTrue(deleteResponse.StatusCode == HttpStatusCode.NoContent);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual(_testProduct.Name, createdProduct.Name);
            Assert.AreNotEqual(Guid.Empty, createdProduct.Id);

            if(allProducts.Count > 0)
                Assert.IsTrue(allProducts.Any(x => x.Id == createdProduct.Id == false));
        }

        private async Task<Product> InsertIfNotAny()
        {
            var getResponse = await _client.GetAsync(_requestProduct + "Get");
            var all = getResponse.Content.ReadAsStringAsync();
            var allProducts = JsonConvert.DeserializeObject<List<Product>>(all.Result);
            if (allProducts.Count > 0)
            {
                _testProduct = allProducts.FirstOrDefault();
            }
            else
            {
                var postResponse = await _client.PostAsJsonAsync(_requestProduct, _testProduct);
                var created = await postResponse.Content.ReadAsStringAsync();
                _testProduct = JsonConvert.DeserializeObject<Product>(created);
                _testBrand = _testProduct.Brand;
                _testCategory = _testProduct.Category;
                _testImageProduct.Id = _testProduct.ImageId;
                _testImageBrand.Id = _testProduct.Brand.ImageId;
                _testCategory.Id = _testProduct.Category.Id;
                _testBrand.Id = _testProduct.Brand.Id;
            }
            return _testProduct;
        }

        private async Task<Image> InsertNewImageIfNotAny(Image image)
        {
            var getResponseImage = await _client.GetAsync(_requestImage + "Get");
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
                selectedImage = await InsertNewImage(image);
            }

            return selectedImage;
        }

        private async Task<Image> InsertNewImage(Image image)
        {
            var postResponseImage = await _client.PostAsJsonAsync(_requestImage, image);
            var created = await postResponseImage.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Image>(created);
        }

        [TearDown]
        public async Task DeleteImagesAndBrands()
        {
            //Cleanup
            if (_testProduct != null && _testProduct.Id > 0
                && (_testProduct.Name == Enums.GetEnumDescription(Enums.ProductTestData.Name)
                || _testProduct.Name == Enums.GetEnumDescription(Enums.ProductUpdatedTestData.Name)))
            {
                await _client.DeleteAsync(_requestProduct + _testProduct.Id);
                _testProduct = null;
            }

            if (_testBrand != null && _testBrand.Id > 0
                && (_testBrand.Name == Enums.GetEnumDescription(Enums.BrandTestData.Name)
                || _testBrand.Name == Enums.GetEnumDescription(Enums.BrandUpdatedTestData.Name)))
            {
                await _client.DeleteAsync(_requestBrand + _testBrand.Id);
                _testBrand = null;
            }

            if (_testImageBrand != null && _testImageBrand.Id > 0
                && (_testImageBrand.Name == Enums.GetEnumDescription(Enums.ImageBrandTestData.Name)
                || _testImageBrand.Name == Enums.GetEnumDescription(Enums.ImageBrandUpdatedTestData.Name)))
            {
                await _client.DeleteAsync(_requestImage + _testImageBrand.Id);
                _testImageBrand = null;
            }

            if (_testImageProduct != null && _testImageProduct.Id > 0
                && (_testImageProduct.Name == Enums.GetEnumDescription(Enums.ImageProductTestData.Name)
                || _testImageProduct.Name == Enums.GetEnumDescription(Enums.ImageProductUpdatedTestData.Name)))
            {
                await _client.DeleteAsync(_requestImage + _testImageProduct.Id);
                _testImageProduct = null;
            }

            if (_testCategory != null && _testCategory.Id > 0
                && (_testCategory.Name == Enums.GetEnumDescription(Enums.CategoryTestData.Name)
                || _testCategory.Name == Enums.GetEnumDescription(Enums.CategoryUpdatedTestData.Name)))
            {
                await _client.DeleteAsync(_requestCategory + _testCategory.Id);
                _testCategory = null;
            }

        }

    }

}
