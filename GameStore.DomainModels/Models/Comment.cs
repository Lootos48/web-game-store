using GameStore.DomainModels.Enums;
using System;
using System.Collections.Generic;

namespace GameStore.DomainModels.Models
{
    public class Comment
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Body { get; set; }

        public Guid? ParentId { get; set; }

        public Comment Parent { get; set; }

        public List<Comment> Replies { get; set; } = new List<Comment>();

        public Guid GameId { get; set; }

        public bool IsDeleted { get; set; }

        public CommentType Type { get; set; }

        public User Author { get; set; }

        public Guid? AuthorId { get; set; }
    }
}
