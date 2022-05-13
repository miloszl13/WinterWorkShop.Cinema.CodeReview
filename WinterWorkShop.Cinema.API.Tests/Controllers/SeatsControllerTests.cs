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
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Tests.Controllers
{
    [TestClass]
    public class SeatsControllerTests
    {
        private Mock<ISeatService> _seatService;
        private SeatsController _controller;
        private List<SeatDomainModel> seats;
        private SeatDomainModel _seat;

        [TestInitialize]
        public void TestInitialize()
        {
            _seatService = new Mock<ISeatService>();
            _controller = new SeatsController(_seatService.Object);

            //domain models
            seats = new List<SeatDomainModel>();
            _seat = new SeatDomainModel()
            {
                Id = Guid.NewGuid(),
                AuditoriumId=1,
                Number=5,
                Row=5
            };
            seats.Add(_seat);
        }

        [TestMethod]
        public void GetAsync_EmptySeatTable_ReturnsEmptyList()
        {
            //arrange 
            int expectedResultCount = 0;
            int expectedStatusCode = 200;
            List<SeatDomainModel> seats = null;
            _seatService.Setup(x => x.GetAllAsync()).Returns(Task.FromResult(seats));
            //act 
            var result=_controller.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var seastDomainModelResult = (List<SeatDomainModel>)resultList;
            //assert
            expectedResultCount.Equals(seastDomainModelResult.Count());
            result.Should().BeOfType<OkObjectResult>();
            expectedStatusCode.Equals(((OkObjectResult)result).StatusCode);
        }
        [TestMethod]
        public void GetAsync_Return_AllSeats()
        {
            //Arrange
            Task<List<SeatDomainModel>> responseTask = Task.FromResult(seats);
            int expectedResultCount = 1;
            int expectedStatusCode = 200;

            _seatService.Setup(x => x.GetAllAsync()).Returns(responseTask);

            //Act
            var result = _controller.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var seatDomainModelList = (List<SeatDomainModel>)resultList;

            //Assert
            seatDomainModelList.Should().NotBeNull();
            expectedResultCount.Equals(seatDomainModelList.Count);
            _seat.Id.Equals(seatDomainModelList[0].Id);
            result.Should().BeOfType<OkObjectResult>();
            expectedStatusCode.Equals(((OkObjectResult)result).StatusCode);
        }
    }
}
