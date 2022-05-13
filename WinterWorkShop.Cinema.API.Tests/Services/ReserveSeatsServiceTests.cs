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
using WinterWorkShop.Cinema.Data.Entities;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Domain.Services;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Tests.Services
{
    [TestClass]
    public class ReserveSeatsServiceTests
    {
        private Mock<ISeatsRepository> _seatRepository;
        private Mock<IReservationRepository> _reservationRepository;
        private Mock<IProjectionsRepository> _projectionsRepository;
        private Mock<IReservedSeatsRepository> _reservedSeatsRepository;
        private Mock<IUserService> _userService;
        private Mock<ILevi9PaymentService> _payment;
        private ReserveSeatsService _service;
        private Seat seat1;
        private Seat seat2;
        private List<Seat> seats;
        private Projection projection;
        private PaymentResponse successfulPayment;
        private List<Guid> seatIds;
        private Guid userId;
        private Guid projectionId;

        [TestInitialize]
        public void TestInitialize()
        {
            _seatRepository = new Mock<ISeatsRepository>();
            _reservationRepository = new Mock<IReservationRepository>();
            _projectionsRepository = new Mock<IProjectionsRepository>();
            _reservedSeatsRepository = new Mock<IReservedSeatsRepository>();
            _userService=new Mock<IUserService>();
            _payment = new Mock<ILevi9PaymentService>();
            _service = new ReserveSeatsService(_seatRepository.Object,_reservationRepository.Object,_projectionsRepository.Object,_reservedSeatsRepository.Object,_payment.Object,_userService.Object);
            //seats
            seat1 = new Seat()
            {
                Seat_Id = Guid.Parse("1313d1b3-c0ba-43e4-9d94-56281a90fea3"),
                Auditorium_Id = 1,
                Number = 1,
                Row = 1
            };
            seat2 = new Seat()
            {
                Seat_Id = Guid.Parse("2313d1b3-c0ba-43e4-9d94-56281a90fea3"),
                Auditorium_Id = 1,
                Number = 2,
                Row = 1

            };
            seats=new List<Seat>() { seat1,seat2};
            //projection
            projection = new Projection()
            {
                Projection_Id = Guid.Parse("2313d1b3-c0ba-43e4-9d94-56281a90fea3"),
                Auditorium_Id = 1,
                Auditorium = new Auditorium { Auditorium_Id = 1 },
                Movie_Id = Guid.Parse("a1b6b0eb-2ec4-49d1-8c43-38c5468d390a"),
                Price = 300,
                Movie = new Movie() { Movie_Id = Guid.Parse("a1b6b0eb-2ec4-49d1-8c43-38c5468d390a") },
                DateTime = DateTime.Parse("2022-04-30 11:07:29.3204088")
            };
            //payment response
            successfulPayment= new PaymentResponse
            {
                 IsSuccess = true,
                 Message = "Payment is successful."
             };
            //parameters for methods
            seatIds = new List<Guid>() { Guid.Parse("1313d1b3-c0ba-43e4-9d94-56281a90fea3"),
                                                Guid.Parse("2313d1b3-c0ba-43e4-9d94-56281a90fea3") };
            userId = Guid.Parse("4313d1b3-c0ba-43e4-9d94-56281a90fea3");
            projectionId = Guid.Parse("5313d1b3-c0ba-43e4-9d94-56281a90fea3");

        }
        //if seats are not in the same row
        [TestMethod]
        public void ReserveSeat_SeatsAreNotInTheSameRow_ReturnBadRequestObjectResult()
        {
            //arrange
            ErrorResponseModels SeatsAreNotInTheSameRow = new ErrorResponseModels()
            {
                ErrorMessage = Messages.SEATS_ARE_NOT_IN_THE_SAME_ROW,
                StatusCode = System.Net.HttpStatusCode.BadRequest
            };
            seat2.Row = 2;            
            _seatRepository.Setup(x => x.GetAll()).Returns(Task.FromResult(seats));           
            //act
            var result = _service.ReserveSeat(1, seatIds, userId, projectionId).Result;
            var BadRequestResult = (BadRequestObjectResult)result;
            //assert
            result.Should().BeOfType<BadRequestObjectResult>();
            BadRequestResult.Value.Should().BeEquivalentTo(SeatsAreNotInTheSameRow);
        }
        //if seats are not one next to another

        [TestMethod]
        public void ReserveSeat_SeatsAreNotOneNextToAnother_ReturnBadRequestObjectResult()
        {
            //arrange
            ErrorResponseModels SeatsAreNotOneNextToAnother = new ErrorResponseModels()
            {
                ErrorMessage = Messages.SEATS_ARE_NOT_ONE_TO_ANOTHER,
                StatusCode = System.Net.HttpStatusCode.BadRequest
            };
            seat2.Number = 3;
            _seatRepository.Setup(x => x.GetAll()).Returns(Task.FromResult(seats));           
            //act
            var result = _service.ReserveSeat(1, seatIds, userId, projectionId).Result;
            var BadRequestResult = (BadRequestObjectResult)result;
            //assert
            result.Should().BeOfType<BadRequestObjectResult>();
            BadRequestResult.Value.Should().BeEquivalentTo(SeatsAreNotOneNextToAnother);
        }
        //if get all seats returns empty list
        [TestMethod]
        public void ReserveSeat_GetAllSeatsReturnsEmptyList_ReturnNotFoundObjectResult()
        {
            //arrange
            ErrorResponseModels GetAllSeatsError = new ErrorResponseModels()
            {
                ErrorMessage = Messages.SEAT_GET_ALL_SEATS_ERROR,
                StatusCode = System.Net.HttpStatusCode.NotFound
            };
            _seatRepository.Setup(x => x.GetAll()).Returns(Task.FromResult(new List<Seat>()));
            //act
            var result = _service.ReserveSeat(1, seatIds, userId, projectionId).Result;
            var NotFoundResult = (NotFoundObjectResult)result;
            //assert
            result.Should().BeOfType<NotFoundObjectResult>();
            NotFoundResult.Value.Should().BeEquivalentTo(GetAllSeatsError);
        }
        //if projection does not exist
        [TestMethod]
        public void ReserveSeat_ProjectionDoesNotExist_ReturnNotFoundObjectResult()
        {
            //arrange
            ErrorResponseModels ProjectionNotExist = new ErrorResponseModels()
            {
                ErrorMessage = Messages.PROJECTION_DOES_NOT_EXIST,
                StatusCode = System.Net.HttpStatusCode.NotFound
            };
            Projection projection = null;
            _seatRepository.Setup(x => x.GetAll()).Returns(Task.FromResult(seats));
            _projectionsRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(projection));
            //act
            var result = _service.ReserveSeat(1, seatIds, userId, projectionId).Result;
            var BadRequestResult = (NotFoundObjectResult)result;
            //assert
            result.Should().BeOfType<NotFoundObjectResult>();
            BadRequestResult.Value.Should().BeEquivalentTo(ProjectionNotExist);
        }
        //if any of passed seats is already reserved
        [TestMethod]
        public void ReserveSeat_IfAnySeatIsALreadyReserved_ReturnBadRequestObjectResult()
        {
            //arrange
            ErrorResponseModels SeatsAreReserved = new ErrorResponseModels()
            {
                ErrorMessage = Messages.SEAT_ALREADY_RESERVED,
                StatusCode = System.Net.HttpStatusCode.BadRequest
            };
            ReservedSeats reservedSeat = new ReservedSeats()
            {
                Reservation_Id = 1,
                Seat_Id = Guid.Parse("1313d1b3-c0ba-43e4-9d94-56281a90fea3"),
                DateTime = DateTime.Parse("2022-04-30 11:07:29.3204088"),

            };
            List<ReservedSeats> reservedSeats = new List<ReservedSeats>() { reservedSeat};
            _seatRepository.Setup(x => x.GetAll()).Returns(Task.FromResult(seats));
            _projectionsRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(projection));
            _reservedSeatsRepository.Setup(x => x.GetAll()).Returns(Task.FromResult(reservedSeats));
            //act
            var result = _service.ReserveSeat(1, seatIds, userId, projectionId).Result;
            var BadRequestResult = (BadRequestObjectResult)result;
            //assert
            result.Should().BeOfType<BadRequestObjectResult>();
            BadRequestResult.Value.Should().BeEquivalentTo(SeatsAreReserved);
        }
        //make payment returns "Connection error."
        [TestMethod]
        public void ReserveSeat_IfPaymentReturnsConnectionError_ReturnBadRequestObjectResult()
        {
            //arrange
            PaymentResponse paymentResponse=new PaymentResponse
            {
                IsSuccess = false,
                Message = "Connection error."
            };
            ErrorResponseModels PaymentError = new ErrorResponseModels()
            {
                ErrorMessage = "Connection error.",
                StatusCode = System.Net.HttpStatusCode.BadRequest
            };           
            List<ReservedSeats> ResSeats = new List<ReservedSeats>();
            _seatRepository.Setup(x => x.GetAll()).Returns(Task.FromResult(seats));
            _projectionsRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(projection));
            _reservedSeatsRepository.Setup(x => x.GetAll()).Returns(Task.FromResult(ResSeats));
            _payment.Setup(x => x.MakePayment()).Returns(Task.FromResult(paymentResponse));
            //act
            var result = _service.ReserveSeat(1, seatIds, userId, projectionId).Result;
            var BadRequestResult = (BadRequestObjectResult)result;
            //assert
            result.Should().BeOfType<BadRequestObjectResult>();
            BadRequestResult.Value.Should().BeEquivalentTo(PaymentError);
        }
        //make payment returns "Insufficient founds."
        [TestMethod]
        public void ReserveSeat_PaymentReturnsInsufficientFounds_ReturnBadRequestObjectResult()
        {
            //arrange
            PaymentResponse paymentResponse = new PaymentResponse
            {
                IsSuccess = false,
                Message = "Insufficient founds."
            };
            ErrorResponseModels PaymentError = new ErrorResponseModels()
            {
                ErrorMessage = "Insufficient founds.",
                StatusCode = System.Net.HttpStatusCode.BadRequest
            };
            List<ReservedSeats> ResSeats = new List<ReservedSeats>();
            _seatRepository.Setup(x => x.GetAll()).Returns(Task.FromResult(seats));
            _projectionsRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(projection));
            _reservedSeatsRepository.Setup(x => x.GetAll()).Returns(Task.FromResult(ResSeats));
            _payment.Setup(x => x.MakePayment()).Returns(Task.FromResult(paymentResponse));
            //act
            var result = _service.ReserveSeat(1, seatIds, userId, projectionId).Result;
            var BadRequestResult = (BadRequestObjectResult)result;
            //assert
            result.Should().BeOfType<BadRequestObjectResult>();
            BadRequestResult.Value.Should().BeEquivalentTo(PaymentError);
        }
        //make payment returns "Payment is successful." - Successful reservation
        [TestMethod]
        public void ReserveSeat_SuccessfulReservation_ReturnTrue()
        {
            //arrange
            PaymentResponse paymentResponse = new PaymentResponse
            {
                IsSuccess = true,
                Message = "Payment is successful."
            };           
            List<ReservedSeats> ResSeats = new List<ReservedSeats>();
            _seatRepository.Setup(x => x.GetAll()).Returns(Task.FromResult(seats));
            _projectionsRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(projection));
            _reservedSeatsRepository.Setup(x => x.GetAll()).Returns(Task.FromResult(ResSeats));
            _payment.Setup(x => x.MakePayment()).Returns(Task.FromResult(paymentResponse));
            _reservationRepository.Setup(x => x.Insert(It.IsAny<Reservation>()));
            _reservedSeatsRepository.Setup(x => x.Insert(It.IsAny<ReservedSeats>()));
            _userService.Setup(x => x.AddBonusPoint(It.IsAny<Guid>()));
            //act
            var result = _service.ReserveSeat(1, seatIds, userId, projectionId);
            //assert
            result.Value.Should().Be(true);
            
        }
    }
}

