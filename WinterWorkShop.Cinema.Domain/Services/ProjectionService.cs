using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Domain.Services
{
    public class ProjectionService : IProjectionService
    {
        private readonly IProjectionsRepository _projectionsRepository;
        private readonly IMoviesRepository _moviesRepository;
        private readonly IAuditoriumsRepository _auditoriumRepository;
        public ProjectionService(IProjectionsRepository projectionsRepository, IMoviesRepository moviesRepository,
            IAuditoriumsRepository auditoriumsRepository)
        {
            _projectionsRepository = projectionsRepository;
            _moviesRepository = moviesRepository;
            _auditoriumRepository = auditoriumsRepository;
        }

        public async Task<List<ProjectionDomainModel>> GetAllAsync()
        {
            var data = await _projectionsRepository.GetAll();

            if (data == null)
            {
                return null;
            }

            List<ProjectionDomainModel> result = new List<ProjectionDomainModel>();
            ProjectionDomainModel model;
            foreach (var item in data)
            {
                model = new ProjectionDomainModel
                {
                    Id = item.Projection_Id,
                    MovieId = item.Movie_Id,
                    AuditoriumId = item.Auditorium_Id,
                    ProjectionTime = item.DateTime,
                    MovieTitle = item.Movie.Title,
                    AditoriumName = item.Auditorium.AuditName
                };
                result.Add(model);
            }

            return result;
        }

        public async Task<CreateProjectionResultModel> CreateProjection(ProjectionDomainModel domainModel)
        {
            int projectionTime = 3;

            var projectionsAtSameTime = _projectionsRepository.GetByAuditoriumId(domainModel.AuditoriumId)
                .Where(x => x.DateTime < domainModel.ProjectionTime.AddHours(projectionTime) && x.DateTime > domainModel.ProjectionTime.AddHours(-projectionTime))
                .ToList();

            if (projectionsAtSameTime != null && projectionsAtSameTime.Count > 0)
            {
                return new CreateProjectionResultModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.PROJECTIONS_AT_SAME_TIME
                };
            }

            var newProjection = new Data.Projection
            {
                Movie_Id = domainModel.MovieId,
                Auditorium_Id = domainModel.AuditoriumId,
                DateTime = domainModel.ProjectionTime,
                Price=domainModel.ProjectionPrice
            };

            var insertedProjection = _projectionsRepository.Insert(newProjection);
  
            if (insertedProjection == null)
            {
                return new CreateProjectionResultModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.PROJECTION_CREATION_ERROR
                };
            }

            _projectionsRepository.Save();
            var movie=_moviesRepository.GetByIdAsync(domainModel.MovieId).Result;
            movie.Current = true;
            _moviesRepository.Update(movie);
            CreateProjectionResultModel result = new CreateProjectionResultModel
            {
                IsSuccessful = true,
                ErrorMessage = null,
                Projection = new ProjectionDomainModel
                {
                    Id = insertedProjection.Projection_Id,
                    AuditoriumId = insertedProjection.Auditorium_Id,
                    MovieId = insertedProjection.Movie_Id,
                    ProjectionTime = insertedProjection.DateTime,
                    ProjectionPrice=insertedProjection.Price
                }
            };

            return result;
        }
        public ActionResult<List<ProjectionDomainModel>> GetProjectionsByCinemaId(int cinemaId)
        {
            List<Projection> projections = new List<Projection>();
            //find all auditoriums from cinema with current cinemaid
            List<Auditorium> auditoriums = _auditoriumRepository.GetAll().Result.Where(x=>x.Cinema_Id==cinemaId).ToList();
            //find all projections that are playing in some of those auditoriums
            foreach(Auditorium a in auditoriums)
            {
                List<Projection> projection = _projectionsRepository.GetAll().Result.Where(x => x.Auditorium_Id==a.Auditorium_Id).ToList();
                if(projection.Count > 0)
                {
                    projections.AddRange(projection);                 
                }
            }
            List<ProjectionDomainModel> result = new List<ProjectionDomainModel>();
            foreach(Projection p in projections)
            {
                var projectionDomainModel = new ProjectionDomainModel()
                {
                    Id = p.Projection_Id,
                    MovieId = p.Movie_Id,
                    MovieTitle = p.Movie.Title,
                    AuditoriumId = p.Auditorium_Id,
                    AditoriumName = p.Auditorium.AuditName,
                    ProjectionPrice = p.Price,
                    ProjectionTime = p.DateTime
                };
                result.Add(projectionDomainModel);
            }
            if (result.Count == 0)
            {
                var responseModel = new ErrorResponseModels()
                {
                    ErrorMessage = Messages.PROJECTION_IN_CINEMA_NOT_EXIST,
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
                return new NotFoundObjectResult(responseModel);
            }
            return result;
        }
        public ActionResult<List<ProjectionDomainModel>> GetProjectionsByAuditorium(int auditoriumId)
        {
            List<Projection> projections = _projectionsRepository.GetAll().Result.Where(x => x.Auditorium_Id == auditoriumId).ToList();
            
            List<ProjectionDomainModel> result = new List<ProjectionDomainModel>();
            
            foreach (Projection p in projections)
            {
                var projectionDomainModel = new ProjectionDomainModel()
                {
                    Id = p.Projection_Id,
                    MovieId = p.Movie_Id,
                    MovieTitle = p.Movie.Title,
                    AuditoriumId = p.Auditorium_Id,
                    AditoriumName = p.Auditorium.AuditName,
                    ProjectionPrice = p.Price,
                    ProjectionTime = p.DateTime
                };
                result.Add(projectionDomainModel);
            }
            if (result.Count == 0)
            {
                var responseModel = new ErrorResponseModels()
                {
                    ErrorMessage = Messages.PROJECTION_IN_AUDITORIUM_NOT_EXIST,
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
                return new NotFoundObjectResult(responseModel);
            }
            return result;
        }
       public ActionResult<List<ProjectionDomainModel>> GetProjectionsByMovie(Guid movieId)
        {
            var movie = _moviesRepository.GetByIdAsync(movieId).Result;
            if (movie == null)
            {
                var responseModel = new ErrorResponseModels()
                {
                    ErrorMessage = Messages.MOVIE_DOES_NOT_EXIST,
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
                return new NotFoundObjectResult(responseModel);

            }
            List<Projection> projections = _projectionsRepository.GetAll().Result.Where(x => x.Movie_Id == movieId).ToList();

            List<ProjectionDomainModel> result = new List<ProjectionDomainModel>();

            foreach (Projection p in projections)
            {
                var projectionDomainModel = new ProjectionDomainModel()
                {
                    Id = p.Projection_Id,
                    MovieId = p.Movie_Id,
                    MovieTitle = p.Movie.Title,
                    AuditoriumId = p.Auditorium_Id,
                    AditoriumName = p.Auditorium.AuditName,
                    ProjectionPrice = p.Price,
                    ProjectionTime = p.DateTime
                };
                result.Add(projectionDomainModel);
            }
            if (result.Count == 0)
            {
                var responseModel = new ErrorResponseModels()
                {
                    ErrorMessage = Messages.PROJECTION_FOR_THAT_MOVIE_NOT_EXIST,
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
                return new NotFoundObjectResult(responseModel);
            }
            return result;
        }
        public ActionResult<List<ProjectionDomainModel>> GetProjectionsByDateTime(DateTime datetime)
        {
            
            List<Projection> projections = _projectionsRepository.GetAll().Result.Where(x => x.DateTime == datetime).ToList();

            List<ProjectionDomainModel> result = new List<ProjectionDomainModel>();

            foreach (Projection p in projections)
            {
                var projectionDomainModel = new ProjectionDomainModel()
                {
                    Id = p.Projection_Id,
                    MovieId = p.Movie_Id,
                    MovieTitle = p.Movie.Title,
                    AuditoriumId = p.Auditorium_Id,
                    AditoriumName = p.Auditorium.AuditName,
                    ProjectionPrice = p.Price,
                    ProjectionTime = p.DateTime
                };
                result.Add(projectionDomainModel);
            }
            if (result.Count == 0)
            {
                var responseModel = new ErrorResponseModels()
                {
                    ErrorMessage = Messages.PROJECTION_FOR_THAT_TIME_NOT_EXIST,
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
                return new NotFoundObjectResult(responseModel);
            }
            return result;
        }
    }
}
