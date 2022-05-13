using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WinterWorkShop.Cinema.Data
{
    public class Auditorium
    {
        [Key]
        public int Auditorium_Id { get; set; }

        [ForeignKey("Cinema")]
        public int Cinema_Id { get; set; }
        public Cinema Cinema { get; set; }


        public string AuditName { get; set; }

        public virtual List<Projection> Projections { get; set; }

        public virtual List<Seat> Seats { get; set; }

    }
}
