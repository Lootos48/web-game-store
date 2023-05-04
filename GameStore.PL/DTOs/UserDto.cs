using GameStore.DomainModels.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace GameStore.PL.DTOs
{
    public class UserDTO
    {
        public Guid Id { get; set; }

        [DisplayName("Username")]
        public string Username { get; set; }

        [DisplayName("Email")]
        public string Email { get; set; }

        [DisplayName("Password")]
        public string Password { get; set; }

        public List<CommentDTO> Comments { get; set; } = new List<CommentDTO>();

        public List<OrderDTO> Orders { get; set; } = new List<OrderDTO>();

        public UserRoles Role { get; set; }
    }
}
