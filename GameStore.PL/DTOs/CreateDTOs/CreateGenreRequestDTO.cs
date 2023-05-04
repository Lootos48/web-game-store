using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GameStore.PL.DTOs.CreateDTOs
{
    public class CreateGenreRequestDTO
    {
        [DisplayName("Name")]
        [Required(ErrorMessage = "NameRequired")]
        public string Name { get; set; }

        public Guid? ParentId { get; set; }
    }
}
