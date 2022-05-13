using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using WinterWorkShop.Cinema.Data.Entities;

namespace WinterWorkShop.Cinema.Data
{
    public class Movie
    {
        [Key]
        public Guid Movie_Id { get; set; }

        public string Title { get; set; }

        public int Year { get; set; }

        public double? Rating { get; set; }

        public bool Current { get; set; }
        public bool WonOscar { get; set; } = false;

        public virtual List<Projection> Projections { get; set; }
        public List<MovieTags> MovieTags { get; set; }
    }
}
