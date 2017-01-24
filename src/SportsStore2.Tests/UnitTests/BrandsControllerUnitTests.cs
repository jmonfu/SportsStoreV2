using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SportsStore2.API.Controllers;
using SportsStore2.API.Models;
using SportsStore2.API.Services;
using SportsStore2.API.Repository;

namespace SportsStore2.Tests.UnitTests
{
    [TestFixture()]
    public class BrandsControllerUnitTests
    {
        public List<Brand> Brands;

        [SetUp]
        public void Setup()
        {
            Brands = new List<Brand>
            {
                new Brand
                {
                    Id = 1,
                    Name = "Adidas",
                    ImageId = 1
                },
                new Brand
                {
                    Id = 2,
                    Name = "Nike",
                    ImageId = 2
                },
                new Brand
                {
                    Id = 3,
                    Name = "Puma",
                    ImageId = 3
                }
            };
        }

        [Test]
        public async Task Get_ReturnsAListOfBrands_BrandsController()
        {
            //Arrange
            var mockService = new Mock<IGenericService<Brand>>();
            mockService.Setup(repo => repo.GetAll(
                    It.IsAny<Func<IQueryable<Brand>, IOrderedQueryable<Brand>>>(),
                    It.IsAny<string>()
                )).ReturnsAsync(Brands);
            var controller = new BrandsController(mockService.Object);

            //Act
            JsonResult actualResult = await controller.Get() as JsonResult;
            dynamic obj = actualResult.Value;
            int i = 0;
            foreach (var value in obj)
            {
                i++;
            }

            //Assert
            Assert.AreEqual(i, 3);
        }

        //[Test]
        //public async Task GetById_ReturnsOneBrand_BrandsController()
        //{
        //    //Arrange
        //    var mockService = new Mock<IGenericService<Brand>>();
        //    mockService.Setup(repo => repo.GetById<Brand>(
        //            It.IsAny<Expression<Func<Brand, bool>>>(),
        //            It.IsAny<string>(),
        //            It.IsAny<bool>())
        //        ).ReturnsAsync(new Brand {Id = 1, Name = "Adidas"});
        //    var controller = new BrandsController(mockService.Object);

        //    //Act
        //    JsonResult actualResult = await controller.Get(1) as JsonResult;
        //    var obj = actualResult.Value as Brand;

        //    //Assert
        //    Assert.AreEqual(obj.Id, 1);
        //    Assert.AreEqual(obj.Name, "Adidas");
        //}

        //[Test]
        //public void CreateBrand_ValidBrand_Returns_CreatedAtRouteResult_BrandsController()
        //{
        //    var mockService = new Mock<IGenericService<Brand>>();
        //    var brand = new Brand
        //    {
        //        Id = 10,
        //        Name = "TestBrand",
        //        ImageId = 1,
        //        Image = new Image { Id = 1, Name = "TestImage", Url = "C:/Images" }
        //    };

        //    mockService.Setup(s => s.Add(
        //                It.IsAny<Brand>(),
        //                It.IsAny<Expression<Func<Brand, bool>>>())
        //                ).Returns(new Task<bool>(() => true));

        //    var controller = new BrandsController(mockService.Object);
        //    var actualResult = controller.Create(brand);

        //    //Assert
        //    Assert.AreEqual(actualResult.GetType(), typeof(CreatedAtRouteResult));
        //}

        //[Test]
        //public void CreateBrand_NullBrand_Returns_BadRequest_BrandsController()
        //{
        //    var mockService = new Mock<IGenericService<Brand>>();
        //    Brand brand = null;

        //    mockService.Setup(s => s.Add(
        //                It.IsAny<Brand>(),
        //                It.IsAny<Expression<Func<Brand, bool>>>())
        //                ).Returns(new Task<bool>(() => true));

        //    var controller = new BrandsController(mockService.Object);
        //    var actualResult = controller.Create(brand);

        //    //Assert
        //    Assert.AreEqual(actualResult.GetType(), typeof(BadRequestResult));
        //}

        //[Test]
        //public void UpdateBrand_ValidBrand_Returns_NoContentResult_BrandsController()
        //{
        //    var mockService = new Mock<IGenericService<Brand>>();
        //    mockService.Setup(s => s.Update(
        //                It.IsAny<Brand>())
        //                ).Returns(true);

        //    var controller = new BrandsController(mockService.Object);
        //    var brand = Brands.FirstOrDefault(x => x.Id == 1);
        //    var actualResult = controller.Update(Convert.ToInt32(brand.Id) , brand);

        //    //Assert
        //    Assert.AreEqual(actualResult.GetType(), typeof(NoContentResult));
        //}

        //[Test]
        //public void UpdateBrand_NullBrand_Returns_BadRequest_BrandsController()
        //{
        //    var mockService = new Mock<IGenericService<Brand>>();

        //    mockService.Setup(s => s.Update(
        //                It.IsAny<Brand>())
        //                ).Returns(true);

        //    var controller = new BrandsController(mockService.Object);
        //    var actualResult = controller.Update(Convert.ToInt32(0), null);

        //    //Assert
        //    Assert.AreEqual(actualResult.GetType(), typeof(BadRequestResult));
        //}

        //[Test]
        //public void DeleteBrand_ValidBrand_Returns_NoContentResult_BrandsController()
        //{
        //    var mockService = new Mock<IGenericService<Brand>>();
        //    mockService.Setup(s => s.Delete(
        //                It.IsAny<Brand>())
        //                );

        //    var controller = new BrandsController(mockService.Object);
        //    var brand = Brands.FirstOrDefault(x => x.Id == 1);
        //    var actualResult = controller.Delete(Convert.ToInt32(brand.Id));

        //    //Assert
        //    Assert.AreEqual(actualResult.GetType(), typeof(NoContentResult));
        //}

        //[Test]
        //public void DeleteBrand_NullBrand_Returns_BadRequest_BrandsController()
        //{
        //    var mockService = new Mock<IGenericService<Brand>>();

        //    mockService.Setup(s => s.Update(
        //                It.IsAny<Brand>())
        //                ).Returns(true);

        //    var controller = new BrandsController(mockService.Object);
        //    var actualResult = controller.Delete(Convert.ToInt32(0));

        //    //Assert
        //    Assert.AreEqual(actualResult.GetType(), typeof(BadRequestResult));
        //}
    }
}
