using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
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
    public class MovieServiceTests
    {
        private Mock<IMoviesRepository> _moviesRepository;
        private Mock<IProjectionsRepository> _projectionRepository;
        private Mock<IMovieTagsRepository> _movieTagsRepository;
        private MovieService _movieService;

        private MovieDomainModel _movieDomainModel;
        private List<MovieDomainModel> _movieDomains;
        private Movie _movie;
        private List<Movie> _movieList;

        private Projection projection;
        private List<Projection> projectionList;

        private Tag tag;
        private List<Tag> tags;

        private MovieTags movietag;
        private List<MovieTags> movietags;

        [TestInitialize]
        public void TestInitialize()
        {
            _moviesRepository = new Mock<IMoviesRepository>();
            _projectionRepository = new Mock<IProjectionsRepository>();
            _movieTagsRepository = new Mock<IMovieTagsRepository>();
            _movieService = new MovieService(_moviesRepository.Object, _projectionRepository.Object, _movieTagsRepository.Object);

            _movieDomainModel = new MovieDomainModel()
            {
                Id = Guid.Parse("a1b6b0eb-2ec4-49d1-8c43-38c5468d390a"),
                Rating = 9.1,
                Year = 2020,
                Title = "Superman",
                Current = true,
                WonOscar=false
            };
            _movieDomains = new List<MovieDomainModel>() { _movieDomainModel };
            _movie = new Movie()
            {
                Movie_Id = Guid.Parse("a1b6b0eb-2ec4-49d1-8c43-38c5468d390a"),
                Rating = 9.1,
                Year = 2020,
                Title = "Superman",
                Current = true,
                WonOscar=false
            };
            _movieList = new List<Movie>() { _movie };

            //projections
            projection = new Projection()
            {
                Projection_Id = Guid.Parse("2313d1b3-c0ba-43e4-9d94-56281a90fea3"),
                Auditorium_Id = 1,
                Auditorium = new Auditorium { Auditorium_Id = 1 },
                Movie_Id = Guid.Parse("a1b6b0eb-2ec4-49d1-8c43-38c5468d390a"),
                Price = 300,
                Movie = new Movie() { Movie_Id = Guid.Parse("a1b6b0eb-2ec4-49d1-8c43-38c5468d390a") },
                DateTime = DateTime.Parse("2023-04-30 11:07:29.3204088")
            };
            projectionList = new List<Projection>() { projection };
            //movietags
            tag = new Tag()
            {
                Tag_Id = 1,
                Tag_name = "actor",
                Description = "Brad Pit",
                MovieTags = new List<MovieTags>()
            };
            movietag = new MovieTags()
            {
                Movie_Id = Guid.Parse("a1b6b0eb-2ec4-49d1-8c43-38c5468d390a"),
                Movie = new Movie() { Movie_Id = Guid.Parse("a1b6b0eb-2ec4-49d1-8c43-38c5468d390a") },
                Tag_Id = 1,
                Tag = new Tag() { Tag_Id = 1 }
            };
            movietags = new List<MovieTags>() { movietag };

        }
        [TestMethod]
        public void GetAllCurrentMovies_IfGetCurrentMoviesReturnNull_ReturnNull()
        {
            //Arrange
            List<Movie> nullMovies = null;
            Task<List<Movie>> responseTask = Task.FromResult(nullMovies);
            _moviesRepository.Setup(x => x.GetCurrentMovies()).Returns(responseTask.Result);
            //Act
            var resultAction = _movieService.GetAllCurrentMovies();
            //Assert
            resultAction.Should().BeNull();

        }

        [TestMethod]
        public void GetAllCurrentMovies_IfThereAreSomeMoviesWithCurrentEqualTrue_ReturnListOfMovieDomainModels()
        {
            Task<List<Movie>> responseTask = Task.FromResult(_movieList);
            int expectedResultCount = 1;
            _moviesRepository.Setup(x => x.GetCurrentMovies()).Returns(responseTask.Result);
            //act
            var resultAction = _movieService.GetAllCurrentMovies();
            var result = (List<MovieDomainModel>)resultAction;
            //Assert
            Assert.AreEqual(expectedResultCount, result.Count);
            Assert.AreEqual(_movie.Movie_Id, result[0].Id);
            result[0].Should().BeOfType<MovieDomainModel>();
        }
        [TestMethod]
        public void GetAllMovies_IfGetCurrentMoviesReturnNull_ReturnNull()
        {
            //Arrange
            List<Movie> nullMovies = null;
            Task<List<Movie>> responseTask = Task.FromResult(nullMovies);
            _moviesRepository.Setup(x => x.GetAll()).Returns(responseTask);
            //Act
            var resultAction = _movieService.GetAllMovies();
            //Assert
            resultAction.Should().BeNull();

        }

        [TestMethod]
        public void GetAllMovies_IfThereAreSomeMoviesWithCurrentEqualTrue_ReturnListOfMovieDomainModels()
        {
            Task<List<Movie>> responseTask = Task.FromResult(_movieList);
            int expectedResultCount = 1;
            _moviesRepository.Setup(x => x.GetAll()).Returns(responseTask);
            //act
            var resultAction = _movieService.GetAllMovies();
            var result = (List<MovieDomainModel>)resultAction;
            //Assert
            Assert.AreEqual(expectedResultCount, result.Count);
            Assert.AreEqual(_movie.Movie_Id, result[0].Id);
            result[0].Should().BeOfType<MovieDomainModel>();
        }
        [TestMethod]
        public void GetMovieByIdAsync_IfGetCurrentMoviesReturnNull_ReturnNull()
        {
            //Arrange
            Movie nullMovie = null;
            Task<Movie> responseTask = Task.FromResult(nullMovie);
            _moviesRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(responseTask);
            //Act
            var resultAction = _movieService.GetMovieByIdAsync(Guid.NewGuid()).Result;
            //Assert
            resultAction.Should().BeNull();

        }

        [TestMethod]
        public void GetMovieByIdAsync_IfThereAreSomeMoviesWithCurrentEqualTrue_ReturnListOfMovieDomainModels()
        {
            //arrange
            Task<Movie> responseTask = Task.FromResult(_movie);
            _moviesRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(responseTask);
            //act
            var resultAction = _movieService.GetMovieByIdAsync(Guid.NewGuid()).Result;
            var result = (MovieDomainModel)resultAction;
            //Assert
            Assert.AreEqual(_movie.Movie_Id, result.Id);
            result.Should().BeOfType<MovieDomainModel>();
        }
        [TestMethod]
        public void AddMovie_IfInsertMovieReturnNull_ReturnNull()
        {
            //arrange
            Movie nullMovie = null;
            _moviesRepository.Setup(x => x.Insert(It.IsAny<Movie>())).Returns(nullMovie);
            //act
            var result = _movieService.AddMovie(_movieDomainModel).Result;
            //asser
            result.Should().BeNull();
        }
        [TestMethod]
        public void AddUser_Successful_ReturnMovieDomainModel()
        {
            //arrange
            _moviesRepository.Setup(x => x.Insert(It.IsAny<Movie>())).Returns(_movie);
            //act
            var result = _movieService.AddMovie(_movieDomainModel);
            //asser
            result.Result.Should().BeEquivalentTo(_movieDomainModel);

        }
        [TestMethod]
        public void UpdateMovie_IfUpdateReturnNull_ReturnNull()
        {
            //arrange
            Movie nullMovie = null;
            _moviesRepository.Setup(x => x.Update(It.IsAny<Movie>())).Returns(nullMovie);
            //act
            var result = _movieService.UpdateMovie(_movieDomainModel).Result;
            //asser
            result.Should().BeNull();
        }
        [TestMethod]
        public void UpdateMovie_Successful_ReturnMovieDomainModel()
        {
            //arrange
            _moviesRepository.Setup(x => x.Update(It.IsAny<Movie>())).Returns(_movie);
            //act
            var result = _movieService.UpdateMovie(_movieDomainModel);
            //asser
            result.Result.Should().BeEquivalentTo(_movieDomainModel);

        }
        [TestMethod]
        public void DeleteMovie_IfDeleteReturnNull_ReturnFalse()
        {
            //arrange
            Movie nullMovie = null;
            _moviesRepository.Setup(x => x.Delete(It.IsAny<Guid>())).Returns(nullMovie);
            //act
            var result = _movieService.DeleteMovie(Guid.NewGuid()).Result;
            //asser
            result.Should().Be(false);
        }
        [TestMethod]
        public void DeleteMovie_Successful_ReturnTrue()
        {
            //arrange
            _moviesRepository.Setup(x => x.Delete(It.IsAny<Guid>())).Returns(_movie);
            //act
            var result = _movieService.DeleteMovie(Guid.NewGuid());
            //asser
            result.Result.Should().Be(true);

        }
        //activate-deactivate
        //
        //
        //
        [TestMethod]
        public void ActivateDeactivateMovie_MovieNotExist_ReturnsNotFoundObjectResult()
        {
            //arrange
            var expectedMessage = "Movie does not exist.";
            Movie nullMovie = null;
            _moviesRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(nullMovie));
            //act
            var result = _movieService.ActivateDeactivateMovie(Guid.NewGuid(), false).Result;
            //assert
            result.Should().BeOfType<NotFoundObjectResult>();
            result.ToString().Contains(expectedMessage);

        }
        [TestMethod]
        public void ActivateDeactivateMovie_DeactivateIfMovieHasProjectionsInFuture_ReturnsBadRequest()
        {
            //arrange
            var expectedMessage = "You cant deactivate this movie because it has projections in future!";
            _moviesRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(_movie));
            _projectionRepository.Setup(x => x.GetAll()).Returns(Task.FromResult(projectionList));
            //act
            var result = _movieService.ActivateDeactivateMovie(Guid.Parse("a1b6b0eb-2ec4-49d1-8c43-38c5468d390a"), false);
            //assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
            result.ToString().Contains(expectedMessage);

        }
        [TestMethod]
        public void ActivateDeactivateMovie_ActivateIfMovieDoesNotHaveProjections_ReturnsBadRequest()
        {
            //arrange
            var expectedMessage = "You cant activate this movie because it does not have any projections!";
            _movie.Current = false;
            _moviesRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(_movie));
            _projectionRepository.Setup(x => x.GetAll()).Returns(Task.FromResult(new List<Projection>()));
            //act
            var result = _movieService.ActivateDeactivateMovie(Guid.NewGuid(), true).Result;
            //assert
            result.Should().BeOfType<BadRequestObjectResult>();
            result.ToString().Contains(expectedMessage);

        }
        [TestMethod]
        public void ActivateDeactivateMovie_SuccessfulActivateOrDeactivate_ReturnsTrue()
        {
            //arrange
            _moviesRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(_movie));
            _projectionRepository.Setup(x => x.GetAll()).Returns(Task.FromResult(new List<Projection>()));
            _moviesRepository.Setup(x => x.Update(It.IsAny<Movie>()));
            //act
            var result = _movieService.ActivateDeactivateMovie(Guid.NewGuid(), false);
            //assert
            result.Value.Should().Be(true);

        }
        [TestMethod]
        public void GetMovieWithByTags_MovieWithPassedTagsDoesNotExist_ReturnNotFoundObjectResult()
        {
            //arrange
            var expectedMessage = "Movie does not exist.";
            List<int> tags = new List<int>() { 1, 2, 3 };
            _movieTagsRepository.Setup(x => x.GetAllMovieTags()).Returns(movietags);
            _moviesRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(_movie));
            //act
            var result = _movieService.GetMoviesByTags(tags);
            //assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
            result.ToString().Contains(expectedMessage);

        }
        [TestMethod]
        public void GetMovieWithByTags__ReturnListOfMovieDomainModels()
        {
            //arrange
            List<int> tags = new List<int>() { 1 };
            _movieTagsRepository.Setup(x => x.GetAllMovieTags()).Returns(movietags);
            _moviesRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(_movie));
            //act
            var result = _movieService.GetMoviesByTags(tags);
            //assert
            result.Value.Should().BeEquivalentTo(_movieDomains);

        }
        //TopTenMovies
        [TestMethod]
        public void Top10Movies_MoviesTableIsEmpty_ReturnNull()
        {
            //arrange
            List<Movie> MovieNullList = new List<Movie>();
            _moviesRepository.Setup(x => x.GetAll()).Returns(Task.FromResult(MovieNullList));
            //act
            var result = _movieService.GetTopTenMovies(null);
            //assert
            result.Should().Be(null);
        }
        [TestMethod]
        public void Top10Movies_YearIsNull_ReturnMovieDomainModels()
        {
            //arrange
            List<Movie> movieList = new List<Movie>();
            for (int i = 0; i < 10; i++)
            {
                Movie movie = new Movie()
                {
                    Movie_Id = Guid.Parse(i + "313d1b3-c0ba-43e4-9d94-56281a90fea3"),
                    Rating = i + 1,
                    Year = 2020,
                    Title = "test1",
                    Current = true,
                    MovieTags = new List<MovieTags>(),
                    Projections = new List<Projection>()
                };
                movieList.Add(movie);
            }
            List<Movie> orderedMovies = movieList.OrderByDescending(x => x.Rating).ToList();
            _moviesRepository.Setup(x => x.GetAll()).Returns(Task.FromResult(orderedMovies));
            //act
            var result = _movieService.GetTopTenMovies(null).Result;
            var resultDomainModels = (List<MovieDomainModel>)result;
            //assert
            for (int i = 0; i < resultDomainModels.Count; i++)
            {
                Assert.AreEqual(resultDomainModels[i].Id, orderedMovies[i].Movie_Id);
            }
        }
        [TestMethod]
        public void Top10Movies_IfThereAreLessThan10Movies_ReturnMoviesOrderedByRating()
        {
            //arrange
            List<Movie> movieList = new List<Movie>();
            for (int i = 0; i < 7; i++)
            {
                Movie movie = new Movie()
                {
                    Movie_Id = Guid.Parse(i + "313d1b3-c0ba-43e4-9d94-56281a90fea3"),
                    Rating = i + 1,
                    Year = 2020,
                    Title = "test1",
                    Current = true,
                    MovieTags = new List<MovieTags>(),
                    Projections = new List<Projection>()
                };
                movieList.Add(movie);
            }
            List<Movie> orderedMovies = movieList.OrderByDescending(x => x.Rating).ToList();
            _moviesRepository.Setup(x => x.GetAll()).Returns(Task.FromResult(orderedMovies));
            //act
            var result = _movieService.GetTopTenMovies(null).Result;
            var resultDomainModels = (List<MovieDomainModel>)result;
            //assert
            for (int i = 0; i < resultDomainModels.Count; i++)
            {
                Assert.AreEqual(resultDomainModels[i].Id, orderedMovies[i].Movie_Id);
            }
        }
        [TestMethod]
        public void Top10Movies_YearIsPassed_ReturnMovieDomainModels()
        {
            //arrange
            List<Movie> movieList = new List<Movie>();
            Movie movie = new Movie()
            {
                Movie_Id = Guid.Parse("1313d1b3-c0ba-43e4-9d94-56281a90fea3"),
                Rating = 1,
                Year = 2020,
                Title = "test1",
                Current = true,
                MovieTags = new List<MovieTags>(),
                Projections = new List<Projection>(),
                WonOscar=false,
            };
            Movie movie1 = new Movie()
            {
                Movie_Id = Guid.Parse("2313d1b3-c0ba-43e4-9d94-56281a90fea3"),
                Rating = 1,
                Year = 2020,
                Title = "test1",
                Current = true,
                MovieTags = new List<MovieTags>(),
                Projections = new List<Projection>(),
                WonOscar = true,
            };
            Movie movie2 = new Movie()
            {
                Movie_Id = Guid.Parse("3313d1b3-c0ba-43e4-9d94-56281a90fea3"),
                Rating = 2,
                Year = 2020,
                Title = "test1",
                Current = true,
                MovieTags = new List<MovieTags>(),
                Projections = new List<Projection>(),
                WonOscar = false,
            };
            Movie movie3 = new Movie()
            {
                Movie_Id = Guid.Parse("4313d1b3-c0ba-43e4-9d94-56281a90fea3"),
                Rating = 2,
                Year = 2020,
                Title = "test1",
                Current = true,
                MovieTags = new List<MovieTags>(),
                Projections = new List<Projection>(),
                WonOscar = true,
            };
            Movie movie4 = new Movie()
            {
                Movie_Id = Guid.Parse("5313d1b3-c0ba-43e4-9d94-56281a90fea3"),
                Rating = 1,
                Year = 2021,
                Title = "test1",
                Current = true,
                MovieTags = new List<MovieTags>(),
                Projections = new List<Projection>(),
                WonOscar = false,
            };
            movieList.AddRange(new List<Movie>() { movie, movie1, movie2, movie3, movie4 });           
            List<Movie> orderedMovies = movieList.OrderByDescending(x => x.Rating).ToList();
            List<Movie> moviesInYear=orderedMovies.Where(x=>x.Year==2020).ToList();
            for (int i = 0, j = i + 1; i < orderedMovies.Count - 1; i++, j++)
            {
                if (orderedMovies[i].Rating == orderedMovies[j].Rating)
                {
                    if (orderedMovies[i].WonOscar == true && orderedMovies[j].WonOscar == false)
                    {
                        continue;
                    }
                    else if (orderedMovies[i].WonOscar == false && orderedMovies[j].WonOscar == true)
                    {
                        Movie t = orderedMovies[i];
                        orderedMovies[i] = orderedMovies[j];
                        orderedMovies[j] = t;

                    }
                    else
                    {
                        continue;
                    }
                }
            }
            _moviesRepository.Setup(x => x.GetAll()).Returns(Task.FromResult(orderedMovies));
            //act
            var result = _movieService.GetTopTenMovies(2020).Result;
            var resultDomainModels = (List<MovieDomainModel>)result;
            //assert
            for (int i = 0; i < resultDomainModels.Count; i++)
            {
                Assert.AreEqual(resultDomainModels[i].Id, orderedMovies[i].Movie_Id);
            }
        }

    }
}
