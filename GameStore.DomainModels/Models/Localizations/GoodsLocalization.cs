using System;

namespace GameStore.DomainModels.Models.Localizations
{
    public class GoodsLocalization
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Guid GoodsId { get; set; }

        public Goods Goods { get; set; }

        public Guid LocalizationId { get; set; }

        public Localization Localization { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DateOfDelete { get; set; }
    }
}
