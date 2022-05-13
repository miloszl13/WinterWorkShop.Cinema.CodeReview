using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Domain.Interfaces
{
    public interface ISeatService
    {
        Task<List<SeatDomainModel>> GetAllAsync();
        //void ReserveSeat(Guid seat_id,int idReservation);
    }
}
