using GameStore.DomainModels.Models.Localizations;
using System;
using System.Collections.Generic;

namespace GameStore.DomainModels.Models
{
    public class PlatformType
    {
        public Guid Id { get; set; }

        public string Type { get; set; }

        public List<PlatformTypeLocalization> Localizations { get; set; }
    }
}
