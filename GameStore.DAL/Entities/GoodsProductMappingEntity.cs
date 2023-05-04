using System;

namespace GameStore.DAL.Entities
{
    public class GoodsProductMappingEntity : BaseEntity
    {
        public Guid Id { get; set; }

        public Guid GameId { get; set; }

        public string GameKey { get; set; }

        public int? ProductId { get; set; }
    }
}
