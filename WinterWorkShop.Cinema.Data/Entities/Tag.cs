using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinterWorkShop.Cinema.Data.Entities
{
    public class Tag
    {
        [Key]
        public int Tag_Id { get; set; }
        public string Tag_name { get; set; }
        public string Description { get; set; }
        public List<MovieTags> MovieTags { get; set; }
    }
}
