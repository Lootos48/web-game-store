using System;

namespace GameStore.DAL.Entities.Localizations
{
    public class GameLocalizationEntity : Entity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public Guid GameId { get; set; }

        public GameEntity Game { get; set; }

        public Guid LocalizationId { get; set; }

        public LocalizationEntity Localization { get; set; }
    }
}
