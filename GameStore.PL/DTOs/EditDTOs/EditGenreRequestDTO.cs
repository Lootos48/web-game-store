using GameStore.PL.DTOs.LocalizationsDTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GameStore.PL.DTOs.EditDTOs
{
    public class EditGenreRequestDTO
    {
        [Required]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "NameRequired")]
        [DisplayName("Name")]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        public string Description { get; set; }

        public string Picture { get; set; }

        public Guid? ParentId { get; set; }

        public Guid? ChosedLocalization { get; set; }

        public List<GenreLocalizationDTO> Localizations { get; set; } = new List<GenreLocalizationDTO>();
    }
}
