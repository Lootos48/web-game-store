using GameStore.PL.DTOs.LocalizationsDTO;
using GameStore.PL.Util.Localizers.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GameStore.PL.DTOs
{
    public class GenreDTO : ILocalizable
    {
        [Required]
        public Guid Id { get; set; }

        [DisplayName("Name")]
        public string Name { get; set; }

        public GenreDTO Parent { get; set; }

        public List<GenreDTO> SubGenres { get; set; }

        public IList Localizations { get; set; } = new List<GenreLocalizationDTO>();
    }
}
