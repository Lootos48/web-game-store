using GameStore.DomainModels.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GameStore.PL.DTOs
{
    public class CommentDTO
    {
        public Guid Id { get; set; }

        [DisplayName("Name")]
        public string Name { get; set; }

        [DisplayName("Body")]
        [DataType(DataType.MultilineText)]
        public string Body { get; set; }

        public CommentType Type { get; set; }

        public Guid? ParentId { get; set; }

        public CommentDTO Parent { get; set; }

        public bool IsDeleted { get; set; }

        public UserDTO Author { get; set; }

        public Guid? AuthorId { get; set; }

        public List<CommentDTO> Replies { get; set; }
    }
}
