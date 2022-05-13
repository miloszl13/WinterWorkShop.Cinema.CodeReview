using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Domain.Services;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Tests.Services
{
    [TestClass]
    public class AuditoriumsServiceTests
    {
        private Mock<IAuditoriumsRepository> _auditoriumsRepository;
        private Mock<ICinemasRepository> _cinemasRepository;
        private AuditoriumService _auditoriumService;
        private Auditorium auditorium;
        private List<Auditorium> auditoriums;
        private AuditoriumDomainModel _auditoriumDomainModel;
        private List<AuditoriumDomainModel> _auditoriumDomains;
        private Data.Cinema cinema;

        [TestInitialize]
        public void TestInitialize()
        {
            _auditoriumsRepository = new Mock<IAuditoriumsRepository>();
            _cinemasRepository = new Mock<ICinemasRepository>();
            _auditoriumService = new AuditoriumService(_auditoriumsRepository.Object,_cinemasRepository.Object);
            //models
            auditorium = new Auditorium()
            {
                Auditorium_Id = 1,
                AuditName = "Sala1",
                Cinema_Id = 1,
                Cinema = new Data.Cinema(),
                Projections = new List<Projection>(),
                Seats = new List<Seat>()
            };
            auditoriums=new List<Auditorium> { auditorium };
            //domain models
            _auditoriumDomainModel = new AuditoriumDomainModel()
            {
                Id = 1,
                Name = "Sala1",
                CinemaId = 1,
                SeatsList = new List<SeatDomainModel>()
            };
            _auditoriumDomains = new List<AuditoriumDomainModel>() { _auditoriumDomainModel };
            //cinema
            cinema = new Data.Cinema()
            {
                Cinema_Id = 1,
                Auditoriums = auditoriums,
                Name = "Cinema1"
            };
        }
        [TestMethod]
        public void GetAllAsync_ReturnNull()
        {
            //Arrange
            List<Auditorium> emptyList = new List<Auditorium>();
            Task<List<Auditorium>> responseTask = Task.FromResult(emptyList);
            _auditoriumsRepository.Setup(x => x.GetAll()).Returns(responseTask);
            //Act
            var resultAction = _auditoriumService.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            resultAction.Should().BeNull();

        }

        [TestMethod]
        public void GetAllAsync_ReturnListOfAuditoriums()
        {
            Task<List<Auditorium>> responseTask = Task.FromResult(auditoriums);
            int expectedResultCount = 1;
            _auditoriumsRepository.Setup(x => x.GetAll()).Returns(responseTask);
            //act
            var resultAction = _auditoriumService.GetAllAsync().Result;
            var result = (List<AuditoriumDomainModel>)resultAction;
            //Assert
            Assert.AreEqual(expectedResultCount, result.Count);
            Assert.AreEqual(auditorium.Auditorium_Id, result[0].Id);
            result[0].Should().BeOfType<AuditoriumDomainModel>();
        }
        [TestMethod]
        public void CreateAuditorium_IfCinemaDoesNotExist_ReturnErrorModel()
        {
            //arrange
            Data.Cinema cinema = null;
            string expectedMessage = "Cannot create new auditorium, cinema with given cinemaId does not exist.";
            _cinemasRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(cinema));
            //act
            var result=_auditoriumService.CreateAuditorium(_auditoriumDomainModel, 5, 5).Result;
            //assert
            result.Should().NotBeNull();
            expectedMessage.Should().BeEquivalentTo(result.ErrorMessage);
            result.IsSuccessful.Should().BeFalse();
        }
        [TestMethod]
        public void CreateAuditorium_AuditoriumAlreadyExistWithThatName_ReturnErrorModel()
        {
            //arrange
            string expectedMessage = "Cannot create new auditorium, auditorium with same name alredy exist."; ;
            _cinemasRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(cinema));
            _auditoriumsRepository.Setup(x => x.GetByAuditName(It.IsAny<string>(),It.IsAny<int>())).Returns(Task.FromResult(auditoriums));
            //act
            var result = _auditoriumService.CreateAuditorium(_auditoriumDomainModel, 5, 5).Result;
            //assert
            result.Should().NotBeNull();
            expectedMessage.Should().BeEquivalentTo(result.ErrorMessage);
            result.IsSuccessful.Should().BeFalse();
        }
        [TestMethod]
        public void CreateAuditorium_InsertAuditoriumReturnNull_ReturnErrorModel()
        {
            //arrange
            Auditorium auditorium = null;
            string expectedMessage = "Error occured while creating new auditorium, please try again.";
            _cinemasRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(cinema));
            _auditoriumsRepository.Setup(x => x.GetByAuditName(It.IsAny<string>(), It.IsAny<int>())).Returns(Task.FromResult(new List<Auditorium>()));
            _auditoriumsRepository.Setup(x => x.Insert(It.IsAny<Auditorium>())).Returns(auditorium);
            //act
            var result = _auditoriumService.CreateAuditorium(_auditoriumDomainModel, 5, 5).Result;
            //assert
            result.Should().NotBeNull();
            expectedMessage.Should().BeEquivalentTo(result.ErrorMessage);
            result.IsSuccessful.Should().BeFalse();
        }
        [TestMethod]
        public void CreateAuditorium_Successful_ReturnCreateAuditoriumResultModel()
        {
            //arrange
            _cinemasRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(cinema));
            _auditoriumsRepository.Setup(x => x.GetByAuditName(It.IsAny<string>(), It.IsAny<int>())).Returns(Task.FromResult(new List<Auditorium>()));
            _auditoriumsRepository.Setup(x => x.Insert(It.IsAny<Auditorium>())).Returns(auditorium);
            //act
            var result = _auditoriumService.CreateAuditorium(_auditoriumDomainModel, 5, 5).Result;
            //assert
            Assert.IsNotNull(result);
            Assert.AreEqual(auditorium.Auditorium_Id, result.Auditorium.Id);
            Assert.IsNull(result.ErrorMessage);
            Assert.IsTrue(result.IsSuccessful);
        }

    }


}
