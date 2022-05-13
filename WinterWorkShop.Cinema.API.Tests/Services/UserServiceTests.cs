using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Domain.Services;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Tests.Services
{
    [TestClass]
    public class UserServiceTests
    {
        private Mock<IUsersRepository> _usersRepository;
        private UserService _userService;
        private UserDomainModel _userDomainModel;
        private User _user;
        private List<User> _users;

        [TestInitialize]
        public void TestInitialize()
        {
            _usersRepository = new Mock<IUsersRepository>();
            _userService = new UserService(_usersRepository.Object);
            //domain models
            _userDomainModel = new UserDomainModel()
            {
                Id = Guid.Parse("8cbd0b6d-1437-4cee-8780-7c6fdc4b58c6"),
                FirstName = "milos",
                LastName = "mihajlovic",
                UserName = "milos123",
                //IsAdmin = true,
                BonusPoints=0
            };
            //enitiy
            _user = new User()
            {
                User_Id = Guid.Parse("8cbd0b6d-1437-4cee-8780-7c6fdc4b58c6"),
                FirstName = "milos",
                LastName = "mihajlovic",
                UserName = "milos123",
                //IsAdmin = true,
                BonusPoints=0
            };
            _users = new List<User>() { _user };

        }
        [TestMethod]
        public void AddUser_IfInsertUserReturnNull_ReturnNull()
        {
            //arrange
            Data.User nullUser = null;
            _usersRepository.Setup(x => x.Insert(It.IsAny<Data.User>())).Returns(nullUser);
            //act
            var result=_userService.AddUser(_userDomainModel).Result;
            //asser
            result.Should().BeNull();
        }
        [TestMethod]
        public void AddUser_Successful_ReturnUserDomainModel()
        {
            //arrange
            _usersRepository.Setup(x => x.Insert(It.IsAny<Data.User>())).Returns(_user);
            //act
            var result = _userService.AddUser(_userDomainModel);
            //asser
            result.Result.Should().BeEquivalentTo(_userDomainModel);
            
        }
        [TestMethod]
        public void GetAllAsync_ReturnNull()
        {
            //Arrange
            List<User> nullUsers = null;
            Task<List<User>> responseTask = Task.FromResult(nullUsers);
            _usersRepository.Setup(x => x.GetAll()).Returns(responseTask);
            //Act
            var resultAction = _userService.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            resultAction.Should().BeNull();

        }

        [TestMethod]
        public void GetAllAsync_ReturnListOfAuditoriums()
        {
            Task<List<User>> responseTask = Task.FromResult(_users);
            int expectedResultCount = 1;
            _usersRepository.Setup(x => x.GetAll()).Returns(responseTask);
            //act
            var resultAction = _userService.GetAllAsync().Result;
            var result = (List<UserDomainModel>)resultAction;
            //Assert
            Assert.AreEqual(expectedResultCount, result.Count);
            Assert.AreEqual(_user.User_Id, result[0].Id);
            result[0].Should().BeOfType<UserDomainModel>();
        }
        [TestMethod]
        public void GetByIdAsync_UserNotExist_ReturnNull()
        {
            //arrange
            User userDomain = null;
            Task<User> responseTask = Task.FromResult(userDomain);
            _usersRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(responseTask);
            //act
            var result = _userService.GetUserByIdAsync(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            result.Should().Be(null);
        }
        [TestMethod]
        public void GetByIdAsync_UserExist_ReturnUserDomainModel()
        {
            //arrange
            _usersRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(_user));
            //act
            var result = _userService.GetUserByIdAsync(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            result.Should().NotBe(null);
            result.Should().BeEquivalentTo(_userDomainModel);
        }
        [TestMethod]
        public void GetUserByUserName_UserNotExist_ReturnNull()
        {
            //arrange
            User userDomain = null;
            string userName = "milos123";
            _usersRepository.Setup(x => x.GetByUserName(It.IsAny<string>())).Returns(userDomain);
            //act
            var result = _userService.GetUserByUserName(userName).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            result.Should().Be(null);
        }
        [TestMethod]
        public void GetUserByUserName_UserExist_ReturnUserDomainModel()
        {
            //arrange
            string userName = "milos123";
            _usersRepository.Setup(x => x.GetByUserName(It.IsAny<string>())).Returns(_user);
            //act
            var result = _userService.GetUserByUserName(userName).ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            result.Should().NotBe(null);
            result.Should().BeEquivalentTo(_userDomainModel);
        }
        [TestMethod]
        public void AddBonusPoints_IfUserNotExist_ReturnNotFoundObjectResult()
        {
            //arrange
            ErrorResponseModels responseModel = new ErrorResponseModels()
            {
                ErrorMessage = Messages.USER_NOT_FOUND,
                StatusCode = System.Net.HttpStatusCode.NotFound
            };
            User nullUser=null;
            _usersRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(nullUser));
            //act
            var result = _userService.AddBonusPoint(Guid.NewGuid()).Result;
            var badRequestResult = (NotFoundObjectResult)result;
            //assert
            result.Should().BeOfType<NotFoundObjectResult>();
            badRequestResult.Value.Should().BeEquivalentTo(responseModel);
        }
        [TestMethod]
        public void AddBonusPoints_IfUserExist_ReturnTrue()
        {
            //arrange
            _usersRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(_user));
            _usersRepository.Setup(x => x.Update(It.IsAny<User>()));
            //act
            var result = _userService.AddBonusPoint(Guid.NewGuid());
            //assert
            result.Value.Should().Be(true);
        }
    }
}
