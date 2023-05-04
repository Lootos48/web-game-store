using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GameStore.PL.DTOs
{
    public class LoginUserRequestDTO
    {
        [Required(ErrorMessage = "EmailRequired")]
        [DisplayName("Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "PasswordRequired")]
        [DisplayName("Password")]
        public string Password { get; set; }
    }
}
