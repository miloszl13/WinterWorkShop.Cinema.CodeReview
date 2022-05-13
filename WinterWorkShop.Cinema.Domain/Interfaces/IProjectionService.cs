using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Domain.Interfaces
{
    public interface IProjectionService
    {
        Task<List<ProjectionDomainModel>> GetAllAsync();
        Task<CreateProjectionResultModel> CreateProjection(ProjectionDomainModel domainModel);
        ActionResult<List<ProjectionDomainModel>> GetProjectionsByCinemaId(int cinemaId);
        ActionResult<List<ProjectionDomainModel>>GetProjectionsByAuditorium(int auditoriumId);
        ActionResult<List<ProjectionDomainModel>>GetProjectionsByMovie(Guid movieId);    
        ActionResult<List<ProjectionDomainModel>> GetProjectionsByDateTime(DateTime datetime);
    }
}
