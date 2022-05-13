using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
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
    public class ProjectionsControllerTests
    {
        private ProjectionsController projectionsController;
        private Mock<IProjectionService> _projectionService;
        ErrorResponseModels projectionNotExistForCinema;
        ErrorResponseModels projectionNotExistForAuditorium;
        ErrorResponseModels projectionNotExistForMovie;
        ErrorResponseModels projectionNotExistAtTime;
        List<ProjectionDomainModel> projections;
        ProjectionDomainModel projectionDomainModel;
        CreateProjectionModel createProjectionModel;
        CreateProjectionResultModel createProjectionResultModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _projectionService = new Mock<IProjectionService>();
            projectionsController = new ProjectionsController(_projectionService.Object);
            //ErrorModels
            projectionNotExistForCinema = new ErrorResponseModels()
            {
                ErrorMessage = Messages.PROJECTION_IN_CINEMA_NOT_EXIST,
                StatusCode = System.Net.HttpStatusCode.NotFound
            };
            projectionNotExistForAuditorium= new ErrorResponseModels()
            {
                ErrorMessage = Messages.PROJECTION_IN_AUDITORIUM_NOT_EXIST,
                StatusCode = System.Net.HttpStatusCode.NotFound
            };
            projectionNotExistForMovie = new ErrorResponseModels()
            {
                ErrorMessage = Messages.PROJECTION_FOR_THAT_MOVIE_NOT_EXIST,
                StatusCode = System.Net.HttpStatusCode.NotFound
            };
            projectionNotExistAtTime = new ErrorResponseModels()
            {
                ErrorMessage = Messages.PROJECTION_FOR_THAT_TIME_NOT_EXIST,
                StatusCode = System.Net.HttpStatusCode.NotFound
            };
            //Domain Models
            projections = new List<ProjectionDomainModel>();
            projectionDomainModel = new ProjectionDomainModel
            {
                Id = Guid.NewGuid(),
                AditoriumName = "ImeSale",
                AuditoriumId = 1,
                MovieId = Guid.NewGuid(),
                MovieTitle = "ImeFilma",
                ProjectionTime = DateTime.Now.AddDays(1)
            };
            projections.Add(projectionDomainModel);
            //create models
            createProjectionModel = new CreateProjectionModel()
            {
                MovieId = Guid.NewGuid(),
                ProjectionTime = DateTime.Now.AddDays(1),
                AuditoriumId = 1
            };
            createProjectionResultModel = new CreateProjectionResultModel
            {
                Projection = new ProjectionDomainModel
                {
                    Id = Guid.NewGuid(),
                    AditoriumName = "ImeSale",
                    AuditoriumId = createProjectionModel.AuditoriumId,
                    MovieId = createProjectionModel.MovieId,
                    MovieTitle = "ImeFilma",
                    ProjectionTime = createProjectionModel.ProjectionTime
                },
                IsSuccessful = true
            };
        }

        [TestMethod]
        public void GetAsync_Return_All_Projections()
        {
            //Arrange
            
            List<ProjectionDomainModel> projectionDomainModels = projections;
            Task<List<ProjectionDomainModel>> responseTask = Task.FromResult(projectionDomainModels);
            int expectedResultCount = 1;
            int expectedStatusCode = 200;

            
            _projectionService.Setup(x => x.GetAllAsync()).Returns(responseTask);

            //Act
            var result = projectionsController.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var projectionDomainModelResultList = (List<ProjectionDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(projectionDomainModelResultList);
            Assert.AreEqual(expectedResultCount, projectionDomainModelResultList.Count);
            Assert.AreEqual(projectionDomainModel.Id, projectionDomainModelResultList[0].Id);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void GetAsync_Return_NewList()
        {
            //Arrange
            List<ProjectionDomainModel> projectionDomainModels = null;
            Task<List<ProjectionDomainModel>> responseTask = Task.FromResult(projectionDomainModels);
            int expectedResultCount = 0;
            int expectedStatusCode = 200;

            _projectionService.Setup(x => x.GetAllAsync()).Returns(responseTask);

            //Act
            var result = projectionsController.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var projectionDomainModelResultList = (List<ProjectionDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(projectionDomainModelResultList);
            Assert.AreEqual(expectedResultCount, projectionDomainModelResultList.Count);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }

        // if (!ModelState.IsValid) - false
        // if (projectionModel.ProjectionTime < DateTime.Now) - false
        // try  await _projectionService.CreateProjection(domainModel) - return valid mock
        // if (!createProjectionResultModel.IsSuccessful) - false
        // return Created
        [TestMethod]
        public void PostAsync_Create_createProjectionResultModel_IsSuccessful_True_Projection() 
        {
            //Arrange
            int expectedStatusCode = 201;            
            Task<CreateProjectionResultModel> responseTask = Task.FromResult(createProjectionResultModel);

            _projectionService.Setup(x => x.CreateProjection(It.IsAny<ProjectionDomainModel>())).Returns(responseTask);
            //Act
            var result = projectionsController.PostAsync(createProjectionModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var createdResult = ((CreatedResult)result).Value;
            var projectionDomainModel = (ProjectionDomainModel)createdResult;

            //Assert
            Assert.IsNotNull(projectionDomainModel);
            Assert.AreEqual(createProjectionModel.MovieId, projectionDomainModel.MovieId);
            Assert.IsInstanceOfType(result, typeof(CreatedResult));
            Assert.AreEqual(expectedStatusCode, ((CreatedResult)result).StatusCode);
        }

        // if (!ModelState.IsValid) - false
        // if (projectionModel.ProjectionTime < DateTime.Now) - false
        // try  await _projectionService.CreateProjection(domainModel) - throw DbUpdateException
        // return BadRequest
        [TestMethod]
        public void PostAsync_Create_Throw_DbException_Projection()
        {
            //Arrange
            string expectedMessage = "Inner exception error message.";
            int expectedStatusCode = 400;

            Task<CreateProjectionResultModel> responseTask = Task.FromResult(createProjectionResultModel);
            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);

            _projectionService.Setup(x => x.CreateProjection(It.IsAny<ProjectionDomainModel>())).Throws(dbUpdateException);

            //Act
            var result = projectionsController.PostAsync(createProjectionModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultResponse = (BadRequestObjectResult)result;
            var badObjectResult = ((BadRequestObjectResult)result).Value;
            var errorResult = (ErrorResponseModel)badObjectResult;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, errorResult.ErrorMessage);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }


        // if (!ModelState.IsValid) - false
        // if (projectionModel.ProjectionTime < DateTime.Now) - false
        // try  await _projectionService.CreateProjection(domainModel) - return valid mock
        // if (!createProjectionResultModel.IsSuccessful) - true
        // return BadRequest
        [TestMethod]
        public void PostAsync_Create_createProjectionResultModel_IsSuccessful_False_Return_BadRequest()
        {
            //Arrange
            string expectedMessage = "Error occured while creating new projection, please try again.";
            int expectedStatusCode = 400;

            createProjectionResultModel.IsSuccessful = false;
            createProjectionResultModel.ErrorMessage = Messages.PROJECTION_CREATION_ERROR;
            
            Task<CreateProjectionResultModel> responseTask = Task.FromResult(createProjectionResultModel);
            _projectionService.Setup(x => x.CreateProjection(It.IsAny<ProjectionDomainModel>())).Returns(responseTask);

            //Act
            var result = projectionsController.PostAsync(createProjectionModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultResponse = (BadRequestObjectResult)result;
            var badObjectResult = ((BadRequestObjectResult)result).Value;
            var errorResult = (ErrorResponseModel)badObjectResult;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, errorResult.ErrorMessage);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }

        // if (!ModelState.IsValid) - true
        // return BadRequest
        [TestMethod]
        public void PostAsync_With_UnValid_ModelState_Return_BadRequest()
        {
            //Arrange
            string expectedMessage = "Invalid Model State";
            int expectedStatusCode = 400;

            projectionsController.ModelState.AddModelError("key","Invalid Model State");

            //Act
            var result = projectionsController.PostAsync(createProjectionModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultResponse = (BadRequestObjectResult)result;
            var createdResult = ((BadRequestObjectResult)result).Value;
            var errorResponse = ((SerializableError)createdResult).GetValueOrDefault("key");
            var message = (string[])errorResponse;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, message[0]);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }

        // if (!ModelState.IsValid) - false
        // if (projectionModel.ProjectionTime < DateTime.Now) - true
        // return BadRequest
        [TestMethod]
        public void PostAsync_With_UnValid_ProjectionDate_Return_BadRequest()
        {
            //Arrange
            string expectedMessage = "Projection time cannot be in past.";
            int expectedStatusCode = 400;

            CreateProjectionModel createProjectionModel = new CreateProjectionModel()
            {
                MovieId = Guid.NewGuid(),
                ProjectionTime = DateTime.Now.AddDays(-1),
                AuditoriumId = 0
            };

          
            //Act
            var result = projectionsController.PostAsync(createProjectionModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultResponse = (BadRequestObjectResult)result;
            var createdResult = ((BadRequestObjectResult)result).Value;
            var errorResponse = ((SerializableError)createdResult).GetValueOrDefault(nameof(createProjectionModel.ProjectionTime));
            var message = (string[])errorResponse;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, message[0]);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }

        [TestMethod]
        public void GetProjectionsByCinema_IfThereAreZeroProjectionsInThatCinema_ReturnNotFoundObjectResult()
        {
            //arrange
            _projectionService.Setup(x => x.GetProjectionsByCinemaId(It.IsAny<int>())).Returns(new NotFoundObjectResult(projectionNotExistForCinema));
            //act
            var result = projectionsController.GetProjectionsByCinema(1);
            //assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }
        [TestMethod]
        public void GetProjectionsByCinema_CinemaId_ReturnProjection()
        {
            //arrange

            _projectionService.Setup(x => x.GetProjectionsByCinemaId(It.IsAny<int>())).Returns(projections);
            //act
            var result = projectionsController.GetProjectionsByCinema(1);
            //assert
            result.Value.Should().BeEquivalentTo(projections);
        }
        [TestMethod]
        public void GetProjectionsByAuditorium_IfThereAreZeroProjectionsInThatAuditorium_ReturnNotFoundObjectResult()
        {
            //arrange
            _projectionService.Setup(x => x.GetProjectionsByAuditorium(It.IsAny<int>())).Returns(new NotFoundObjectResult(projectionNotExistForAuditorium));
            //act
            var result = projectionsController.GetProjectionsByAuditorium(1);
            //assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }
        [TestMethod]
        public void GetProjectionsByAuditorium_AuditoriumId_ReturnProjection()
        {
            //arrange
            _projectionService.Setup(x => x.GetProjectionsByAuditorium(It.IsAny<int>())).Returns(projections);
            //act
            var result = projectionsController.GetProjectionsByAuditorium(1);
            //assert
            result.Value.Should().BeEquivalentTo(projections);
        }
        [TestMethod]
        public void GetProjectionsByMovie_IfThereAreZeroProjectionsForMovie_ReturnNotFoundObjectResult()
        {
            //arrange
            _projectionService.Setup(x => x.GetProjectionsByMovie(It.IsAny<Guid>())).Returns(new NotFoundObjectResult(projectionNotExistForMovie));
            //act
            var result = projectionsController.GetProjectionsByMovie(Guid.NewGuid());
            //assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }
        [TestMethod]
        public void GetProjectionsByMovie_MovieId_ReturnProjection()
        {
            //arrange
            _projectionService.Setup(x => x.GetProjectionsByMovie(It.IsAny<Guid>())).Returns(projections);
            //act
            var result = projectionsController.GetProjectionsByMovie(Guid.NewGuid());
            //assert
            result.Value.Should().BeEquivalentTo(projections);
        }
        [TestMethod]
        public void GetProjectionsByDateTime_IfThereAreZeroProjectionsAtThatTime_ReturnNotFoundObjectResult()
        {
            //arrange
            _projectionService.Setup(x => x.GetProjectionsByDateTime(It.IsAny<DateTime>())).Returns(new NotFoundObjectResult(projectionNotExistAtTime));
            //act
            var result = projectionsController.GetProjectionsByDateTime(DateTime.Now);
            //assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }
        [TestMethod]
        public void GetProjectionsByDateTime_DateTime_ReturnProjection()
        {
            //arrange
            _projectionService.Setup(x => x.GetProjectionsByDateTime(It.IsAny<DateTime>())).Returns(projections);
            //act
            var result = projectionsController.GetProjectionsByDateTime(DateTime.Now);
            //assert
            result.Value.Should().BeEquivalentTo(projections);
        }
    }
}
