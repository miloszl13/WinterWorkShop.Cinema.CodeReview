using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinterWorkShop.Cinema.Domain.Models
{
    public class ReserveSeatDomainModel
    {
        public int Reservation_Id { get; set; }
        public Guid Seat_Id { get; set; }
        public DateTime DateTime { get; set; }
    }
}
