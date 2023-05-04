using System;

namespace GameStore.DAL.Entities.Localizations
{
    public class PlatformTypeLocalizaitionEntity : Entity
    {
        public string Type { get; set; }

        public Guid PlatformTypeId { get; set; }

        public PlatformTypeEntity PlatformType { get; set; }

        public Guid LocalizationId { get; set; }

        public LocalizationEntity Localization { get; set; }
    }
}
