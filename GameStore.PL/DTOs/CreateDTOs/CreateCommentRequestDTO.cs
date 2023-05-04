using GameStore.DomainModels.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GameStore.PL.DTOs.CreateDTOs
{
    public class CreateCommentRequestDTO
    {
        [DisplayName("Name")]
        [StringLength(100, ErrorMessage = "NameLength")]
        public string Name { get; set; }

        [Required(ErrorMessage = "BodyRequired")]
        [DisplayName("Body")]
        [DataType(DataType.MultilineText)]
        [StringLength(300, ErrorMessage = "BodyLength")]
        public string Body { get; set; }

        public CommentType Type { get; set; }

        public Guid? ParentId { get; set; }

        public Guid? AuthorId { get; set; }
    }
}
