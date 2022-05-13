using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
    public class MoviesControllerTests
    {
        private Mock<IMovieService> _movieService;
        private MoviesController _controller;
        private Mock<ILogger<MoviesController>> _logger;
        private MovieDomainModel movieDomainModel;
        private List<MovieDomainModel> movies;
        private MovieModel movieModel;
        private ErrorResponseModel MovieNotExist;
        private ErrorResponseModel MovieHasProjectionsInFuture;
        private ErrorResponseModel MovieDoesNotHaveProjections;

        [TestInitialize]
        public void TestInitialize()
        {
            _movieService = new Mock<IMovieService>();
            _logger = new Mock<ILogger<MoviesController>>();
            _controller = new MoviesController(_logger.Object,_movieService.Object);
            //movie domain model
            movieDomainModel = new MovieDomainModel()
            {
                Id = Guid.NewGuid(),
                Rating = 9.1,
                Year = 2020,
                Title = "Superman",
                Current = true
            };
            movies = new List<MovieDomainModel>();
            movies.Add(movieDomainModel);
            //movie  model
            movieModel = new MovieModel()
            {
                Title = "Superman",
                Year = 2020,
                Current = true,
                Rating = 9.1
            };
            //error response model
            MovieNotExist = new ErrorResponseModel()
            {
                ErrorMessage = Messages.MOVIE_DOES_NOT_EXIST,
                StatusCode = System.Net.HttpStatusCode.NotFound
            };
            MovieHasProjectionsInFuture = new ErrorResponseModel()
            {
                ErrorMessage = Messages.MOVIE_CANNOT_DEACTIVATE_MOVIE,
                StatusCode = System.Net.HttpStatusCode.NotFound
            };
            MovieDoesNotHaveProjections = new ErrorResponseModel()
            {
                ErrorMessage = Messages.MOVIE_CANNOT_ACTIVATE_MOVIE,
                StatusCode = System.Net.HttpStatusCode.NotFound
            };

        }
        [TestMethod]
        public void GetAsync_Id_ReturnsMovieDomainModel()
        {
            //arrange
            int expectedStatusCode = 200;
            _movieService.Setup(x => x.GetMovieByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(movieDomainModel));
            //act
            var result=_controller.GetAsync(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            //assert
            var resultList = ((OkObjectResult)result).Value;
            var movieDomainModelResult = (MovieDomainModel)resultList;

            //Assert
            movieDomainModelResult.Should().NotBeNull();
            movieDomainModelResult.Id.Equals(movieDomainModel.Id);
            result.Should().BeOfType<OkObjectResult>();
            expectedStatusCode.Equals(((OkObjectResult)result).StatusCode);
        }
        [TestMethod]
        public void GetAsync_MovieWithIdNotExist_ReturnsNotFoundObjectResult()
        {
            //arrange
            int expectedStatusCode = 404;
            MovieDomainModel movieModel = null;
            _movieService.Setup(x => x.GetMovieByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(movieModel));
            //act
            var result = _controller.GetAsync(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            //assert
            var resultList = ((NotFoundObjectResult)result).Value;

            //Assert
            resultList.ToString().Should().Contain("Movie does not exist.");
            result.Should().BeOfType<NotFoundObjectResult>();
            expectedStatusCode.Equals(((NotFoundObjectResult)result).StatusCode);
        }
        [TestMethod]
        public void GetAsync_CurrentIsTrue_ReturnMovies()
        {
            //Arrange
            int expectedResultCount = 1;
            int expectedStatusCode = 200;

            _movieService.Setup(x => x.GetAllMovies()).Returns(movies);

            //Act
            var result = _controller.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var movieDomainModels = (List<MovieDomainModel>)resultList;

            //Assert
            movieDomainModels.Should().NotBeNull();
            expectedResultCount.Equals(movieDomainModels.Count);
            movieDomainModel.Id.Equals(movieDomainModels[0].Id);
            result.Should().BeOfType<OkObjectResult>();
            expectedStatusCode.Equals(((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void GetAsync_NoMoviesWithCurrentEqualTrue_ReturnsNewList()
        {
            //Arrange
            List<MovieDomainModel> cinemaDomainModels = null;
            
            int expectedResultCount = 0;
            int expectedStatusCode = 200;

            _movieService.Setup(x => x.GetAllMovies()).Returns(cinemaDomainModels);

            //Act
            var result = _controller.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var movieDomainModelResult = (List<MovieDomainModel>)resultList;

            //Assert
            expectedResultCount.Equals(movieDomainModelResult.Count);
            result.Should().BeOfType<OkObjectResult>();
            expectedStatusCode.Equals(((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void Post_Create_createMovieResultModel_IsSuccessful_True_ReturnMovie()
        {
            //Arrange
            int expectedStatusCode = 201;
            Task<MovieDomainModel> responseTask = Task.FromResult(movieDomainModel);
            _movieService.Setup(x => x.AddMovie(It.IsAny<MovieDomainModel>())).Returns(responseTask);

            //Act
            var result = _controller.Post(movieModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var createdResult = ((CreatedResult)result).Value;
            var movieDomainModelResult = (MovieDomainModel)createdResult;

            //Assert
            movieDomainModelResult.Should().NotBeNull();
            result.Should().BeOfType<CreatedResult>();
            expectedStatusCode.Equals(((CreatedResult)result).StatusCode).Should().BeTrue();
        }
        [TestMethod]
        public void Post_Create_Throw_DbException_Movie()
        {
            //Arrange
            string expectedMessage = "Inner exception error message.";
            int expectedStatusCode = 400;

            Task<MovieModel> responseTask = Task.FromResult(movieModel);
            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);

            _movieService.Setup(x => x.AddMovie(It.IsAny<MovieDomainModel>())).Throws(dbUpdateException);
            //Act
            var result = _controller.Post(movieModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = (BadRequestObjectResult)result;
            var badObjectResult = ((BadRequestObjectResult)result).Value;
            var errorResult = (ErrorResponseModel)badObjectResult;

            //Assert
            resultResponse.Should().NotBeNull();
            expectedMessage.Should().BeEquivalentTo(errorResult.ErrorMessage);
            result.Should().BeOfType<BadRequestObjectResult>();
            expectedStatusCode.Should().Be(resultResponse.StatusCode);
          
        }
        [TestMethod]
        public void Post_ServiceAddMovieReturnsNull_DbException_Movie()
        {
            //Arrange
            string expectedMessage = "Error occured while creating new movie, please try again.";
            int expectedStatusCode = 500;
            
            MovieDomainModel movieNullModel = null;
            Task<MovieDomainModel> responseTask = Task.FromResult(movieNullModel);
            

            _movieService.Setup(x => x.AddMovie(It.IsAny<MovieDomainModel>())).Returns(responseTask);
            //Act
            var result = _controller.Post(movieModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = (ObjectResult)result;
            var ObjectResult = ((ObjectResult)result).Value;
            var errorResult = (ErrorResponseModel)ObjectResult;

            //Assert
            //resultResponse.Should().NotBeNull();
            expectedMessage.Should().BeEquivalentTo(errorResult.ErrorMessage);
            expectedStatusCode.Should().Be(resultResponse.StatusCode);

        }
        [TestMethod]
        public void Post_With_UnValid_ModelState_Return_BadRequest()
        {
            //Arrange
            string expectedMessage = "Invalid Model State";
            int expectedStatusCode = 400;
            _controller.ModelState.AddModelError("key", "Invalid Model State");

            //Act
            var result = _controller.Post(movieModel).ConfigureAwait(false).GetAwaiter().GetResult();
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

        [TestMethod]
        public void Put_IfMovieDoesNotExist_ReturnBadRequest()
        {
            //arrange
            string expectedMessage = "Movie does not exist.";
            MovieDomainModel movieNullModel = null;
            _movieService.Setup(x => x.GetMovieByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(movieNullModel));
            //act
            var result = _controller.Put(Guid.NewGuid(), movieModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var errorResponse=((BadRequestObjectResult)result).Value;
            var errorResult = (ErrorResponseModel)errorResponse;
            //assert
            result.Should().BeOfType<BadRequestObjectResult>();
            errorResult.ErrorMessage.Should().Be(expectedMessage);
        }
        [TestMethod]
        public void Put_Successful_ReturnAcceptedResult()
        {
            //arrange
            _movieService.Setup(x => x.GetMovieByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(movieDomainModel));
            _movieService.Setup(x => x.UpdateMovie(It.IsAny<MovieDomainModel>())).Returns(Task.FromResult(movieDomainModel));
            //act
            var result = _controller.Put(Guid.NewGuid(), movieModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var acceptedResultValue = ((AcceptedResult)result).Value;
            //assert
            result.Should().BeOfType<AcceptedResult>();
            acceptedResultValue.Should().BeEquivalentTo(movieDomainModel);
           
        }
        [TestMethod]
        public void Put_With_UnValid_ModelState_Return_BadRequest()
        {
            //Arrange
            string expectedMessage = "Invalid Model State";
            int expectedStatusCode = 400;
            _controller.ModelState.AddModelError("key", "Invalid Model State");

            //Act
            var result = _controller.Put(Guid.NewGuid(), movieModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = (BadRequestObjectResult)result;
            var createdResult = ((BadRequestObjectResult)result).Value;
            var errorResponse = ((SerializableError)createdResult).GetValueOrDefault("key");
            var message = (string[])errorResponse;

            //Assert
            resultResponse.Should().NotBeNull();
            expectedMessage.Should().BeEquivalentTo(message[0]);
            result.Should().BeOfType<BadRequestObjectResult>();
            expectedStatusCode.Should().Be(resultResponse.StatusCode);
        }
        [TestMethod]
        public void Put_UpdateMovieThrowDbException_ReturnsBadRequest()
        {
            //Arrange
            string expectedMessage = "Inner exception error message.";
            int expectedStatusCode = 400;

            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);

            _movieService.Setup(x => x.UpdateMovie(It.IsAny<MovieDomainModel>())).Throws(dbUpdateException);
            _movieService.Setup(x => x.GetMovieByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(movieDomainModel));
            //Act
            var result = _controller.Put(Guid.NewGuid(), movieModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = (BadRequestObjectResult)result;
            var badObjectResult = ((BadRequestObjectResult)result).Value;
            var errorResult = (ErrorResponseModel)badObjectResult;

            //Assert
            resultResponse.Should().NotBeNull();
            expectedMessage.Should().BeEquivalentTo(errorResult.ErrorMessage);
            result.Should().BeOfType<BadRequestObjectResult>();
            expectedStatusCode.Should().Be(resultResponse.StatusCode);

        }
        [TestMethod]
        public void Delete_DeleteMovieThrowDbException_ReturnsBadRequest()
        {
            //Arrange
            string expectedMessage = "Inner exception error message.";
            int expectedStatusCode = 400;

            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);

            _movieService.Setup(x => x.DeleteMovie(It.IsAny<Guid>())).Throws(dbUpdateException);
            //Act
            var result = _controller.Delete(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();          
            var resultResponse = (BadRequestObjectResult)result;
            var badObjectResult = ((BadRequestObjectResult)result).Value;
            var errorResult = (ErrorResponseModel)badObjectResult;

            //Assert
            resultResponse.Should().NotBeNull();
            expectedMessage.Should().BeEquivalentTo(errorResult.ErrorMessage);
            result.Should().BeOfType<BadRequestObjectResult>();
            expectedStatusCode.Should().Be(resultResponse.StatusCode);
        }
        [TestMethod]
        public void Delete_MovieNotExist_ReturnsErrorResponse()
        {
            //arrange
            string expectedMessage = "Movie does not exist.";
            _movieService.Setup(x => x.DeleteMovie(It.IsAny<Guid>())).Returns(Task.FromResult(false));
            //act
            var result = _controller.Delete(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();
            var errorResponse = ((ObjectResult)result).Value;
            var errorResult = (ErrorResponseModel)errorResponse;
            //assert
            errorResult.ErrorMessage.Should().Be(expectedMessage);
        }
        [TestMethod]
        public void Delete_Successful_ReturnsAcceptedResult()
        {
            //arrange
            var expectedStatusCode = 202;
            _movieService.Setup(x => x.DeleteMovie(It.IsAny<Guid>())).Returns(Task.FromResult(true));
            //act
            var result = _controller.Delete(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();
            var acceptedResult = ((AcceptedResult)result).Value;
            //assert
            result.Should().BeOfType<AcceptedResult>();
            expectedStatusCode.Equals(((AcceptedResult)result).StatusCode).Should().BeTrue();
        }
        [TestMethod]
        public void ActivateDeactivateMovie_MovieNotExist_ReturnsNotFoundObjectResult()
        {
            //arrange
            var expectedMessage = "Movie does not exist.";
            _movieService.Setup(x => x.ActivateDeactivateMovie(It.IsAny<Guid>(), It.IsAny<bool>())).Returns(new NotFoundObjectResult(MovieNotExist));
            //act
            var result = _controller.ActivateDeactivateMovie(Guid.NewGuid(), true).Result;
            //assert
            result.Should().BeOfType<NotFoundObjectResult>();
            result.ToString().Contains(expectedMessage);

        }
        [TestMethod]
        public void ActivateDeactivateMovie_DeactivateIfMovieHasProjectionsInFuture_ReturnsBadRequest()
        {
            //arrange
            var expectedMessage = "You cant deactivate this movie because it has projections in future!";
            _movieService.Setup(x => x.ActivateDeactivateMovie(It.IsAny<Guid>(), It.IsAny<bool>())).Returns(new BadRequestObjectResult(MovieHasProjectionsInFuture));
            //act
            var result = _controller.ActivateDeactivateMovie(Guid.NewGuid(), false).Result;
            //assert
            result.Should().BeOfType<BadRequestObjectResult>();
            result.ToString().Contains(expectedMessage);
        
        }
        [TestMethod]
        public void ActivateDeactivateMovie_ActivateIfMovieDoesNotHaveProjections_ReturnsBadRequest()
        {
            //arrange
            var expectedMessage = "You cant activate this movie because it does not have any projections!";
            _movieService.Setup(x => x.ActivateDeactivateMovie(It.IsAny<Guid>(), It.IsAny<bool>())).Returns(new BadRequestObjectResult(MovieDoesNotHaveProjections));
            //act
            var result = _controller.ActivateDeactivateMovie(Guid.NewGuid(), false).Result;
            //assert
            result.Should().BeOfType<BadRequestObjectResult>();
            result.ToString().Contains(expectedMessage);

        }
        [TestMethod]
        public void ActivateDeactivateMovie_SuccessfulActivateOrDeactivate_ReturnsTrue()
        {
            //arrange
            _movieService.Setup(x => x.ActivateDeactivateMovie(It.IsAny<Guid>(), It.IsAny<bool>())).Returns(true);
            //act
            var result = _controller.ActivateDeactivateMovie(Guid.NewGuid(), true);
            //assert
            result.Value.Should().Be(true);

        }
        [TestMethod]
        public void GetMovieWithByTags_MovieWithPassedTagsDoesNotExist_ReturnNotFoundObjectResult()
        {
            //arrange
            var expectedMessage = "Movie does not exist.";
            List<int> tags=new List<int>() { 1,2,3 };
            _movieService.Setup(x => x.GetMoviesByTags(It.IsAny<List<int>>())).Returns(new NotFoundObjectResult(MovieNotExist));
            //act
            var result=_controller.GetMovieWithByTags(tags);
            //assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
            result.ToString().Contains(expectedMessage);

        }
        [TestMethod]
        public void GetMovieWithByTags__ReturnListOfMovieDomainModels()
        {
            //arrange
            List<int> tags = new List<int>() { 1, 2, 3 };
            _movieService.Setup(x => x.GetMoviesByTags(It.IsAny<List<int>>())).Returns(movies);
            //act
            var result = _controller.GetMovieWithByTags(tags);
            //assert
            result.Value.Should().BeEquivalentTo(movies);

        }
        [TestMethod]
        public void Top10Movies_MoviesTableIsEmpty_ReturnNotFoundObjectResult()
        {
            //arrange
            List<MovieDomainModel> MovieNullList = null;
            _movieService.Setup(x => x.GetTopTenMovies(null)).Returns(Task.FromResult(MovieNullList));
            //act
            var result = _controller.Top10Movies();
            //assert
            result.Should().Be(Task.FromResult(MovieNullList));
        }
        [TestMethod]
        public void Top10Movies_ReturnMovieDomainModels()
        {
            //arrange
            List<MovieDomainModel> movieList = new List<MovieDomainModel>();
            Random r = new Random();
            for(int i = 0; i < 10; i++)
            {
                MovieDomainModel movie = new MovieDomainModel()
                {
                    Id = Guid.NewGuid(),
                    Rating = r.Next(1, 10),
                    Year = 2020,
                    Title = "test1",
                    Current = true
                };
                movieList.Add(movie);
            }
            movieList.OrderBy(x => x.Rating);
            _movieService.Setup(x => x.GetTopTenMovies(null)).Returns(Task.FromResult(movieList));
            //act
            var result = _controller.Top10Movies();
            //assert
            result.Result.Should().BeEquivalentTo(movieList);
        }
        [TestMethod]
        public void Top10Movies_IfThereAreLessThan10Movies_ReturnMoviesOrderedByRating()
        {
            //arrange
            List<MovieDomainModel> movieList = new List<MovieDomainModel>();
            Random r = new Random();
            for (int i = 0; i < 6; i++)
            {
                MovieDomainModel movie = new MovieDomainModel()
                {
                    Id = Guid.NewGuid(),
                    Rating = r.Next(1, 10),
                    Year = 2020,
                    Title = "test1",
                    Current = true
                };
                movieList.Add(movie);
            }
            movieList.OrderBy(x => x.Rating);
            _movieService.Setup(x => x.GetTopTenMovies(null)).Returns(Task.FromResult(movieList));
            //act
            var result = _controller.Top10Movies();
            //assert
            result.Result.Should().BeEquivalentTo(movieList);
        }
        [TestMethod]
        public void Top10Movies_IfYearIsPassed_ReturnMovieDomainModels()
        {
            //arrange
            List<MovieDomainModel> movieList = new List<MovieDomainModel>();
            Random r = new Random();
            for (int i = 0; i < 10; i++)
            {
                MovieDomainModel movie = new MovieDomainModel()
                {
                    Id = Guid.NewGuid(),
                    Rating = r.Next(1, 5),
                    Year = r.Next(2020,2021),
                    Title = "test1",
                    Current = true,
                    WonOscar=r.Next(1,2)==1
                };
                movieList.Add(movie);
            }
            movieList.OrderBy(x => x.Rating);
            _movieService.Setup(x => x.GetTopTenMovies(null)).Returns(Task.FromResult(movieList));
            //act
            var result = _controller.Top10Movies();
            //assert
            result.Result.Should().BeEquivalentTo(movieList.Where(x=>x.Year==2020));
        }



    }
}
