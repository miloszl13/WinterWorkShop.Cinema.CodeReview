using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Domain.Interfaces
{
    public interface IReserveSeatsService
    {
        ActionResult<bool> ReserveSeat(int idReservation, List<Guid> seats, Guid user_id, Guid projection_id);
        ActionResult<List<ReserveSeatDomainModel>> GetAllReservedSeats();
    }
}
