using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WinterWorkShop.Cinema.Data
{
    public class Projection
    {
        [Key]
        public Guid Projection_Id { get; set; }
        [ForeignKey("Auditorium")]
        public int Auditorium_Id { get; set; }
        public virtual Auditorium Auditorium { get; set; }

        public DateTime DateTime { get; set; }
        public double Price { get; set; }
        [ForeignKey("Movie")]
        public Guid Movie_Id { get; set; }

        public virtual Movie Movie { get; set; }

    }
}
