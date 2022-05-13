using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WinterWorkShop.Cinema.Data
{
    public class Cinema
    {
        [Key]
        public int Cinema_Id { get; set; }

        public string Name { get; set; }

        public virtual List<Auditorium> Auditoriums { get; set; }
    }
}
