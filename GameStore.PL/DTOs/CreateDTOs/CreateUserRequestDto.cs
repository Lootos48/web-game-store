using System.ComponentModel.DataAnnotations;

namespace GameStore.PL.DTOs.CreateDTOs
{
    public class CreateUserRequestDTO
    {
        [Required(ErrorMessage = "EmailRequired")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "UsernameRequired")]
        public string Username { get; set; }

        [Required(ErrorMessage = "PasswordRequired")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
