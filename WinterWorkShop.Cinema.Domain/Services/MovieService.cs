using Microsoft.AspNetCore.Mvc;
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
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Domain.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMoviesRepository _moviesRepository;
        private readonly IProjectionsRepository _projectionRepository;
        private readonly IMovieTagsRepository _movieTagsRepository;

        public MovieService(IMoviesRepository moviesRepository, IProjectionsRepository projectionRepository, IMovieTagsRepository movieTagsRepository)
        {
            _moviesRepository = moviesRepository;
            _projectionRepository = projectionRepository;
            _movieTagsRepository = movieTagsRepository;
        }

        public List<MovieDomainModel> GetAllCurrentMovies()
        {
            var data = _moviesRepository.GetCurrentMovies();

            if (data == null)
            {
                return null;
            }

            List<MovieDomainModel> result = new List<MovieDomainModel>();
            MovieDomainModel model;
            foreach (var item in data)
            {
                model = new MovieDomainModel
                {
                    Current = item.Current,
                    Id = item.Movie_Id,
                    Rating = item.Rating ?? 0,
                    Title = item.Title,
                    Year = item.Year,
                    WonOscar = item.WonOscar
                };
                result.Add(model);
            }

            return result;

        }
        public List<MovieDomainModel> GetAllMovies()
        {
            var data = _moviesRepository.GetAll().Result;

            if (data == null)
            {
                return null;
            }

            List<MovieDomainModel> result = new List<MovieDomainModel>();
            MovieDomainModel model;
            foreach (var item in data)
            {
                model = new MovieDomainModel
                {
                    Current = item.Current,
                    Id = item.Movie_Id,
                    Rating = item.Rating ?? 0,
                    Title = item.Title,
                    Year = item.Year,
                    WonOscar = item.WonOscar
                };
                result.Add(model);
            }

            return result;

        }

        public async Task<MovieDomainModel> GetMovieByIdAsync(Guid id)
        {
            var data = await _moviesRepository.GetByIdAsync(id);

            if (data == null)
            {
                return null;
            }

            MovieDomainModel domainModel = new MovieDomainModel
            {
                Id = data.Movie_Id,
                Current = data.Current,
                Rating = data.Rating ?? 0,
                Title = data.Title,
                Year = data.Year,
                WonOscar = data.WonOscar
            };

            return domainModel;
        }

        public async Task<MovieDomainModel> AddMovie(MovieDomainModel newMovie)
        {
            Movie movieToCreate = new Movie()
            {
                Title = newMovie.Title,
                Current = newMovie.Current,
                Year = newMovie.Year,
                Rating = newMovie.Rating,
                WonOscar = newMovie.WonOscar
            };

            var data = _moviesRepository.Insert(movieToCreate);
            if (data == null)
            {
                return null;
            }

            _moviesRepository.Save();

            MovieDomainModel domainModel = new MovieDomainModel()
            {
                Id = data.Movie_Id,
                Title = data.Title,
                Current = data.Current,
                Year = data.Year,
                Rating = data.Rating ?? 0,
                WonOscar = data.WonOscar
            };

            return domainModel;
        }

        public async Task<MovieDomainModel> UpdateMovie(MovieDomainModel updateMovie) {

            Movie movie = new Movie()
            {
                Movie_Id = updateMovie.Id,
                Title = updateMovie.Title,
                Current = updateMovie.Current,
                Year = updateMovie.Year,
                Rating = updateMovie.Rating,
                WonOscar = updateMovie.WonOscar
            };

            var data = _moviesRepository.Update(movie);

            if (data == null)
            {
                return null;
            }
            _moviesRepository.Save();

            MovieDomainModel domainModel = new MovieDomainModel()
            {
                Id = data.Movie_Id,
                Title = data.Title,
                Current = data.Current,
                Year = data.Year,
                Rating = data.Rating ?? 0,
                WonOscar = data.WonOscar
            };

            return domainModel;
        }

        public async Task<bool> DeleteMovie(Guid id)
        {
            var data = _moviesRepository.Delete(id);

            if (data == null)
            {
                return false;
            }

            _moviesRepository.Save();
            return true;
        }
        public ActionResult<bool> ActivateDeactivateMovie(Guid movieId, bool current)
        {
            //find movie in db by passed id
            var movie = _moviesRepository.GetByIdAsync(movieId).Result;
            if (movie == null) //return error if moview does not exist
            {
                var responseModel = new ErrorResponseModels()
                {
                    ErrorMessage = Messages.MOVIE_DOES_NOT_EXIST,
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
                return new NotFoundObjectResult(responseModel);
            }
            //get all projections
            var projections = _projectionRepository.GetAll().Result;
            //find projections for passed movie
            var projectionForMovie = projections.FirstOrDefault(x => x.Movie_Id == movie.Movie_Id);
            //if projection exist,projection is in future and we passed false for movie wich current is true return error
            //becasue you cant deactivate movie that has projections in future
            if (projectionForMovie != null && projectionForMovie.DateTime > DateTime.Now && movie.Current == true && current == false)
            {
                var responseModel = new ErrorResponseModels()
                {
                    ErrorMessage = Messages.MOVIE_CANNOT_DEACTIVATE_MOVIE,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return new BadRequestObjectResult(responseModel);
            }
            else if (projectionForMovie == null && current == true && movie.Current == false)
            {
                var responseModel = new ErrorResponseModels()
                {
                    ErrorMessage = Messages.MOVIE_CANNOT_ACTIVATE_MOVIE,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return new BadRequestObjectResult(responseModel);
            }
            movie.Current = current;
            _moviesRepository.Update(movie);
            return true;
        }

        public ActionResult<List<MovieDomainModel>> GetMoviesByTags(List<int> tags)
        {
            List<Guid> MoviesWithTag = new List<Guid>();
            List<MovieTags> MovieTags = new List<MovieTags>();
            for (int i = 0; i < tags.Count; i++)
            {
                List<MovieTags> moviesTagsWithFirstTag = _movieTagsRepository.GetAllMovieTags().Where(x => x.Tag_Id == tags[i]).ToList();
                MovieTags.AddRange(moviesTagsWithFirstTag);
                foreach (var movie in moviesTagsWithFirstTag)
                {
                    MoviesWithTag.Add(movie.Movie_Id);
                }
            }
            List<Guid> resultMovieIds = new List<Guid>();

            for (int i = 0; i < MoviesWithTag.Count; i++)
            {
                bool contains = false;
                for (int j = 0; j < tags.Count; j++)
                {
                    if ((MovieTags.FirstOrDefault(x => x.Tag_Id == tags[j] && x.Movie_Id == MoviesWithTag[i])) != null)
                    {
                        contains = true;
                    }
                    else
                    {
                        contains = false;
                        break;
                    }

                }
                if (contains == true)
                {
                    resultMovieIds.Add(MoviesWithTag[i]);
                }
            }
            List<Movie> Movies = new List<Movie>();
            List<MovieDomainModel> resultMovies = new List<MovieDomainModel>();

            foreach (Guid g in resultMovieIds)
            {
                var movie = _moviesRepository.GetByIdAsync(g).Result;
                if (movie != null && Movies.FirstOrDefault(x => x.Movie_Id == g) == null)
                {
                    Movies.Add(movie);
                }
            }
            foreach (Movie m in Movies)
            {
                var movieDomainModel = new MovieDomainModel()
                {
                    Id = m.Movie_Id,
                    Title = m.Title,
                    Current = m.Current,
                    Rating = m.Rating ?? 0,
                    Year = m.Year,
                    WonOscar = m.WonOscar

                };
                resultMovies.Add(movieDomainModel);
            }
            if (resultMovies.Count == 0)
            {
                var responseModel = new ErrorResponseModels()
                {
                    ErrorMessage = Messages.MOVIE_DOES_NOT_EXIST,
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
                return new NotFoundObjectResult(responseModel);
            }
            return resultMovies;
        }
        public Task<List<MovieDomainModel>> GetTopTenMovies(int? year)
        {
            var moviesDb = _moviesRepository.GetAll().Result;
            if (moviesDb.Count() == 0)
            {
                return null;
            }
            if (year == null)
            {
                moviesDb = moviesDb.OrderByDescending(x => x.Rating).ToList();
                List<MovieDomainModel> result = SortMovies(moviesDb);

                return Task.FromResult(result);
            }
            else
            {
                List<Movie> moviesInYear = moviesDb.Where(x => x.Year == year)
                    .OrderByDescending(x=>x.Rating).ToList();
                if (moviesInYear.Count() == 0)
                {
                    return null;
                }
                var result = SortMoviesByOscar(moviesInYear);
                return Task.FromResult(result);
            }
        }
        private List<MovieDomainModel> SortMovies(List<Movie> movies)
        {
            List<Movie> topRatedMovies=new List<Movie>();
            if (movies.Count < 10)
            {
                for (int i = 0; i < movies.Count; i++)
                {
                    var movie = movies[i];
                    topRatedMovies.Add(movie);

                }
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    var movie = movies[i];
                    topRatedMovies.Add(movie);

                }
            }
            var result = new List<MovieDomainModel>();
            foreach (var movie in topRatedMovies)
            {
                var domainModel = new MovieDomainModel()
                {
                    Id = movie.Movie_Id,
                    Title = movie.Title,
                    Year = movie.Year,
                    Rating = movie.Rating ?? 0,
                    Current = movie.Current,
                    WonOscar = movie.WonOscar
                };
                result.Add(domainModel);
            }
            return result;
        }
        private List<MovieDomainModel> SortMoviesByOscar(List<Movie> moviesInYear)
        {
            List<Movie> topRatedMovies = new List<Movie>();
            if (moviesInYear.Count() < 10)
            {
                for (int i = 0; i < moviesInYear.Count(); i++)
                {
                    var movie = moviesInYear[i];
                    topRatedMovies.Add(movie);

                }
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    var movie = moviesInYear[i];
                    topRatedMovies.Add(movie);

                }
            }
            for (int i = 0, j = i + 1; i < topRatedMovies.Count - 1; i++, j++)

            {
                if (topRatedMovies[i].Rating == topRatedMovies[j].Rating)
                {
                    if (topRatedMovies[i].WonOscar == true && topRatedMovies[j].WonOscar == false)
                    {
                        continue;
                    }
                    else if (topRatedMovies[i].WonOscar == false && topRatedMovies[j].WonOscar == true)
                    {
                        Movie t = topRatedMovies[i];
                        topRatedMovies[i] = topRatedMovies[j];
                        topRatedMovies[j] = t;

                    }
                    else
                    {
                        continue;
                    }
                }
            }



            var result = new List<MovieDomainModel>();
            foreach (var movie in topRatedMovies)
            {
                var domainModel = new MovieDomainModel()
                {
                    Id = movie.Movie_Id,
                    Title = movie.Title,
                    Year = movie.Year,
                    Rating = movie.Rating ?? 0,
                    Current = movie.Current,
                    WonOscar = movie.WonOscar
                };
                result.Add(domainModel);
            }
            return result;
        }
    
    }
}
 