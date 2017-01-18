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
        public async Task Get_ReturnsAAListOfBrands()
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

    }
}
