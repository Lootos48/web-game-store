using System;

namespace GameStore.DAL.Entities
{
    public class LegacyKeyEntity : BaseEntity
    {
        public Guid Id { get; set; }

        public string LegacyKey { get; set; }

        public Guid GameId { get; set; }

        public GameEntity Game { get; set; }
    }
}
