using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using WinterWorkShop.Cinema.Domain.Common;

namespace WinterWorkShop.Cinema.Domain.Models
{
    public class UserDomainModel
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public int? BonusPoints { get; set; }
    }
}
