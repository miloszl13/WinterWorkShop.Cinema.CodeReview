using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WinterWorkShop.Cinema.Domain.Common;

namespace WinterWorkShop.Cinema.API.Models
{
    public class UserModel:IValidatableObject
    {

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = Messages.USER_USERNAME_TOO_LONG)]
        public string UserName { get; set; }

        [Required]
        [StringLength(15,MinimumLength = 5, ErrorMessage = Messages.USER_PASSWORD_ERROR)]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(Role != "admin" && Role != "user")
            {
                yield return new ValidationResult($"Roles can be only user or admin !");
            }
        }
    }
}