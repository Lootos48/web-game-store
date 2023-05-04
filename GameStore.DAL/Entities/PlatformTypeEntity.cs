using GameStore.DAL.Entities.Localizations;
using System.Collections.Generic;

namespace GameStore.DAL.Entities
{
    public class PlatformTypeEntity : Entity
    {
        public string Type { get; set; }

        public ICollection<PlatformTypeLocalizaitionEntity> Localizations { get; set; }

        public ICollection<GamePlatformTypeEntity> GamesPlatformTypes { get; } = new List<GamePlatformTypeEntity>();
    }
}
