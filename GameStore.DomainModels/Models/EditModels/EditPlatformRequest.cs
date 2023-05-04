using GameStore.DomainModels.Models.Localizations;
using System;
using System.Collections.Generic;

namespace GameStore.DomainModels.Models.EditModels
{
    public class EditPlatformRequest
    {
        public Guid Id { get; set; }

        public string Type { get; set; }

        public Guid? ChosedLocalization { get; set; }

        public List<PlatformTypeLocalization> Localizations { get; set; } = new List<PlatformTypeLocalization>();
    }
}
