using GameStore.DomainModels.Enums;
using System;
using System.Collections.Generic;

namespace GameStore.DomainModels.Models
{
    public class GoodsSearchRequest
    {
        public List<Guid> Genres { get; set; } = new List<Guid>();

        public List<Guid> PlatformTypes { get; set; } = new List<Guid>();

        public List<string> Distributors { get; set; } = new List<string>();

        public string Name { get; set; }

        public GamesSortType SortBy { get; set; }

        public decimal MinPrice { get; set; }

        public decimal MaxPrice { get; set; }

        public uint CountOfDaysBeforePublishingDate { get; set; }

        public int ItemsPerPage { get; set; }

        public int CurrentPage { get; set; }
    }
}
