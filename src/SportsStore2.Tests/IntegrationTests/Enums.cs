using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SportsStore2.Tests.IntegrationTests
{
    public class Enums
    {
        public enum Requests
        {
            [Description("api/Address/")]
            Address = 1,
            [Description("api/Users/")]
            User = 2,
            [Description("api/Countries/")]
            Countries = 3,
            [Description("api/AddressType/")]
            AddressType = 4,
            [Description("api/Brands/")]
            Brands = 5,
            [Description("api/Images/")]
            Images = 6,
            [Description("api/Categories/")]
            Categories = 7,
            [Description("api/Products/")]
            Products = 8
        }

        public enum UserTestData
        {
            [Description("TestUserName")]
            Name = 1,
            [Description("TestUserSurname")]
            Surname = 2,
            [Description("TestUserEmail@test.com")]
            Email = 3,
            [Description("07460634348")]
            HomeNo = 4,
            [Description("02460634348")]
            MobNo = 5
        }

        public enum UserUpatedTestData
        {
            [Description("TestUserNameUpdated")]
            Name = 1,
            [Description("TestUserSurnameUpdated")]
            Surname = 2,
            [Description("TestUserEmailUpdated@test.com")]
            Email = 3,
            [Description("07000000000")]
            HomeNo = 4,
            [Description("02000000000")]
            MobNo = 5
        }

        public enum CountryTestData
        {
            [Description("testCountry")]
            Name = 1,
            [Description("TT")]
            Code = 2,
            [Description("1 Tier")]
            Type = 3
        }
        public enum CountryUpdatedTestData
        {
            [Description("TestCountryUpdated")]
            Name = 1,
            [Description("AA")]
            Code = 2,
            [Description("2 Tier")]
            Type = 3
        }

        public enum AddressTestData
        {
            [Description("TestAddress1")]
            Address1 = 1,
            [Description("TestAddress2")]
            Address2 = 2,
            [Description("TestAddress3")]
            Address3 = 3,
            [Description("TestCity")]
            City = 4,
            [Description("E14 2DA")]
            PostCode = 5
        }

        public enum AddressTestDataUpdated
        {
            [Description("TestAddress1Updated")]
            Address1 = 1,
            [Description("TestAddress2Updated")]
            Address2 = 2,
            [Description("TestAddress3Updated")]
            Address3 = 3,
            [Description("TestCityUpdated")]
            City = 4,
            [Description("A12 3AA")]
            PostCode = 5
        }

        public enum CategoryTestData
        {
            [Description("TestCategory")]
            Name = 1
        }
        public enum CategoryUpdatedTestData
        {
            [Description("TestCategoryUpdated")]
            Name = 1
        }

        public enum ImageBrandTestData
        {
            [Description("TestBrandImage")]
            Name = 1,
            [Description("/Brands/test.png")]
            Url = 2
        }

        public enum ImageBrandUpdatedTestData
        {
            [Description("TestBrandImageUpdated")]
            Name = 1,
            [Description("/Brands/testUpdated.png")]
            Url = 2
        }

        public enum ImageProductTestData
        {
            [Description("TestProductImage")]
            Name = 1,
            [Description("/Products/test.png")]
            Url = 2
        }

        public enum ImageProductUpdatedTestData
        {
            [Description("TestProductImageUpdated")]
            Name = 1,
            [Description("/Products/testUpdated.png")]
            Url = 2
        }

        public enum ProductTestData
        {
            [Description("TestProduct")]
            Name = 1,
            [Description("false")]
            Deal = 2,
            [Description("TestDescription")]
            Description = 3,
            [Description("50% Discount")]
            Discount = 4,
            [Description("50.00")]
            Price = 5,
            [Description("5")]
            Stock = 6
        }

        public enum ProductUpdatedTestData
        {
            [Description("TestProductUpdated")]
            Name = 1,
            [Description("true")]
            Deal = 2,
            [Description("TestDescriptionUpdated")]
            Description = 3,
            [Description("20% Discount")]
            Discount = 4,
            [Description("100.00")]
            Price = 5,
            [Description("10")]
            Stock = 6
        }

        public enum BrandTestData
        {
            [Description("TestBrand")]
            Name = 1
        }

        public enum BrandUpdatedTestData
        {
            [Description("TestBrandUpdated")]
            Name = 1
        }

        public enum AddressTypeTestData
        {
            [Description("TestAddressType")]
            Name = 1
        }
        public enum AddressTypeUpdtedTestData
        {
            [Description("TestAddressTypeUpdated")]
            Name = 1
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
    }
}
