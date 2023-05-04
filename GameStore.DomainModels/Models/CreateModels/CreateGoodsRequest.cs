using System;
using System.Collections.Generic;

namespace GameStore.DomainModels.Models.CreateModels
{
    public class CreateGoodsRequest
    {
        public string Key { get; set; }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public short UnitsInStock { get; set; }

        public DateTime? DateOfPublishing { get; set; }

        public DateTime DateOfAdding { get; set; }

        public Distributor Distributor { get; set; }

        public string DistributorId { get; set; }

        public List<PlatformType> PlatformTypes { get; set; } = new List<PlatformType>();

        public List<Genre> Genres { get; set; } = new List<Genre>();
    }
}
