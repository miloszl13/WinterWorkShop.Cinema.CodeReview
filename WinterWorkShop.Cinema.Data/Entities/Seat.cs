using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using WinterWorkShop.Cinema.Data.Entities;

namespace WinterWorkShop.Cinema.Data
{
    public class Seat
    {
        [Key]
        public Guid Seat_Id { get; set; }
        
        [ForeignKey("Auditorium")]
        public int Auditorium_Id { get; set; }
        public virtual Auditorium Auditorium { get; set; }


        public int Row { get; set; }
        
        public int Number { get; set; }
        public List<ReservedSeats> ReservedSeats { get; set; }

    }
}
