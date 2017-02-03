using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json;
using NUnit.Framework;
using SportsStore2.API;
using SportsStore2.API.Models;
using SportsStore2.API.Models.AccountViewModels;

namespace SportsStore2.Tests.IntegrationTests
{
    [TestFixture]
    public class AddressControllerIntegrationTests
    {
        private HttpClient _client;
        private Address _testAddress;
        private Country _testCountry;
        private User _testUser;
        private AddressType _testAddressType;
        private string _request;
        private string _requestUser;
        private string _requestCountry;
        private string _requestAddressType;

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

            _testCountry = new Country
            {
                Name = Enums.GetEnumDescription(Enums.CountryTestData.Name),
                Code = Enums.GetEnumDescription(Enums.CountryTestData.Code),
                Type = Enums.GetEnumDescription(Enums.CountryTestData.Type)
            };

            _testUser = new User
            {
                Name = Enums.GetEnumDescription(Enums.UserTestData.Name),
                Surname = Enums.GetEnumDescription(Enums.UserTestData.Surname),
                Email = Enums.GetEnumDescription(Enums.UserTestData.Email),
                HomeNo = Enums.GetEnumDescription(Enums.UserTestData.HomeNo),
                MobNo = Enums.GetEnumDescription(Enums.UserTestData.MobNo)
            };

            _testAddress = new Address
            {
                Address1 = Enums.GetEnumDescription(Enums.AddressTestData.Address1),
                Address2 = Enums.GetEnumDescription(Enums.AddressTestData.Address2),
                Address3 = Enums.GetEnumDescription(Enums.AddressTestData.Address3),
                City = Enums.GetEnumDescription(Enums.AddressTestData.City),
                PostCode = Enums.GetEnumDescription(Enums.AddressTestData.PostCode)
            };

            _testAddressType = new AddressType
            {
                Name = Enums.GetEnumDescription(Enums.AddressTypeTestData.Name)
            };

            _request = Enums.GetEnumDescription(Enums.Requests.Address);
            _requestUser = Enums.GetEnumDescription(Enums.Requests.User);
            _requestCountry = Enums.GetEnumDescription(Enums.Requests.Countries);
            _requestAddressType = Enums.GetEnumDescription(Enums.Requests.AddressType);
        }


        [Test]
        public async Task Get_ReturnsAListOfAddresses_AddressController()
        {
            var response = await _client.GetAsync(_request + "Get");
            response.EnsureSuccessStatusCode();

            Assert.IsTrue(true);
        }

        [Test]
        public async Task GetById_GetOneAddress_AddressController()
        {
            //Arrange 
            _testAddress = await InsertIfNotAny();

            var getResponseOneAddress = await _client.GetAsync(_request + "Get/" + _testAddress.Id);
            var fetched = await getResponseOneAddress.Content.ReadAsStringAsync();
            var fetchedAddress = JsonConvert.DeserializeObject<Address>(fetched);

            Assert.IsTrue(getResponseOneAddress.IsSuccessStatusCode);
            Assert.AreEqual(_testAddress.Id, fetchedAddress.Id);
            Assert.AreEqual(_testAddress.Address1, fetchedAddress.Address1);
            Assert.AreEqual(_testAddress.City, fetchedAddress.City);
            Assert.AreEqual(_testAddress.Country, fetchedAddress.Country);
            Assert.AreEqual(_testAddress.PostCode, fetchedAddress.PostCode);

        }

        [Test]
        public async Task Create_CreateAnAddress_NewCountryNewUser_AddressController()
        {
            //Arrange 
            //insert new country
            var newCountry = await InsertNewCountry();
            _testCountry.Id = newCountry.Id;

            //insert new user
            var newUser = await InsertNewUserAndAspNetUser(_testUser);
            _testUser.Id = newUser.Id;

            //insert new AddressType
            var newAddressType = await InsertNewAddressType();
            _testAddressType.Id = newAddressType.Id;

            _testAddress.CountryId = _testCountry.Id;
            _testAddress.UserId = _testUser.Id;
            _testAddress.AddressTypeId = _testAddressType.Id;

            //Act
            var postResponse = await _client.PostAsJsonAsync(_request, _testAddress);
            var created = await postResponse.Content.ReadAsStringAsync();
            var createdAddress = JsonConvert.DeserializeObject<Address>(created);

            _testAddress.Id = createdAddress.Id;

            var getResponse = await _client.GetAsync(_request + "Get/" + createdAddress.Id);
            var fetched = await getResponse.Content.ReadAsStringAsync();
            var fetchedAddress = JsonConvert.DeserializeObject<Address>(fetched);

            // Assert
            Assert.IsTrue(postResponse.IsSuccessStatusCode);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual(_testAddress.Address1, createdAddress.Address1);
            Assert.AreEqual(_testAddress.Address1, fetchedAddress.Address1);
            Assert.AreEqual(_testAddress.Address2, createdAddress.Address2);
            Assert.AreEqual(_testAddress.Address2, fetchedAddress.Address2);
            Assert.AreEqual(_testAddress.Address3, createdAddress.Address3);
            Assert.AreEqual(_testAddress.Address3, fetchedAddress.Address3);
            Assert.AreEqual(_testAddress.City, createdAddress.City);
            Assert.AreEqual(_testAddress.City, fetchedAddress.City);
            Assert.AreEqual(_testAddress.Country, createdAddress.Country);
            Assert.AreEqual(_testAddress.Country, fetchedAddress.Country);
            Assert.AreEqual(_testAddress.User, createdAddress.User);
            Assert.AreEqual(_testAddress.User, fetchedAddress.User);

            Assert.AreNotEqual(Guid.Empty, createdAddress.Id);
            Assert.AreEqual(createdAddress.Id, createdAddress.Id);
        }

