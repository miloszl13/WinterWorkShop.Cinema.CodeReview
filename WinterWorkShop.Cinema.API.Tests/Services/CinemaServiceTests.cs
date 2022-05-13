using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Domain.Services;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Tests.Services
{
    [TestClass]
    public class CinemaServiceTests
    {
        private Mock<ICinemasRepository> _cinemasRepository;
        private Mock<IAuditoriumService> _auditoriumService;
        private CinemaService _cinemaService;
        private CinemaDomainModel cinemaDomainModel;
        private List<Data.Cinema> cinemas;
        private Data.Cinema cinema;
        private CreateAuditoriumResultModel auditoriumResultModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _cinemasRepository = new Mock<ICinemasRepository>();
            _auditoriumService = new Mock<IAuditoriumService>();
            _cinemaService = new CinemaService(_cinemasRepository.Object, _auditoriumService.Object);
            //domain model
            cinemaDomainModel = new CinemaDomainModel()
            {
                Name = "Cinema1",
                Id = 1
            };
            //cinema
            cinema = new Data.Cinema()
            {
                Cinema_Id = 1,
                Name = "Cinema1",
                Auditoriums = new List<Data.Auditorium>()
            };
            cinemas = new List<Data.Cinema>() { cinema };
            //auditoriumResultModel
            auditoriumResultModel = new CreateAuditoriumResultModel()
            {
                ErrorMessage = Messages.AUDITORIUM_CREATION_ERROR,
                IsSuccessful = false
            };
        }
        [TestMethod]
        public void Create_InsertThrowsException_ReturnBadRequestObjectResult()
        {
            //arrange 
            ErrorResponseModel error = new ErrorResponseModel()

            {
                ErrorMessage = "Error occured while creating new cinema, please try again",
                StatusCode = HttpStatusCode.BadRequest
            };
            
            Exception exception = new Exception();
            _cinemasRepository.Setup(x => x.Insert(It.IsAny<Data.Cinema>())).Throws(exception);
            string expectedMessage = "Error occured while creating new cinema, please try again";
            //act
            var result=_cinemaService.Create(cinemaDomainModel);
            var BadObject=result.Result.As<BadRequestObjectResult>();
            //assert
            result.Should().NotBeNull();
            result.Result.Should().BeOfType<BadRequestObjectResult>();
            BadObject.Value.Should().BeEquivalentTo(error);
        }
        [TestMethod]
        public void Create_Successful_ReturnTrue()
        {
            //arrange
            _cinemasRepository.Setup(x => x.Insert(It.IsAny<Data.Cinema>()));
            //act
            var result = _cinemaService.Create(cinemaDomainModel);
            //assert
            result.Value.Should().Be(true);
            
        }
        [TestMethod]
        public void Create_CreateAuditoriumReturnsError_ReturnNotFoundObjectResult()
        {
            //arrange 
            ErrorResponseModels AuditCreateError = new ErrorResponseModels()
            {
                ErrorMessage = "Error occured while creating new auditorium, please try again.",
                StatusCode = System.Net.HttpStatusCode.BadRequest
            };
            cinemaDomainModel.auditName = "sala1";
            cinemaDomainModel.seatRows = 10;
            cinemaDomainModel.numberOfSeats = 10;
            _auditoriumService.Setup(x => x.CreateAuditorium(It.IsAny<AuditoriumDomainModel>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(auditoriumResultModel));
            //act
            var result = _cinemaService.Create(cinemaDomainModel).Result;
            var BadObject = (BadRequestObjectResult)result;
            var errorResponse = (ErrorResponseModels)BadObject.Value;
            //assert
            result.Should().BeOfType<BadRequestObjectResult>();
            errorResponse.Should().BeEquivalentTo(AuditCreateError);
        }
        [TestMethod]
        public void GetAllAsync_ReturnNull()
        {
            //Arrange
            List<Data.Cinema> nullCinema = null;
            Task<List<Data.Cinema>> responseTask = Task.FromResult(nullCinema);
            _cinemasRepository.Setup(x => x.GetAll()).Returns(responseTask);
            //Act
            var resultAction = _cinemaService.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            resultAction.Should().BeNull();

        }

        [TestMethod]
        public void GetAllAsync_ReturnListOfCinemas()
        {
            Task<List<Data.Cinema>> responseTask = Task.FromResult(cinemas);
            int expectedResultCount = 1;
            _cinemasRepository.Setup(x => x.GetAll()).Returns(responseTask);
            //act
            var resultAction = _cinemaService.GetAllAsync().Result;
            var result = (List<CinemaDomainModel>)resultAction;
            //Assert
            Assert.AreEqual(expectedResultCount, result.Count);
            Assert.AreEqual(cinema.Cinema_Id, result[0].Id);
            result[0].Should().BeOfType<CinemaDomainModel>();
        }
        [TestMethod]
        public void Delete_Successful_ReturnTrue()
        {
            //arrange
            _cinemasRepository.Setup(x => x.Delete(It.IsAny<int>())).Returns(new Data.Cinema());
            //act
            var result = _cinemaService.Delete(1);
            var okObject = (OkObjectResult)result.Result;
            //assert
            result.Result.Should().BeOfType<OkObjectResult>();
            okObject.Value.Should().Be(true);
        }
        [TestMethod]
        public void Delete_ifCinemaDoesNotExist_ReturnNotFoundObjectResult()
        {
            //arrange 
            ErrorResponseModels cinemaNotExist = new ErrorResponseModels()
            {
                ErrorMessage = Messages.CINEMA_NOT_EXIST,
                StatusCode = System.Net.HttpStatusCode.NotFound
            };
            Data.Cinema nullCinema = null;
            _cinemasRepository.Setup(x => x.Delete(It.IsAny<int>())).Returns(nullCinema);
            //act
            var result = _cinemaService.Delete(1).Result;
            var notFound = (NotFoundObjectResult)result;
            var errorResponse = (ErrorResponseModels)notFound.Value;
            //assert
            result.Should().BeOfType<NotFoundObjectResult>();
            errorResponse.Should().BeEquivalentTo(cinemaNotExist);
        }
    }
}
