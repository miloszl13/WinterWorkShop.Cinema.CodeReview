using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinterWorkShop.Cinema.Data.Entities
{
    public class Reservation
    {
        [Key]
        public int Reservation_Id { get; set; }
        public double Reservation_Price { get; set; }
        [ForeignKey("User")]
        public Guid User_Id { get; set; }
        public User User { get; set; }
        [ForeignKey("Projection")]
        public Guid Projection_Id { get; set; }
        public Projection Projection { get; set; }
        public List<ReservedSeats> ReservedSeats { get; set; }
    }
}
