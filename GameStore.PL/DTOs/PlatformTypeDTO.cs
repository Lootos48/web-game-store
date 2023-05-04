using GameStore.PL.DTOs.LocalizationsDTO;
using GameStore.PL.Util.Localizers.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GameStore.PL.DTOs
{
    public class PlatformTypeDTO : ILocalizable
    {
        [Required]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "TypeRequired")]
        [DisplayName("Type")]
        public string Type { get; set; }

        public IList Localizations { get; set; } = new List<PlatformTypeLocalizationDTO>();
    }
}
