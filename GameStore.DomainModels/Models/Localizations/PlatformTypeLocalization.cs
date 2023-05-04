using System;

namespace GameStore.DomainModels.Models.Localizations
{
    public class PlatformTypeLocalization
    {
        public Guid Id { get; set; }

        public string Type { get; set; }

        public Guid PlatformTypeId { get; set; }

        public PlatformType PlatformType { get; set; }

        public Guid LocalizationId { get; set; }

        public Localization Localization { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DateOfDelete { get; set; }
    }
}
