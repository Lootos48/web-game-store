using System;
using System.ComponentModel;

namespace GameStore.PL.DTOs.LocalizationsDTO
{
    public class GoodsLocalizationDTO
    {
        public Guid Id { get; set; }

        [DisplayName("Name")]
        public string Name { get; set; }

        [DisplayName("Description")]
        public string Description { get; set; }

        public Guid GoodsId { get; set; }

        public GoodsDTO Goods { get; set; }

        public Guid LocalizationId { get; set; }

        public LocalizationDTO Localization { get; set; }
    }
}
