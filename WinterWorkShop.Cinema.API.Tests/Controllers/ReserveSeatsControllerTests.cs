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

namespace WinterWorkShop.Cinema.Tests.Controllers
{
    [TestClass]
    public class ReserveSeatsControllerTests
    {
        private Mock<IReserveSeatsService> _reserveSeatsService;
        private ReservationsController _controller;
        private CreateReservationPassingModel createModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _reserveSeatsService = new Mock<IReserveSeatsService>();
            _controller = new ReservationsController(_reserveSeatsService.Object);
            //create model
            createModel = new CreateReservationPassingModel()
            {
                Id = 1,
                Projection_Id = Guid.Parse("5313d1b3-c0ba-43e4-9d94-56281a90fea3"),
                User_Id = Guid.Parse("4313d1b3-c0ba-43e4-9d94-56281a90fea3"),
                Seats = new List<Guid>() { Guid.Parse("1313d1b3-c0ba-43e4-9d94-56281a90fea3"),
                                                Guid.Parse("2313d1b3-c0ba-43e4-9d94-56281a90fea3") }
            };
        }
        //if seats are not in the same row
        [TestMethod]
        public void CreateReservation_SeatsAreNotInTheSameRow_ReturnsBadRequestObjectResult()
        {
            //arrange
            ErrorResponseModels SeatsAreNotInTheSameRow = new ErrorResponseModels()
            {
                ErrorMessage = Messages.SEATS_ARE_NOT_IN_THE_SAME_ROW,
                StatusCode = System.Net.HttpStatusCode.BadRequest
            };
            var expectedStatusCode = 400;
            _reserveSeatsService.Setup(x => x.ReserveSeat(It.IsAny<int>(), It.IsAny<List<Guid>>(),
                It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(new BadRequestObjectResult(SeatsAreNotInTheSameRow));
            //act
            var result=_controller.CreateReservation(createModel).Result;
            var badRequestResult = (BadRequestObjectResult)result;
            //assert
            result.Should().BeOfType<BadRequestObjectResult>();
            badRequestResult.Value.Should().BeEquivalentTo(SeatsAreNotInTheSameRow);
            badRequestResult.StatusCode.Should().Be(expectedStatusCode);
        }
        //if seats are in same row 
        [TestMethod]
        public void CreateReservation_SeatsAreNotOneNextToAnother_ReturnsBadRequestObjectResult()
        {
            //arrange
            ErrorResponseModels SeatsAreNotOneNextToAnother = new ErrorResponseModels()
            {
                ErrorMessage = Messages.SEATS_ARE_NOT_ONE_TO_ANOTHER,
                StatusCode = System.Net.HttpStatusCode.BadRequest
            };
            var expectedStatusCode = 400;
            _reserveSeatsService.Setup(x => x.ReserveSeat(It.IsAny<int>(), It.IsAny<List<Guid>>(),
                It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(new BadRequestObjectResult(SeatsAreNotOneNextToAnother));
            //act
            var result = _controller.CreateReservation(createModel).Result;
            var badRequestResult = (BadRequestObjectResult)result;
            //assert
            result.Should().BeOfType<BadRequestObjectResult>();
            badRequestResult.Value.Should().BeEquivalentTo(SeatsAreNotOneNextToAnother);
            badRequestResult.StatusCode.Should().Be(expectedStatusCode);
        }
        //if get all seats returns empty list
        [TestMethod]
        public void CreateReservation_ErrorOccuredWhileGettingAllSeats_ReturnsNotFoundObjectResult()
        {
            //arrange
            ErrorResponseModels GettingAllSeatsError = new ErrorResponseModels()
            {
                ErrorMessage = Messages.SEAT_GET_ALL_SEATS_ERROR,
                StatusCode = System.Net.HttpStatusCode.NotFound
            };
            var expectedStatusCode = 404;
            _reserveSeatsService.Setup(x => x.ReserveSeat(It.IsAny<int>(), It.IsAny<List<Guid>>(),
                It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(new NotFoundObjectResult(GettingAllSeatsError));
            //act
            var result = _controller.CreateReservation(createModel).Result;
            var badRequestResult = (NotFoundObjectResult)result;
            //assert
            result.Should().BeOfType<NotFoundObjectResult>();
            badRequestResult.Value.Should().BeEquivalentTo(GettingAllSeatsError);
            badRequestResult.StatusCode.Should().Be(expectedStatusCode);

        }
        //if projection does not exist
        [TestMethod]
        public void CreateReservation_ProjectionDoesNotExist_ReturnsNotFoundObjectResult()
        {
            //arrange
            ErrorResponseModels ProjectionNotExist = new ErrorResponseModels()
            {
                ErrorMessage = Messages.PROJECTION_DOES_NOT_EXIST,
                StatusCode = System.Net.HttpStatusCode.NotFound
            };
            var expectedStatusCode = 404;
            _reserveSeatsService.Setup(x => x.ReserveSeat(It.IsAny<int>(), It.IsAny<List<Guid>>(),
                It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(new NotFoundObjectResult(ProjectionNotExist));
            //act
            var result = _controller.CreateReservation(createModel).Result;
            var badRequestResult = (NotFoundObjectResult)result;
            //assert
            result.Should().BeOfType<NotFoundObjectResult>();
            badRequestResult.Value.Should().BeEquivalentTo(ProjectionNotExist);
            badRequestResult.StatusCode.Should().Be(expectedStatusCode);
        }
        //if any of passed seats is already reserved
        [TestMethod]
        public void CreateReservation_SomeOfPassedSeatsIsAlreadyReserved_ReturnsBadRequestObjectResult()
        {
            //arrange
            ErrorResponseModels SeatsAreReserved = new ErrorResponseModels()
            {
                ErrorMessage = Messages.SEAT_ALREADY_RESERVED,
                StatusCode = System.Net.HttpStatusCode.BadRequest
            };
            var expectedStatusCode = 400;
            _reserveSeatsService.Setup(x => x.ReserveSeat(It.IsAny<int>(), It.IsAny<List<Guid>>(),
                It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(new BadRequestObjectResult(SeatsAreReserved));
            //act
            var result = _controller.CreateReservation(createModel).Result;
            var badRequestResult = (BadRequestObjectResult)result;
            //assert
            result.Should().BeOfType<BadRequestObjectResult>();
            badRequestResult.Value.Should().BeEquivalentTo(SeatsAreReserved);
            badRequestResult.StatusCode.Should().Be(expectedStatusCode);
        }
        //make payment returns "Connection error."
        [TestMethod]
        public void CreateReservation_PaymentReturnConnectionError_ReturnsBadRequestObjectResult()
        {
            //arrange
            ErrorResponseModels PaymentError = new ErrorResponseModels()
            {
                ErrorMessage = "Connection error.",
                StatusCode = System.Net.HttpStatusCode.BadRequest
            };
            var expectedStatusCode = 400;
            _reserveSeatsService.Setup(x => x.ReserveSeat(It.IsAny<int>(), It.IsAny<List<Guid>>(),
                It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(new BadRequestObjectResult(PaymentError));
            //act
            var result = _controller.CreateReservation(createModel).Result;
            var badRequestResult = (BadRequestObjectResult)result;
            //assert
            result.Should().BeOfType<BadRequestObjectResult>();
            badRequestResult.Value.Should().BeEquivalentTo(PaymentError);
            badRequestResult.StatusCode.Should().Be(expectedStatusCode);
        }
        //make payment returns "Insufficient founds."
        [TestMethod]
        public void CreateReservation_PaymentReturnInsufficientfoundsError_ReturnsBadRequestObjectResult()
        {
            //arrange
            ErrorResponseModels PaymentError = new ErrorResponseModels()
            {
                ErrorMessage = "Insufficient founds.",
                StatusCode = System.Net.HttpStatusCode.BadRequest
            };
            var expectedStatusCode = 400;
            _reserveSeatsService.Setup(x => x.ReserveSeat(It.IsAny<int>(), It.IsAny<List<Guid>>(),
                It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(new BadRequestObjectResult(PaymentError));
            //act
            var result = _controller.CreateReservation(createModel).Result;
            var badRequestResult = (BadRequestObjectResult)result;
            //assert
            result.Should().BeOfType<BadRequestObjectResult>();
            badRequestResult.Value.Should().BeEquivalentTo(PaymentError);
            badRequestResult.StatusCode.Should().Be(expectedStatusCode);
        }
        //make payment returns "Payment is successful." - Successful reservation
        [TestMethod]
        public void CreateReservation_SuccessfulReservation_ReturnsTrue()
        {
            //arrange
            _reserveSeatsService.Setup(x => x.ReserveSeat(It.IsAny<int>(), It.IsAny<List<Guid>>(),
                It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(true);
            //act
            var result = _controller.CreateReservation(createModel);
            //assert
            result.Value.Should().Be(true);
        }
    }
}
