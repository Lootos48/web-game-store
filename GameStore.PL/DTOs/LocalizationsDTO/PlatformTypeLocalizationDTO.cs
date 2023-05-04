using System;
using System.ComponentModel;

namespace GameStore.PL.DTOs.LocalizationsDTO
{
    public class PlatformTypeLocalizationDTO
    {
        public Guid Id { get; set; }

        [DisplayName("Type")]
        public string Type { get; set; }

        public Guid PlatformTypeId { get; set; }

        public PlatformTypeDTO PlatformType { get; set; }

        public Guid LocalizationId { get; set; }

        public LocalizationDTO Localization { get; set; }
    }
}
