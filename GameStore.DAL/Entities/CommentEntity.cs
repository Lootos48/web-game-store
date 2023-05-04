using GameStore.DomainModels.Enums;
using System;
using System.Collections.Generic;

namespace GameStore.DAL.Entities
{
    public class CommentEntity : Entity
    {
        /// <summary>
        /// Name is Author of comment name
        /// </summary>
        public string Name { get; set; }

        public Guid? AuthorId { get; set; }

        public UserEntity Author { get; set; }

        /// <summary>
        /// Text of comment
        /// </summary>
        public string Body { get; set; }

        public CommentEntity Parent { get; set; }

        public Guid? ParentId { get; set; }

        public GameEntity Game { get; set; }

        public Guid GameId { get; set; }

        public CommentType Type { get; set; }

        public ICollection<CommentEntity> Replies { get; } = new List<CommentEntity>();
    }
}
