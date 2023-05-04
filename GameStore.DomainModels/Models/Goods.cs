using GameStore.DomainModels.Models.Localizations;
using System;
using System.Collections.Generic;

namespace GameStore.DomainModels.Models
{
    public class Goods
    {
        public string Id { get; set; }

        public string Key { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public short UnitsInStock { get; set; }

        public bool Discontinued { get; set; }

        public string QuantityPerUnit { get; set; }

        public int UnitsInOrder { get; set; }

        public int ReorderLevel { get; set; }

        public DateTime? DateOfPublishing { get; set; }

        public DateTime DateOfAdding { get; set; }

        public long ViewCount { get; set; }

        public Distributor Distributor { get; set; }

        public string DistributorId { get; set; }

        public List<Comment> Comments { get; set; } = new List<Comment>();

        public List<PlatformType> PlatformTypes { get; set; } = new List<PlatformType>();

        public List<Genre> Genres { get; set; } = new List<Genre>();

        public List<GoodsLocalization> Localizations { get; set; }
    }
}
