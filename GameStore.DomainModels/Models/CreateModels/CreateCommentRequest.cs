using GameStore.DomainModels.Enums;
using System;

namespace GameStore.DomainModels.Models.CreateModels
{
    public class CreateCommentRequest
    {
        public string Name { get; set; }

        public string Body { get; set; }

        public Guid? ParentId { get; set; }

        public CommentType Type { get; set; }

        public Guid? AuthorId { get; set; }
    }
}
