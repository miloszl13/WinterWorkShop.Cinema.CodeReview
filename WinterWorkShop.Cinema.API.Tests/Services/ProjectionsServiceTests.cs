using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
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
    public class ProjectionsServiceTests
    {
        private ProjectionService projectionsService;
        private Mock<IProjectionsRepository> _mockProjectionsRepository;
        private Mock<IMoviesRepository> _mockMoviesRepository;
        private Mock<IAuditoriumsRepository> _mockAudiitoriumsRepository;

        private Projection projection;
        private List<Projection> projections;

        private ProjectionDomainModel _projectionDomainModel;
        private List<ProjectionDomainModel> _projectionDomainModels;

        private Movie movie;

        private Auditorium auditorium;
        private List<Auditorium> auditoriums;
        //private ErrorResponseModel ProjectionForCinemaNotExist;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockProjectionsRepository = new Mock<IProjectionsRepository>();
            _mockMoviesRepository = new Mock<IMoviesRepository>();
            _mockAudiitoriumsRepository = new Mock<IAuditoriumsRepository>();
            projectionsService = new ProjectionService(_mockProjectionsRepository.Object, _mockMoviesRepository.Object, _mockAudiitoriumsRepository.Object);
            

            projection = new Projection
            {
                Projection_Id = Guid.Parse("8cbd0b6d-1437-4cee-8780-7c6fdc4b58c6"),
                Auditorium = new Auditorium { AuditName = "ImeSale" },
                Movie = new Movie { Title = "ImeFilma" },
                Movie_Id = Guid.Parse("a1b6b0eb-2ec4-49d1-8c43-38c5468d390a"),
                DateTime = DateTime.Parse("2022-04-30 11:07:29.3204088"),
                Auditorium_Id = 1
            };

            _projectionDomainModel = new ProjectionDomainModel
            {
                Id = Guid.Parse("8cbd0b6d-1437-4cee-8780-7c6fdc4b58c6"),
                AditoriumName = "ImeSale",
                AuditoriumId = 1,
                MovieId = Guid.Parse("a1b6b0eb-2ec4-49d1-8c43-38c5468d390a"),
                MovieTitle = "ImeFilma",
                ProjectionTime = DateTime.Parse("2022-04-30 11:07:29.3204088")
            };

            projections = new List<Projection>();
            projections.Add(projection);
            _projectionDomainModels = new List<ProjectionDomainModel>();
            _projectionDomainModels.Add(_projectionDomainModel);

            movie = new Movie()
            {
                Movie_Id = Guid.Parse("a1b6b0eb-2ec4-49d1-8c43-38c5468d390a"),
                Title = "testMovie",
                Current = false,
                Year = 2020,
                Rating = 9.0

            };

            //auditoriums
            auditorium = new Auditorium()
            {
                Auditorium_Id = 1,
                AuditName = "ImeSale",
                Cinema_Id = 1
            };
            auditoriums=new List<Auditorium>() { auditorium };
            //error response models
            //ProjectionForCinemaNotExist = new ErrorResponseModel()
            //{
            //    ErrorMessage = Messages.PROJECTION_IN_CINEMA_NOT_EXIST,
            //    StatusCode = System.Net.HttpStatusCode.NotFound
            //};
        }

        [TestMethod]
        public void GetAllAsync_ReturnNull()
        {
            //Arrange
            List<Projection> nullProjections = null;
            Task<List<Projection>> responseTask = Task.FromResult(nullProjections);
            _mockProjectionsRepository.Setup(x => x.GetAll()).Returns(responseTask);
            //Act
            var resultAction = projectionsService.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            //Assert
            resultAction.Should().BeNull();

        }

        [TestMethod]
        public void GetAllAsync_ReturnListOfProjecrions()
        {         
            Task<List<Projection>> responseTask = Task.FromResult(projections);
            int expectedResultCount = 1;
            _mockProjectionsRepository.Setup(x => x.GetAll()).Returns(responseTask);
            //act
            var resultAction = projectionsService.GetAllAsync().Result;
            var result = (List<ProjectionDomainModel>)resultAction;
            //Assert
            Assert.AreEqual(expectedResultCount, result.Count);
            Assert.AreEqual(projection.Projection_Id, result[0].Id);
            result[0].Should().BeOfType<ProjectionDomainModel>();
        }

        [TestMethod]
        public void CreateProjection_WithProjectionAtSameTime_ReturnErrorMessage() 
        {
            //Arrange
            string expectedMessage = "Cannot create new projection, there are projections at same time alredy.";
            _mockProjectionsRepository.Setup(x => x.GetByAuditoriumId(It.IsAny<int>())).Returns(projections);

            //Act
            var resultAction = projectionsService.CreateProjection(_projectionDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(expectedMessage, resultAction.ErrorMessage);
            Assert.IsFalse(resultAction.IsSuccessful);
        }

        
        [TestMethod]
        public void ProjectionService_CreateProjection_InsertMockedNull_ReturnErrorMessage()
        {
            //Arrange
            List<Projection> projectionsModelsList = new List<Projection>();
            projection = null;
            string expectedMessage = "Error occured while creating new projection, please try again.";

            _mockProjectionsRepository = new Mock<IProjectionsRepository>();
            _mockProjectionsRepository.Setup(x => x.GetByAuditoriumId(It.IsAny<int>())).Returns(projectionsModelsList);
            _mockProjectionsRepository.Setup(x => x.Insert(It.IsAny<Projection>())).Returns(projection);

            //Act
            var resultAction = projectionsService.CreateProjection(_projectionDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(expectedMessage, resultAction.ErrorMessage);
            Assert.IsFalse(resultAction.IsSuccessful);
        }

        // _projectionsRepository.GetByAuditoriumId(domainModel.AuditoriumId) mocked to return empty list
        // if (projectionsAtSameTime != null && projectionsAtSameTime.Count > 0) - false
        // _projectionsRepository.Insert(newProjection) mocked to return valid EntityEntry<Projection>
        //  if (insertedProjection == null) - false
        // return valid projection 
        [TestMethod]
        public void ProjectionService_CreateProjection_InsertMocked_ReturnProjection()
        {
            //Arrange
            _mockProjectionsRepository.Setup(x => x.GetByAuditoriumId(It.IsAny<int>()));
            _mockProjectionsRepository.Setup(x => x.Insert(It.IsAny<Projection>())).Returns(projection);
            _mockProjectionsRepository.Setup(x => x.Save());
            _mockMoviesRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(movie));
            _mockMoviesRepository.Setup(x => x.Update(It.IsAny<Movie>()));

            //Act
            var resultAction = projectionsService.CreateProjection(_projectionDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(projection.Projection_Id, resultAction.Projection.Id);
            Assert.IsNull(resultAction.ErrorMessage);
            Assert.IsTrue(resultAction.IsSuccessful);
        }
        [TestMethod]
        public void GetProjectionsByCinemaId_Successful_ReturnProjections()
        {
            //arrange
            _mockAudiitoriumsRepository.Setup(x => x.GetAll()).Returns(Task.FromResult(auditoriums));
            _mockProjectionsRepository.Setup(x => x.GetAll()).Returns(Task.FromResult(projections));
            //act
            var resultAction = projectionsService.GetProjectionsByCinemaId(1);
            //assert
            resultAction.Value.Should().BeEquivalentTo(_projectionDomainModels);
        }
        [TestMethod]
        public void GetProjectionsByCinemaId_ProjectionNotExistForThatCinema_ReturnNotFoundObjectResult()
        {
            //arrange
            _mockAudiitoriumsRepository.Setup(x => x.GetAll()).Returns(Task.FromResult(auditoriums));
            List<Projection> projections = new List<Projection>();
            _mockProjectionsRepository.Setup(x => x.GetAll()).Returns(Task.FromResult(projections));
            //act
            var resultAction = projectionsService.GetProjectionsByCinemaId(1);
            //assert
            resultAction.Result.Should().BeOfType<NotFoundObjectResult>();
        }
        [TestMethod]
        public void GetProjectionsByAuditorium_Successful_ReturnProjections()
        {
            //arrange
            _mockProjectionsRepository.Setup(x => x.GetAll()).Returns(Task.FromResult(projections));
            //act
            var resultAction = projectionsService.GetProjectionsByAuditorium(1);
            //assert
            resultAction.Value.Should().BeEquivalentTo(_projectionDomainModels);
        }
        [TestMethod]
        public void GetProjectionsByAuditorium_ProjectionNotExistForThatAuditorium_ReturnNotFoundObjectResult()
        {
            //arrange
            List<Projection> projections = new List<Projection>();
            _mockProjectionsRepository.Setup(x => x.GetAll()).Returns(Task.FromResult(projections));
            //act
            var resultAction = projectionsService.GetProjectionsByAuditorium(1);
            //assert
            resultAction.Result.Should().BeOfType<NotFoundObjectResult>();
        }
        [TestMethod]
        public void GetProjectionsByMovie_Successful_ReturnProjections()
        {
            //arrange
            _mockProjectionsRepository.Setup(x => x.GetAll()).Returns(Task.FromResult(projections));
            _mockMoviesRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(movie));
            //act
            var resultAction = projectionsService.GetProjectionsByMovie(Guid.Parse("a1b6b0eb-2ec4-49d1-8c43-38c5468d390a"));
            //assert
            resultAction.Value.Should().BeEquivalentTo(_projectionDomainModels);
        }
        [TestMethod]
        public void GetProjectionsByMovie_ProjectionNotExistForThatMovie_ReturnNotFoundObjectResult()
        {
            //arrange
            List<Projection> projections = new List<Projection>();
            _mockProjectionsRepository.Setup(x => x.GetAll()).Returns(Task.FromResult(projections));
            _mockMoviesRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(movie));

            //act
            var resultAction = projectionsService.GetProjectionsByMovie(Guid.Parse("a1b6b0eb-2ec4-49d1-8c43-38c5468d390a"));
            //assert
            resultAction.Result.Should().BeOfType<NotFoundObjectResult>();
        }
        [TestMethod]
        public void GetProjectionsByMovie_IfMovieDoesNotExist_ReturnNotFoundObjectResult()
        {
            //arrange
            Movie movie = null;
            _mockMoviesRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(movie));

            //act
            var resultAction = projectionsService.GetProjectionsByMovie(Guid.Parse("a1b6b0eb-2ec4-49d1-8c43-38c5468d390a"));
            //assert
            resultAction.Result.Should().BeOfType<NotFoundObjectResult>();
        }
        //datetime
        [TestMethod]
        public void GetProjectionsByDateTime_Successful_ReturnProjections()
        {
            //arrange
            _mockProjectionsRepository.Setup(x => x.GetAll()).Returns(Task.FromResult(projections));
            //act
            var resultAction = projectionsService.GetProjectionsByDateTime(DateTime.Parse("2022-04-30 11:07:29.3204088"));
            //assert
            resultAction.Value.Should().BeEquivalentTo(_projectionDomainModels);
        }
        [TestMethod]
        public void GetProjectionsByDateTime_ProjectionNotExistAtThatDateTime_ReturnNotFoundObjectResult()
        {
            //arrange
            List<Projection> projections = new List<Projection>();
            _mockProjectionsRepository.Setup(x => x.GetAll()).Returns(Task.FromResult(projections));
            //act
            var resultAction = projectionsService.GetProjectionsByDateTime(DateTime.Now);
            //assert
            resultAction.Result.Should().BeOfType<NotFoundObjectResult>();
        }

    }
}
