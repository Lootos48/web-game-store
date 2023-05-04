using System;

namespace GameStore.DomainModels.Models
{
    public class GoodsProductMapping
    {
        public Guid Id { get; set; }

        public Guid GameId { get; set; }

        public string GameKey { get; set; }

        public int? ProductId { get; set; }

        public bool IsFullyMigrated 
        { 
            get
            {
                return GameId != Guid.Empty 
                    && !string.IsNullOrEmpty(GameKey) 
                    && ProductId.HasValue;
            }
        }
    }
}
