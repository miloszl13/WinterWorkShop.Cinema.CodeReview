using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinterWorkShop.Cinema.Data.Entities
{
    public class MovieTags
    {
        [ForeignKey("Tag")]
        public int Tag_Id { get; set; }
        public Tag Tag { get; set; }

        [ForeignKey("Movie")]
        public Guid Movie_Id { get; set; }
        public Movie Movie { get; set; }
    }
}