        [Test]
        public async Task Create_CreateAnAddress_ExisitingCountryExisitingUser_AddressController()
        {
            //Arrange 
            _testAddress = await SetTestAddressWithCountryAndUserAndAddressType();

            //Act
            var postResponse = await _client.PostAsJsonAsync(_request, _testAddress);
            var created = await postResponse.Content.ReadAsStringAsync();
            var createdAddress = JsonConvert.DeserializeObject<Address>(created);

            _testAddress.Id = createdAddress.Id;

            //for cleanup
            var getResponse = await _client.GetAsync(_request + "Get/" + createdAddress.Id);
            var fetched = await getResponse.Content.ReadAsStringAsync();
            var fetchedAddress = JsonConvert.DeserializeObject<Address>(fetched);

            // Assert
            Assert.IsTrue(postResponse.IsSuccessStatusCode);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual(_testAddress.Address1, createdAddress.Address1);
            Assert.AreEqual(_testAddress.Address1, fetchedAddress.Address1);
            Assert.AreEqual(_testAddress.Address2, createdAddress.Address2);
            Assert.AreEqual(_testAddress.Address2, fetchedAddress.Address2);
            Assert.AreEqual(_testAddress.Address3, createdAddress.Address3);
            Assert.AreEqual(_testAddress.Address3, fetchedAddress.Address3);
            Assert.AreEqual(_testAddress.City, createdAddress.City);
            Assert.AreEqual(_testAddress.City, fetchedAddress.City);
            Assert.AreEqual(_testAddress.CountryId, createdAddress.CountryId);
            Assert.AreEqual(_testAddress.CountryId, fetchedAddress.CountryId);
            Assert.AreEqual(_testAddress.UserId, createdAddress.UserId);
            Assert.AreEqual(_testAddress.UserId, fetchedAddress.UserId);

            Assert.AreNotEqual(Guid.Empty, createdAddress.Id);
            Assert.AreEqual(createdAddress.Id, createdAddress.Id);
        }

        [Test]
        public async Task Update_UpdateAnAddressViaPUT_AddressController()
        {
            //Arrange
            //POST(Crete)

            _testAddress = await SetTestAddressWithCountryAndUserAndAddressType();

            var postResponse = await _client.PostAsJsonAsync(_request, _testAddress);
            var created = await postResponse.Content.ReadAsStringAsync();
            var createdAddress = JsonConvert.DeserializeObject<Address>(created);

            _testAddress.Id = createdAddress.Id;

            var updatedAddress = new Address
            {
                Id = createdAddress.Id,
                CountryId = createdAddress.CountryId,
                UserId = createdAddress.UserId,
                Address1 = Enums.GetEnumDescription(Enums.AddressTestDataUpdated.Address1),
                Address2 = Enums.GetEnumDescription(Enums.AddressTestDataUpdated.Address2),
                Address3 = Enums.GetEnumDescription(Enums.AddressTestDataUpdated.Address3),
                AddressTypeId = _testAddress.AddressTypeId,
                City = Enums.GetEnumDescription(Enums.AddressTestDataUpdated.City),
                PostCode = Enums.GetEnumDescription(Enums.AddressTestDataUpdated.PostCode)
            };

            var putResponse = await _client.PutAsJsonAsync(_request + createdAddress.Id, updatedAddress);

            //GET
            var getResponse = await _client.GetAsync(_request + "Get/" + updatedAddress.Id);
            var fetched = await getResponse.Content.ReadAsStringAsync();
            var fetchedAddress = JsonConvert.DeserializeObject<Address>(fetched);

            // Assert
            Assert.IsTrue(postResponse.IsSuccessStatusCode);
            Assert.IsTrue(putResponse.IsSuccessStatusCode);
            Assert.IsTrue(putResponse.StatusCode == HttpStatusCode.NoContent);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual(Enums.GetEnumDescription(Enums.AddressTestData.Address1), createdAddress.Address1);
            Assert.AreEqual(Enums.GetEnumDescription(Enums.AddressTestDataUpdated.Address1), fetchedAddress.Address1);
            Assert.AreEqual(Enums.GetEnumDescription(Enums.AddressTestData.Address2), createdAddress.Address2);
            Assert.AreEqual(Enums.GetEnumDescription(Enums.AddressTestDataUpdated.Address2), fetchedAddress.Address2);
            Assert.AreEqual(Enums.GetEnumDescription(Enums.AddressTestData.Address3), createdAddress.Address3);
            Assert.AreEqual(Enums.GetEnumDescription(Enums.AddressTestDataUpdated.Address3), fetchedAddress.Address3);
            Assert.AreEqual(createdAddress.AddressTypeId, fetchedAddress.AddressTypeId);
            Assert.AreEqual(Enums.GetEnumDescription(Enums.AddressTestData.City), createdAddress.City);
            Assert.AreEqual(Enums.GetEnumDescription(Enums.AddressTestDataUpdated.City), fetchedAddress.City);
            Assert.AreEqual(Enums.GetEnumDescription(Enums.AddressTestData.PostCode), createdAddress.PostCode);
            Assert.AreEqual(Enums.GetEnumDescription(Enums.AddressTestDataUpdated.PostCode), fetchedAddress.PostCode);

            Assert.AreNotEqual(Guid.Empty, createdAddress.Id);
            Assert.AreEqual(createdAddress.Id, fetchedAddress.Id);

        }

