using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.API.Controllers;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Tests.Controllers
{
    [TestClass]
    public class CinemasControllerTests
    {
        private Mock<ICinemaService> _cinemaService;
        private CinemasController _controller;
        List<CinemaDomainModel> cinemas;
        CinemaDomainModel cinemaModelWithoutAuditorium;
        CinemaDomainModel cinemaModelWithAuditorium;
        ErrorResponseModel cinemaAlreadyExist;

        [TestInitialize]
        public void TestInitialize()
        {
            _cinemaService = new Mock<ICinemaService>();
            _controller = new CinemasController(_cinemaService.Object);
            //domain models
            cinemas = new List<CinemaDomainModel>();
            cinemaModelWithoutAuditorium= new CinemaDomainModel
            {
                Id=1,
                Name="CinemaTest"
            };
            cinemas.Add(cinemaModelWithoutAuditorium);
            cinemaModelWithAuditorium = new CinemaDomainModel()
            {
                Id = 2,
                Name = "CinemaTest1",
                auditName = "sala1",
                numberOfSeats = 50,
                seatRows = 10

            };
            //error message
            cinemaAlreadyExist = new ErrorResponseModel()
            {
                ErrorMessage = Messages.CINEMA_CREATE_ERROR,
                StatusCode = System.Net.HttpStatusCode.BadRequest
            };
        }
        [TestMethod]
        public void GetAsync_Return_All_Cinemas()
        {
            //Arrange
            Task<List<CinemaDomainModel>> responseTask = Task.FromResult(cinemas);
            int expectedResultCount = 1;
            int expectedStatusCode = 200;

            _cinemaService.Setup(x => x.GetAllAsync()).Returns(responseTask);

            //Act
            var result = _controller.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var cinemaDomainModels = (List<CinemaDomainModel>)resultList;

            //Assert
            cinemaDomainModels.Should().NotBeNull();
            expectedResultCount.Equals(cinemaDomainModels.Count);
            cinemaModelWithoutAuditorium.Id.Equals(cinemaDomainModels[0].Id);
            result.Should().BeOfType<OkObjectResult>();
            expectedStatusCode.Equals(((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void GetAsync_Return_NewList()
        {
            //Arrange
            List<CinemaDomainModel> cinemaDomainModels = null;
            Task<List<CinemaDomainModel>> responseTask = Task.FromResult(cinemaDomainModels);
            int expectedResultCount = 0;
            int expectedStatusCode = 200;

            _cinemaService.Setup(x => x.GetAllAsync()).Returns(responseTask);

            //Act
            var result = _controller.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var cinemaDomainModelResult = (List<CinemaDomainModel>)resultList;

            //Assert
            cinemaDomainModelResult.Should().NotBeNull();
            expectedResultCount.Equals(cinemaDomainModelResult.Count);
            result.Should().BeOfType<OkObjectResult>();
            expectedStatusCode.Equals(((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void Create_Succesfull_ReturnsTrue()
        {
            //arrange
            _cinemaService.Setup(x => x.Create(It.IsAny<CinemaDomainModel>())).Returns(true);
            //act
            var result= _controller.Create(cinemaModelWithoutAuditorium);
            //assert
            result.Value.Should().Be(true);
        }
        [TestMethod]
        public void Create_IfCinemaWithThatIdExist_ReturnsBadRequestObjectResult()
        {
            //arrange
            _cinemaService.Setup(x => x.Create(It.IsAny<CinemaDomainModel>())).Returns(new BadRequestObjectResult(cinemaAlreadyExist));
            //act
            var result = _controller.Create(cinemaModelWithoutAuditorium);
            //assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
            result.Result.Should().ToString().Contains("Error occured while creating new cinema, please try again");
        }
        [TestMethod]
        public void Create_InvalidAuditoriumSeatsValue_ReturnErrorMessage()
        {
            //arrange
            var expectedMessage = "The auditorium number of seats rows must be between 1-20.";
            _controller.ModelState.AddModelError("key", "The auditorium number of seats rows must be between 1-20.");

            //Act
            var result = _controller.Create(cinemaModelWithAuditorium);
            var resultResponse = (result).Result;
            var createdResult = ((BadRequestObjectResult)resultResponse).Value;
            var errorResponse = ((SerializableError)createdResult).GetValueOrDefault("key");
            var message = (string[])errorResponse;
            //Assert
            resultResponse.Should().NotBeNull();
            expectedMessage.Should().BeEquivalentTo(message[0]);
        }
        [TestMethod]
        public void Create_CreateAuditoriumReturnsError_ReturnBadRequestObjectResult()
        {
            //arrange
            int expectedStatusCode = 400;
            ErrorResponseModels AuditCreateError = new ErrorResponseModels()
            {
                ErrorMessage = "Error occured while creating new auditorium, please try again.",
                StatusCode = System.Net.HttpStatusCode.BadRequest
            };
            cinemaModelWithAuditorium.numberOfSeats = 10;
            _cinemaService.Setup(x => x.Create(It.IsAny<CinemaDomainModel>())).Returns(new BadRequestObjectResult(AuditCreateError));
            //Act
            var result = _controller.Create(cinemaModelWithAuditorium).Result;
            var badRequest= (BadRequestObjectResult)result;
            var errorResponse = (ErrorResponseModels)badRequest.Value;
            //assert
            result.Should().BeOfType<BadRequestObjectResult>();
            errorResponse.Should().BeEquivalentTo(AuditCreateError);
            badRequest.StatusCode.Should().Be(expectedStatusCode);
        }
        [TestMethod]
        public void Delete_IfCinemaDoesNotExist_ReturnsNotFoundObjectResult()
        {
            //arrange
            int expectedStatusCode = 404;
            ErrorResponseModels cinemaNotExist = new ErrorResponseModels()
            {
                ErrorMessage = Messages.CINEMA_NOT_EXIST,
                StatusCode = System.Net.HttpStatusCode.NotFound
            };
            _cinemaService.Setup(x => x.Delete(It.IsAny<int>())).Returns(new NotFoundObjectResult(cinemaNotExist));
            //Act
            var result = _controller.Delete(1).Result;
            var badRequest = (NotFoundObjectResult)result;
            var errorResponse = (ErrorResponseModels)badRequest.Value;
            //assert
            result.Should().BeOfType<NotFoundObjectResult>();
            errorResponse.Should().BeEquivalentTo(cinemaNotExist);
            badRequest.StatusCode.Should().Be(expectedStatusCode);
        }
        [TestMethod]
        public void Delete_Successful_ReturnsTrue()
        {
            //arrange
            int expectedStatusCode = 200;          
            _cinemaService.Setup(x => x.Delete(It.IsAny<int>())).Returns(new OkObjectResult(true));
            //Act
            var result = _controller.Delete(1).Result;
            var okObject = (OkObjectResult)result;
            //assert
            okObject.Value.Should().Be(true);
            okObject.StatusCode.Should().Be(expectedStatusCode);
        }
    }
}
