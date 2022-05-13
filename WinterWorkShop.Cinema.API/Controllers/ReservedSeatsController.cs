using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Domain.Services;

namespace WinterWorkShop.Cinema.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservedSeatsController : ControllerBase
    {
        private readonly IReserveSeatsService _reservationSeatsService;
        public ReservedSeatsController(IReserveSeatsService reservationSeatsService)
        {
            _reservationSeatsService = reservationSeatsService;
        }
        [HttpGet]
        [Route("all")]
        public ActionResult<List<ReserveSeatDomainModel>> GetAsync()
        { 
           var reservedSeatDomainModels = _reservationSeatsService.GetAllReservedSeats();
           return reservedSeatDomainModels;
        }
    }
}
