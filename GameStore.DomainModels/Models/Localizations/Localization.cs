using System;

namespace GameStore.DomainModels.Models.Localizations
{
    public class Localization
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string CultureCode { get; set; }
    }
}
