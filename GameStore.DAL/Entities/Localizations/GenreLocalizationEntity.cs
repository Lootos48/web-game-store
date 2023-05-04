using System;

namespace GameStore.DAL.Entities.Localizations
{
    public class GenreLocalizationEntity : Entity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public Guid GenreId { get; set; }

        public GenreEntity Genre { get; set; }

        public Guid LocalizationId { get; set; }

        public LocalizationEntity Localization { get; set; }
    }
}
