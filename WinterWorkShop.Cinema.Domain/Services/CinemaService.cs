using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Repositories;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.Domain.Common;

namespace WinterWorkShop.Cinema.Domain.Services
{
    public class CinemaService : ICinemaService
    {
        private readonly ICinemasRepository _cinemasRepository;
        private readonly IAuditoriumService _auditoriumService;

        public CinemaService(ICinemasRepository cinemasRepository,IAuditoriumService auditoriumService)
        {
            _cinemasRepository = cinemasRepository;
            _auditoriumService = auditoriumService;
        }

        public ActionResult<bool> Create(CinemaDomainModel cinemaModel)
        {
            Data.Cinema cinemaToCreate = new Data.Cinema()
            {
                Cinema_Id = cinemaModel.Id,
                Name = cinemaModel.Name
            };
            if (cinemaModel.auditName == null)
            {
                try
                {
                    var data = _cinemasRepository.Insert(cinemaToCreate);
                    _cinemasRepository.Save();
                    return true;

                }
                catch (Exception ex)
                {
                    var errorResponse = new ErrorResponseModels()
                    {
                        ErrorMessage = Messages.CINEMA_CREATE_ERROR,
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return new BadRequestObjectResult(errorResponse);
                }
            }
            else
            {
                try
                {
                    var data = _cinemasRepository.Insert(cinemaToCreate);
                    AuditoriumDomainModel audit = new AuditoriumDomainModel()
                    {
                        CinemaId = cinemaModel.Id,
                        Name = cinemaModel.auditName
                    };
                    var createAuditoriumResultModel = _auditoriumService.CreateAuditorium(audit, cinemaModel.seatRows, cinemaModel.numberOfSeats).Result;
                    if (!createAuditoriumResultModel.IsSuccessful)
                    {
                        ErrorResponseModels errorResponse = new ErrorResponseModels()
                        {
                            ErrorMessage = createAuditoriumResultModel.ErrorMessage,
                            StatusCode = System.Net.HttpStatusCode.BadRequest
                        };

                        return new BadRequestObjectResult(errorResponse);
                    }
                    _cinemasRepository.Save();
                    return true;
                }
                catch (Exception ex)
                {
                    ErrorResponseModels errorResponse = new ErrorResponseModels
                    {
                        ErrorMessage = ex.InnerException.Message ?? ex.Message,
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    };

                    return new BadRequestObjectResult(errorResponse);
                }
            }
        }

        public async Task<List<CinemaDomainModel>> GetAllAsync()
        {
            var data = await _cinemasRepository.GetAll();

            if (data == null)
            {
                return null;
            }

            List<CinemaDomainModel> result = new List<CinemaDomainModel>();
            CinemaDomainModel model;
            foreach (var item in data)
            {
                model = new CinemaDomainModel
                {
                    Id = item.Cinema_Id,
                    Name = item.Name
                };
                result.Add(model);
            }

            return result;
        }
        public ActionResult<bool> Delete(int cinemaId)
        {
            var deleteCinema = _cinemasRepository.Delete(cinemaId);
            if (deleteCinema == null)
            {
                ErrorResponseModels errorResponse = new ErrorResponseModels()
                {
                    ErrorMessage = Messages.CINEMA_NOT_EXIST,
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
                return new NotFoundObjectResult(errorResponse);
            }
            else
                return new OkObjectResult(true);
        }
    }

}
