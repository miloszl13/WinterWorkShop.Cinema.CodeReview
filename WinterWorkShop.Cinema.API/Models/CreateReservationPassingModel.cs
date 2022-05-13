using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WinterWorkShop.Cinema.API.Models
{
    public class CreateReservationPassingModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public List<Guid> Seats { get; set; }
        [Required]
        public Guid User_Id { get; set; }
        [Required]
        public Guid Projection_Id { get; set; }
    }
}
