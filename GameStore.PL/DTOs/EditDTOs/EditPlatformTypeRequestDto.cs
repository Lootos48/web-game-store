using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using GameStore.PL.DTOs.LocalizationsDTO;
using System.ComponentModel;

namespace GameStore.PL.DTOs.EditDTOs
{
    public class EditPlatformTypeRequestDTO
    {
        [Required]
        public Guid Id { get; set; }

        [DisplayName("Type")]
        [Required(ErrorMessage = "TypeRequired")]
        public string Type { get; set; }

        public List<PlatformTypeLocalizationDTO> Localizations { get; set; } = new List<PlatformTypeLocalizationDTO>();

        public Guid? ChosedLocalization { get; set; }
    }
}