        [Test]
        public async Task Delete_DeleteAnAddress_AddressController()
        {
            //Arrange 

            //Act
            //POST(Crete)
            _testAddress = await SetTestAddressWithCountryAndUserAndAddressType();

            var postResponse = await _client.PostAsJsonAsync(_request, _testAddress);
            var created = await postResponse.Content.ReadAsStringAsync();
            var createdAddress = JsonConvert.DeserializeObject<Address>(created);

            _testAddress.Id = createdAddress.Id;

            //DELETE
            var deleteResponse = await _client.DeleteAsync(_request+ createdAddress.Id);
            var getResponse = await _client.GetAsync(_request + "Get");
            var all = getResponse.Content.ReadAsStringAsync();
            var allAddresses = JsonConvert.DeserializeObject<List<Address>>(all.Result);

            //for cleanup

            //Assert
            Assert.IsTrue(postResponse.IsSuccessStatusCode);
            Assert.IsTrue(deleteResponse.IsSuccessStatusCode);
            Assert.IsTrue(deleteResponse.StatusCode == HttpStatusCode.NoContent);
            Assert.IsTrue(getResponse.IsSuccessStatusCode);

            Assert.AreEqual(_testAddress.Address1, createdAddress.Address1);
            Assert.AreEqual(_testAddress.Address2, createdAddress.Address2);
            Assert.AreEqual(_testAddress.Address3, createdAddress.Address3);
            Assert.AreEqual(_testAddress.City, createdAddress.City);
            Assert.AreEqual(_testAddress.CountryId, createdAddress.CountryId);
            Assert.AreEqual(_testAddress.UserId, createdAddress.UserId);
            Assert.AreNotEqual(Guid.Empty, createdAddress.Id);

            if(allAddresses.Count > 0)
                Assert.IsTrue(allAddresses.Any(x => x.Id == createdAddress.Id == false));
        }

        private async Task<Country> InsertNewCountry()
        {
            var postResponse = await _client.PostAsJsonAsync(_requestCountry, _testCountry);
            var created = await postResponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Country>(created);
        }

        private async Task<User> InsertNewUserAndAspNetUser(User user)
        {
            var registerViewModel = new RegisterViewModel
            {
                ConfirmPassword = "Password@123",
                Email = user.Email,
                Password = "Password@123"
            };
            var postResponse = await _client.PostAsJsonAsync(_requestUser + "CreateAspNetUsers", registerViewModel);
            var created = await postResponse.Content.ReadAsStringAsync();
            var createdUser = JsonConvert.DeserializeObject<AspNetUsers>(created);


            if (createdUser != null)
                return await InsertNewUser(user);

            return null;
        }

        private async Task<User> InsertNewUser(User user)
        {
            var postResponse = await _client.PostAsJsonAsync(_requestUser + "Create", user);
            var created = await postResponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<User>(created);
        }

        private async Task<AddressType> InsertNewAddressType()
        {
            var postResponse = await _client.PostAsJsonAsync(_requestAddressType, _testAddressType);
            var created = await postResponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<AddressType>(created);
        }

