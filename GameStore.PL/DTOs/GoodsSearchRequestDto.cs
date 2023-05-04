using GameStore.DomainModels.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GameStore.PL.DTOs
{
    public class GoodsSearchRequestDTO
    {
        public List<Guid> Genres { get; set; } = new List<Guid>();

        public List<Guid> PlatformTypes { get; set; } = new List<Guid>();

        public List<string> Distributors { get; set; } = new List<string>();

        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; }

        public GamesSortType SortBy { get; set; }

        public decimal MinPrice { get; set; }

        public decimal MaxPrice { get; set; }

        public uint CountOfDaysBeforePublishingDate { get; set; }

        public int ItemsPerPage { get; set; }

        public int CurrentPage { get; set; }
    }
}
