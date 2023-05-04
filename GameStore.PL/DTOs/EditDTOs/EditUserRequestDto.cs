using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GameStore.PL.DTOs.EditDTOs
{
    public class EditUserRequestDTO
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "UsernameRequired")]
        [DisplayName("Username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "EmailRequired")]
        [DisplayName("Email")]
        public string Email { get; set; }
    }
}
