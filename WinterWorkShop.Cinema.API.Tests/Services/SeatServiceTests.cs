using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Data.Entities;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Domain.Services;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Tests.Services
{
    [TestClass]
    public class SeatServiceTests
    {
        private Mock<ISeatsRepository> _seatsRepository;
        private SeatService _seatService;
        private Seat _seat;
        private List<Seat> _seats;

        [TestInitialize]
        public void TestInitialize()
        {
            _seatsRepository = new Mock<ISeatsRepository>();
            _seatService = new SeatService(_seatsRepository.Object);

            _seat = new Seat()
            {
                Seat_Id = Guid.Parse("a1b6b0eb-2ec4-49d1-8c43-38c5468d390a"),
                Number = 5,
                Row = 5,
                Auditorium_Id = 1,
                Auditorium = new Auditorium(),
                ReservedSeats = new List<ReservedSeats>()
            };
            _seats = new List<Seat>() { _seat };
        }
        [TestMethod]
        public void GetAllAsync_ReturnNull()
        {
            //Arrange
            List<Seat> nullSeats = null;
            Task<List<Seat>> responseTask = Task.FromResult(nullSeats);
            _seatsRepository.Setup(x => x.GetAll()).Returns(responseTask);
            //Act
            var resultAction = _seatService.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            resultAction.Should().BeNull();

        }

        [TestMethod]
        public void GetAllAsync_ReturnListOfSeats()
        {
            Task<List<Seat>> responseTask = Task.FromResult(_seats);
            int expectedResultCount = 1;
            _seatsRepository.Setup(x => x.GetAll()).Returns(responseTask);
            //act
            var resultAction = _seatService.GetAllAsync().Result;
            var result = (List<SeatDomainModel>)resultAction;
            //Assert
            Assert.AreEqual(expectedResultCount, result.Count);
            Assert.AreEqual(_seat.Seat_Id, result[0].Id);
            result[0].Should().BeOfType<SeatDomainModel>();
        }
    }
}
