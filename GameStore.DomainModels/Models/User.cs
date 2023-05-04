using GameStore.DomainModels.Enums;
using System;
using System.Collections.Generic;

namespace GameStore.DomainModels.Models
{
    public class User
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public List<Comment> Comments { get; set; } = new List<Comment>();

        public List<Order> Orders { get; set; } = new List<Order>();

        public UserRoles Role { get; set; }
    }
}
