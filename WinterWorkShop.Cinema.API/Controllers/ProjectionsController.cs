using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectionsController : ControllerBase
    {
        private readonly IProjectionService _projectionService;

        public ProjectionsController(IProjectionService projectionService)
        {
            _projectionService = projectionService;
        }

        /// <summary>
        /// Gets all projections
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("all")]
        public async Task<ActionResult<IEnumerable<ProjectionDomainModel>>> GetAsync()
        {
            IEnumerable<ProjectionDomainModel> projectionDomainModels;
           
             projectionDomainModels = await _projectionService.GetAllAsync();            

            if (projectionDomainModels == null)
            {
                projectionDomainModels = new List<ProjectionDomainModel>();
            }

            return Ok(projectionDomainModels);
        }

        /// <summary>
        /// Adds a new projection
        /// </summary>
        /// <param name="projectionModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ProjectionDomainModel>> PostAsync(CreateProjectionModel projectionModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (projectionModel.ProjectionTime < DateTime.Now)
            {
                ModelState.AddModelError(nameof(projectionModel.ProjectionTime), Messages.PROJECTION_IN_PAST);
                return BadRequest(ModelState);
            }

            ProjectionDomainModel domainModel = new ProjectionDomainModel
            {
                AuditoriumId = projectionModel.AuditoriumId,
                MovieId = projectionModel.MovieId,
                ProjectionTime = projectionModel.ProjectionTime,
                ProjectionPrice=projectionModel.ProjectionPrice
            };

            CreateProjectionResultModel createProjectionResultModel;

            try
            {
                createProjectionResultModel = await _projectionService.CreateProjection(domainModel);
            }
            catch (DbUpdateException e)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = e.InnerException.Message ?? e.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            if (!createProjectionResultModel.IsSuccessful)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = createProjectionResultModel.ErrorMessage,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);                
            }

            return Created("projections//" + createProjectionResultModel.Projection.Id, createProjectionResultModel.Projection);
        }
        [HttpGet]
        [Route("GetProjectionsByCinema/{cinemaId}")]
        public ActionResult<List<ProjectionDomainModel>> GetProjectionsByCinema([FromRoute] int cinemaId)
        {
            var result = _projectionService.GetProjectionsByCinemaId(cinemaId);
            return result;
        }
        [HttpGet]
        [Route("GetProjectionsByAuditorium/{auditoriumId}")]
        public ActionResult<List<ProjectionDomainModel>> GetProjectionsByAuditorium([FromRoute] int auditoriumId)
        {
            var result = _projectionService.GetProjectionsByAuditorium(auditoriumId);
            return result;
        }
        [HttpGet]
        [Route("GetProjectionsByMovie/{movieId}")]
        public ActionResult<List<ProjectionDomainModel>> GetProjectionsByMovie([FromRoute] Guid movieId)
        {
            var result = _projectionService.GetProjectionsByMovie(movieId);
            return result;
        }
        [HttpGet]
        [Route("GetProjectionsByDateTime/{datetime}")]
        public ActionResult<List<ProjectionDomainModel>> GetProjectionsByDateTime([FromRoute] DateTime datetime)
        {
            var result = _projectionService.GetProjectionsByDateTime(datetime);
            return result;
        }
    }
}