        private async Task<Country> GetCountry()
        {
            var getResponseCountry = await _client.GetAsync(_requestCountry + "Get");
            var all = getResponseCountry.Content.ReadAsStringAsync();
            var allCountries = JsonConvert.DeserializeObject<List<Country>>(all.Result);
            Country selectedCountry;
            if (allCountries.Count > 0)
            {
                selectedCountry = allCountries.FirstOrDefault();
            }
            else
            {
                selectedCountry = await InsertNewCountry();
                _testCountry.Id = selectedCountry.Id;
            }

            return selectedCountry;
        }

        private async Task<User> GetUser()
        {
            var getResponseUser = await _client.GetAsync(_requestUser + "Get");
            var allUsers = getResponseUser.Content.ReadAsStringAsync();
            var allUsersObj = JsonConvert.DeserializeObject<List<User>>(allUsers.Result);
            User selectedUser;
            if (allUsersObj.Count > 0)
            {
                selectedUser = allUsersObj.FirstOrDefault();
            }
            else
            {
                selectedUser = await InsertNewUserAndAspNetUser(_testUser);
                _testUser.Id = selectedUser.Id;
            }
            return selectedUser;

        }

        private async Task<AddressType> GetAddressType()
        {
            var getResponseUser = await _client.GetAsync(_requestAddressType + "Get");
            var allAddressTypes = getResponseUser.Content.ReadAsStringAsync();
            var allAddressTypesObj = JsonConvert.DeserializeObject<List<AddressType>>(allAddressTypes.Result);
            AddressType selectedAddressType;
            if (allAddressTypesObj.Count > 0)
            {
                selectedAddressType = allAddressTypesObj.FirstOrDefault();
            }
            else
            {
                selectedAddressType = await InsertNewAddressType();
                _testAddressType.Id = selectedAddressType.Id;
            }
            return selectedAddressType;

        }

        private async Task<Address> InsertIfNotAny()
        {
            //Act
            var getResponse = await _client.GetAsync(_request + "Get");
            var all = getResponse.Content.ReadAsStringAsync();
            var allAddresses = JsonConvert.DeserializeObject<List<Address>>(all.Result);
            if (allAddresses.Count > 0)
            {
                _testAddress = allAddresses.FirstOrDefault();
            }
            else
            {
                //set testAddress with Country and User
                _testAddress = await SetTestAddressWithCountryAndUserAndAddressType();

                var postResponse = await _client.PostAsJsonAsync(_request, _testAddress);
                var created = await postResponse.Content.ReadAsStringAsync();
                _testAddress = JsonConvert.DeserializeObject<Address>(created);
            }
            return _testAddress;
        }

        private async Task<Address> SetTestAddressWithCountryAndUserAndAddressType()
        {
            //get country, if there aren't any, create one
            var selectedCountry = await GetCountry();

            //get user, if there aren't any, create one
            var selectedUser = await GetUser();

            //get addressType, if there aren't any, create one
            var selectedAddressType = await GetAddressType();

            _testAddress.Country = selectedCountry;
            _testAddress.CountryId = selectedCountry.Id;
            _testAddress.User = selectedUser;
            _testAddress.UserId = selectedUser.Id;
            _testAddress.AddressTypeId = selectedAddressType.Id;

            return _testAddress;
        }


        [TearDown]
        public async Task DeleteAddress()
        {
            //Cleanup
            if (_testAddress != null && _testAddress.Id > 0
                    && (_testAddress.Address1 == Enums.GetEnumDescription(Enums.AddressTestData.Address1) 
                    || _testAddress.Address1 == Enums.GetEnumDescription(Enums.AddressTestDataUpdated.Address1)))

            {
                await _client.DeleteAsync(_request + _testAddress.Id);
                _testAddress = null;
            }

            if (_testCountry != null && _testCountry.Id > 0
                    && (_testCountry.Name == Enums.GetEnumDescription(Enums.CountryTestData.Name) 
                    || _testCountry.Name == Enums.GetEnumDescription(Enums.CountryUpdatedTestData.Name)))
            {
                await _client.DeleteAsync(_requestCountry + _testCountry.Id);
                _testCountry = null;
            }

            if (_testUser != null && _testUser.Id > 0
                    && (_testUser.Name == Enums.GetEnumDescription(Enums.UserTestData.Name) 
                    || _testUser.Name == Enums.GetEnumDescription(Enums.UserUpatedTestData.Name)))
            {
                await _client.DeleteAsync(_requestUser + _testUser.Id);
                _testUser = null;
            }

            if (_testAddressType != null && _testAddressType.Id > 0
                && (_testAddressType.Name == Enums.GetEnumDescription(Enums.AddressTypeTestData.Name) 
                || _testAddressType.Name == Enums.GetEnumDescription(Enums.AddressTypeUpdtedTestData.Name)))
    
            {
                await _client.DeleteAsync(_requestAddressType + _testAddressType.Id);
                _testAddressType = null;
            }

        }

    }
}
