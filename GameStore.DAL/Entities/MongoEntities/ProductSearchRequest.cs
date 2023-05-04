using GameStore.DomainModels.Enums;
using System.Collections.Generic;

namespace GameStore.DAL.Entities.MongoEntities
{
    public class ProductSearchRequest
    {
        public List<string> PlatformTypes { get; set; } = new List<string>();

        public List<string> Categories { get; set; } = new List<string>();

        public List<string> Suppliers { get; set; } = new List<string>();

        public List<string> ExcludedProductKeys { get; set; } = new List<string>();

        public string Name { get; set; }

        public decimal MinPrice { get; set; }

        public decimal MaxPrice { get; set; }

        public uint CountOfDaysBeforePublishingDate { get; set; }

        public GamesSortType SortBy { get; set; }

        public int ItemsPerPage { get; set; }

        public int CurrentPage { get; set; }
    }
}
