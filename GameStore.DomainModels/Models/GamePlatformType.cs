using System;

namespace GameStore.DomainModels.Models
{
    public class GamePlatformType
    {
        public Guid GameId { get; set; }

        public Goods Game { get; set; }

        public Guid PlatformId { get; set; }

        public PlatformType PlatformType { get; set; }
    }
}
