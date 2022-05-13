using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationsController : Controller
    {
        private IReserveSeatsService _reservationsService;
        public ReservationsController(IReserveSeatsService reservationService)
        {
            _reservationsService= reservationService;
        }
        [HttpPost("CreateReservation")]
        [AllowAnonymous]
        public ActionResult<bool> CreateReservation([FromBody] CreateReservationPassingModel createModel)
        {
            var createReservation = _reservationsService.ReserveSeat(
                createModel.Id, createModel.Seats, createModel.User_Id, createModel.Projection_Id);
            return createReservation;
        }
      
    }
}
