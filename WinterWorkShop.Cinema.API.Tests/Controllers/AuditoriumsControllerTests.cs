using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public class AuditoriumsControllerTests
        {
            private Mock<IAuditoriumService> _auditoriumService;
            private List<AuditoriumDomainModel> auditoriumDomainModelsList;
            AuditoriumDomainModel auditoriumDomainModel;
            AuditoriumsController controller;
            CreateAuditoriumModel createAuditoriumModel;
            CreateAuditoriumResultModel createAuditoriumResultModel;
            [TestInitialize]
            public void TestInitialize()
            {
                _auditoriumService = new Mock<IAuditoriumService>();
                auditoriumDomainModelsList = new List<AuditoriumDomainModel>();
                auditoriumDomainModel = new AuditoriumDomainModel()
                {
                    Id = 1,
                    Name = "sala1",
                    CinemaId = 1
                };
                auditoriumDomainModelsList.Add(auditoriumDomainModel);
                //controller setup
                controller = new AuditoriumsController(_auditoriumService.Object);

                createAuditoriumModel = new CreateAuditoriumModel()
                {
                    cinemaId = 1,
                    auditName = "sala1",
                    seatRows = 5,
                    numberOfSeats = 5
                };
                createAuditoriumResultModel = new CreateAuditoriumResultModel
                {
                    Auditorium = new AuditoriumDomainModel
                    {
                        Id = 1,
                        CinemaId = 1,
                        Name = createAuditoriumModel.auditName,
                        SeatsList = new List<SeatDomainModel>()
                    },
                    IsSuccessful = true
                };
            }
            [TestMethod]
            public void GetAsync_Return_AllAuditoriums()
            {
                //arrange
                _auditoriumService.Setup(x => x.GetAllAsync()).Returns(Task.FromResult(auditoriumDomainModelsList));
                int expectedResultCount = 1;
                int expectedStatusCode = 200;
                //act
                var result = controller.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
                var resultList = ((OkObjectResult)result).Value;
                var auditoriumResultList = (List<AuditoriumDomainModel>)resultList;
                //Assert
                Assert.IsNotNull(auditoriumResultList);
                Assert.AreEqual(expectedResultCount, auditoriumResultList.Count);
                Assert.AreEqual(auditoriumDomainModel.Id, auditoriumResultList[0].Id);
                Assert.IsInstanceOfType(result, typeof(OkObjectResult));
                Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
            }
            [TestMethod]
            public void GetAsync_Return_NewList()
            {
                //Arrange
                int expectedResultCount = 0;
                int expectedStatusCode = 200;
                List<AuditoriumDomainModel> auditoriums = new List<AuditoriumDomainModel>();
                _auditoriumService.Setup(x => x.GetAllAsync()).Returns(Task.FromResult(auditoriums));

                //Act
                var result = controller.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
                var resultList = ((OkObjectResult)result).Value;
                var auditoriumDomainModelResult = (List<AuditoriumDomainModel>)resultList;

                //Assert
                Assert.IsNotNull(auditoriumDomainModelResult);
                Assert.AreEqual(expectedResultCount, auditoriumDomainModelResult.Count);
                Assert.IsInstanceOfType(result, typeof(OkObjectResult));
                Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
            }
            // if (!ModelState.IsValid) - false
            // try  await _auditoriumService.CreateAuditorium(domainModel,int,int) - return valid mock
            // if (!createAuditoriumResultModel.IsSuccessful) - false
            // return Created
            [TestMethod]
            public void PostAsync_Create_createAuditoriumResultModel_IsSuccessful_True_Auditorium()
            {
                //Arrange
                int expectedStatusCode = 201;
                Task<CreateAuditoriumResultModel> responseTask = Task.FromResult(createAuditoriumResultModel);
                _auditoriumService.Setup(x => x.CreateAuditorium(It.IsAny<AuditoriumDomainModel>(), It.IsAny<int>(), It.IsAny<int>())).Returns(responseTask);

                //Act
                var result = controller.PostAsync(createAuditoriumModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
                var createdResult = ((CreatedResult)result).Value;
                //var auditoriumDomainModel = (AuditoriumDomainModel)createdResult;

                //Assert
                //Assert.IsNotNull(auditoriumDomainModel);
                Assert.AreEqual(createAuditoriumModel.cinemaId, createAuditoriumModel.cinemaId);
                Assert.IsInstanceOfType(result, typeof(CreatedResult));
                Assert.AreEqual(expectedStatusCode, ((CreatedResult)result).StatusCode);
            }
            // if (!ModelState.IsValid) - false
            // try  await _projectionService.CreateProjection(domainModel) - throw DbUpdateException
            // return BadRequest
            [TestMethod]
            public void PostAsync_Create_Throw_DbException_Auditorium()
            {
                //Arrange
                string expectedMessage = "Inner exception error message.";
                int expectedStatusCode = 400;

                Task<CreateAuditoriumResultModel> responseTask = Task.FromResult(createAuditoriumResultModel);
                Exception exception = new Exception("Inner exception error message.");
                DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);

                _auditoriumService.Setup(x => x.CreateAuditorium(It.IsAny<AuditoriumDomainModel>(), It.IsAny<int>(), It.IsAny<int>())).Throws(dbUpdateException);
                //Act
                var result = controller.PostAsync(createAuditoriumModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
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
            // try  await _auditoriumService.CreateAuditorium(domainModel,int,int) - return valid mock
            // if (!createAuditoriumResultModel.IsSuccessful) 
            // return BadRequest
            [TestMethod]
            public void PostAsync_Create_createAuditoriumResultModel_IsSuccessful_False_Return_BadRequest()
            {
                //Arrange
                string expectedMessage = "Error occured while creating new auditorium, please try again.";
                int expectedStatusCode = 400;

                createAuditoriumResultModel.IsSuccessful = false;
                createAuditoriumResultModel.ErrorMessage = Messages.AUDITORIUM_CREATION_ERROR;


                Task<CreateAuditoriumResultModel> responseTask = Task.FromResult(createAuditoriumResultModel);
                _auditoriumService.Setup(x => x.CreateAuditorium(It.IsAny<AuditoriumDomainModel>(), It.IsAny<int>(), It.IsAny<int>())).Returns(responseTask);
                //Act
                var result = controller.PostAsync(createAuditoriumModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
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
                controller.ModelState.AddModelError("key", "Invalid Model State");

                //Act
                var result = controller.PostAsync(createAuditoriumModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
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
        }
    }



       