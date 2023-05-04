using GameStore.DomainModels.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GameStore.PL.DTOs.EditDTOs
{
    public class EditCommentRequestDTO
    {
        public Guid Id { get; set; }

        [DisplayName("Name")]
        public string Name { get; set; }

        [DisplayName("Body")]
        [Required(ErrorMessage = "BodyRequired")]
        [DataType(DataType.MultilineText)]
        public string Body { get; set; }

        public CommentType Type { get; set; }

        public Guid GameId { get; set; }

        public Guid? ParentId { get; set; }

        public Guid? AuthorId { get; set; }
    }
}
