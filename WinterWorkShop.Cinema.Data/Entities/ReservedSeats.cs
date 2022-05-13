using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinterWorkShop.Cinema.Data.Entities
{
    public class ReservedSeats
    {
        [ForeignKey("Reservation")]
        public int Reservation_Id { get; set; }
        public Reservation Reservation { get; set; }

        [ForeignKey("Seat")]
        public Guid Seat_Id { get; set; }
        public Seat Seat { get; set; }
        public DateTime DateTime { get; set; }


    }
}
