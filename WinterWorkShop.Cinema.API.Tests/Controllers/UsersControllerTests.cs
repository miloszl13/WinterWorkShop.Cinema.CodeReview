using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.API.Controllers;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Tests.Controllers
{
    [TestClass]
    public class UsersControllerTests
    {
        private Mock<IUserService> _userService;
        private Mock<IConfiguration> _configuration;
        private UsersController _controller;
        private UserDomainModel _userDomainModel;
        private List<UserDomainModel> _users;
        private UserModel _userModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _userService = new Mock<IUserService>();
            _configuration = new Mock<IConfiguration>();
            _controller = new UsersController(_userService.Object, _configuration.Object);
            //domain models
            _userDomainModel = new UserDomainModel()
            {
                Id = Guid.NewGuid(),
                FirstName = "milos",
                LastName = "mihajlovic",
                UserName = "milos123",
            };
            _users = new List<UserDomainModel>();
            _users.Add(_userDomainModel);
            //user model
            _userModel = new UserModel()
            {
                FirstName = "milos",
                LastName = "mihajlovic",
                UserName = "milos123",
            };
        }

        [TestMethod]
        public void GetAsync_Return_AllUsers()
        {
            //Arrange
            Task<List<UserDomainModel>> responseTask = Task.FromResult(_users);
            int expectedResultCount = 1;
            int expectedStatusCode = 200;

            _userService.Setup(x => x.GetAllAsync()).Returns(responseTask);

            //Act
            var result = _controller.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var userDomainModels = (List<UserDomainModel>)resultList;

            //Assert
            userDomainModels.Should().NotBeNull();
            expectedResultCount.Equals(userDomainModels.Count);
            _userDomainModel.Id.Equals(userDomainModels[0].Id);
            result.Should().BeOfType<OkObjectResult>();
            expectedStatusCode.Equals(((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void GetAsync_Return_NewList()
        {
            //Arrange
            List<UserDomainModel> userDomainModels = null;
            Task<List<UserDomainModel>> responseTask = Task.FromResult(_users);
            int expectedResultCount = 0;
            int expectedStatusCode = 200;

            _userService.Setup(x => x.GetAllAsync()).Returns(responseTask);

            //Act
            var result = _controller.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var userDomainModelResult = (List<UserDomainModel>)resultList;

            //Assert
            userDomainModelResult.Should().NotBeNull();
            expectedResultCount.Equals(userDomainModelResult.Count);
            result.Should().BeOfType<OkObjectResult>();
            expectedStatusCode.Equals(((OkObjectResult)result).StatusCode);
        }
        [TestMethod]
        public void GetByIdAsync_UserNotExist_ReturnNotFound()
        {
            //arrange
            var expectedMessage= "User does not exist.";
            UserDomainModel userDomain = null;
            Task<UserDomainModel> responseTask = Task.FromResult(userDomain);
            _userService.Setup(x => x.GetUserByIdAsync(It.IsAny<Guid>())).Returns(responseTask);
            //act
            var result=_controller.GetbyIdAsync(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var errorResponse = ((NotFoundObjectResult)result).Value;
            //Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            errorResponse.Should().Be(expectedMessage);
        }
        [TestMethod]
        public void GetByIdAsync_UserExist_UserDomainModel()
        {
            //arrange
            _userService.Setup(x => x.GetUserByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(_userDomainModel));
            //act
            var result = _controller.GetbyIdAsync(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultuser = ((OkObjectResult)result).Value;
            //Assert
            result.Should().NotBe(null);
            resultuser.Should().Be(_userDomainModel);
        }
        [TestMethod]
        public void GetbyUserNameAsync_UserNotExist_ReturnNotFound()
        {
            //arrange
            var expectedMessage = "User does not exist.";
            UserDomainModel userDomain = null;
            Task<UserDomainModel> responseTask = Task.FromResult(userDomain);
            _userService.Setup(x => x.GetUserByUserName(It.IsAny<string>())).Returns(responseTask);
            //act
            var result = _controller.GetbyUserNameAsync("milos123").ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var errorResponse = ((NotFoundObjectResult)result).Value;
            //Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            errorResponse.Should().Be(expectedMessage);
        }
        [TestMethod]
        public void GetbyUserNameAsync_UserExist_UserDomainModel()
        {
            //arrange
            _userService.Setup(x => x.GetUserByUserName(It.IsAny<string>())).Returns(Task.FromResult(_userDomainModel));
            //act
            var result = _controller.GetbyUserNameAsync("milos123").ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultuser = ((OkObjectResult)result).Value;
            //Assert
            result.Should().NotBe(null);
            resultuser.Should().Be(_userDomainModel);
        }
        [TestMethod]
        public void Register_WithUnValidModelState_ReturnBadRequest()
        {
            //Arrange
            string expectedMessage = "Invalid Model State";
            int expectedStatusCode = 400;
            _controller.ModelState.AddModelError("key", "Invalid Model State");

            //Act
            var result = _controller.Register(_userModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = (BadRequestObjectResult)result;
            var createdResult = ((BadRequestObjectResult)result).Value;
            var errorResponse = ((SerializableError)createdResult).GetValueOrDefault("key");
            var message = (string[])errorResponse;

            //Assert
            resultResponse.Should().NotBeNull();
            expectedMessage.Should().BeEquivalentTo(message[0]);
            result.Should().BeOfType<BadRequestObjectResult>();
            expectedStatusCode.Should().Be(resultResponse.StatusCode);
        }
        [TestMethod]
        public void Register_CreateUserThrowDbException_ReturnsBadRequest()
        {
            //Arrange
            string expectedMessage = "Inner exception error message.";
            int expectedStatusCode = 400;

            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);

            _userService.Setup(x => x.AddUser(It.IsAny<UserDomainModel>())).Throws(dbUpdateException);
            //Act
            var result = _controller.Register(_userModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = (BadRequestObjectResult)result;
            var badObjectResult = ((BadRequestObjectResult)result).Value;
            var errorResult = (ErrorResponseModel)badObjectResult;

            //Assert
            resultResponse.Should().NotBeNull();
            expectedMessage.Should().BeEquivalentTo(errorResult.ErrorMessage);
            result.Should().BeOfType<BadRequestObjectResult>();
            expectedStatusCode.Should().Be(resultResponse.StatusCode);

        }
        [TestMethod]
        public void Register_CreateUserReturnsNull_ReturnsBadRequest()
        {
            //Arrange
            string expectedMessage = "Error occured while creating new user, please try again.";
            int expectedStatusCode = 500;

            UserDomainModel userNullModel = null;
            Task<UserDomainModel> responseTask = Task.FromResult(userNullModel);


            _userService.Setup(x => x.AddUser(It.IsAny<UserDomainModel>())).Returns(responseTask);
            //Act
            var result = _controller.Register(_userModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = (ObjectResult)result;
            var ObjectResult = ((ObjectResult)result).Value;
            var errorResult = (ErrorResponseModel)ObjectResult;

            //Assert
            //resultResponse.Should().NotBeNull();
            expectedMessage.Should().BeEquivalentTo(errorResult.ErrorMessage);
            expectedStatusCode.Should().Be(resultResponse.StatusCode);

        }
        [TestMethod]
        public void Register_Successful_ReturnUser()
        {
            //Arrange
            int expectedStatusCode = 201;
            Task<UserDomainModel> responseTask = Task.FromResult(_userDomainModel);
            _userService.Setup(x => x.AddUser(It.IsAny<UserDomainModel>())).Returns(responseTask);

            //Act
            var result = _controller.Register(_userModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var createdResult = ((CreatedResult)result).Value;
            var userDomainModelResult = (UserDomainModel)createdResult;

            //Assert
            userDomainModelResult.Should().BeEquivalentTo(_userDomainModel);
            result.Should().BeOfType<CreatedResult>();
            expectedStatusCode.Equals(((CreatedResult)result).StatusCode).Should().BeTrue();
        }
       
    }
}
