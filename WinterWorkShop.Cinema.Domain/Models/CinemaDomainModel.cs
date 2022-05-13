using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using WinterWorkShop.Cinema.Domain.Common;

namespace WinterWorkShop.Cinema.Domain.Models
{
    public class CinemaDomainModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [StringLength(50, ErrorMessage = Messages.AUDITORIUM_PROPERTIE_NAME_NOT_VALID)]
        public string? auditName { get; set; }

        [Range(1, 20, ErrorMessage = Messages.AUDITORIUM_PROPERTIE_SEATROWSNUMBER_NOT_VALID)]
        public int seatRows { get; set; }

        [Range(1, 20, ErrorMessage = Messages.AUDITORIUM_PROPERTIE_SEATNUMBER_NOT_VALID)]
        public int numberOfSeats { get; set; }
    }
}
