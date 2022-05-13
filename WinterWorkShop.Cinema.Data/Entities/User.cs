using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using WinterWorkShop.Cinema.Data.Entities;

namespace WinterWorkShop.Cinema.Data
{
    public class User
    {
        [Key]
        public Guid User_Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }  
        public string Password { get; set; }
        public string Role { get; set; }
        public int BonusPoints { get; set; }
        public List<Reservation> Reservations { get; set; }
    }
}
