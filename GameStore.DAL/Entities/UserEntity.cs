using GameStore.DomainModels.Enums;
using System.Collections.Generic;

namespace GameStore.DAL.Entities
{
    public class UserEntity : Entity
    {
        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public List<CommentEntity> Comments { get; set; } = new List<CommentEntity>();

        public List<OrderEntity> Orders { get; set; } = new List<OrderEntity>();

        public UserRoles Role { get; set; }
    }
}
