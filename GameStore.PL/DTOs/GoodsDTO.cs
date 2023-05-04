using GameStore.PL.DTOs.LocalizationsDTO;
using GameStore.PL.Util.Localizers.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GameStore.PL.DTOs
{
    public class GoodsDTO : ILocalizable
    {
        [Required]
        public string Id { get; set; }

        [DisplayName("Key")]
        public string Key { get; set; }

        [Required]
        [DisplayName("Name")]
        public string Name { get; set; }

        [DisplayName("Description")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [DisplayName("Price")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [DisplayName("UnitsInStock")]
        public short UnitsInStock { get; set; }

        [DisplayName("Discontinued")]
        public bool Discontinued { get; set; }

        [DisplayName("QuantityPerUnit")]
        public string QuantityPerUnit { get; set; }

        [DisplayName("UnitsInOrder")]
        public int UnitsInOrder { get; set; }

        [DisplayName("ReorderLevel")]
        public int ReorderLevel { get; set; }

        [DisplayName("DateOfPublishing")]
        public DateTime? DateOfPublishing { get; set; }

        [DisplayName("DateOfAdding")]
        public DateTime DateOfAdding { get; set; }

        public long ViewCount { get; set; }

        [DisplayName("Distributor")]
        public string Distributor { get; set; }

        [DisplayName("Genres")]
        public List<GenreDTO> Genres { get; set; } = new List<GenreDTO>();

        [DisplayName("PlatformTypes")]
        public List<PlatformTypeDTO> PlatformTypes { get; set; } = new List<PlatformTypeDTO>();

        public IList Localizations { get; set; } = new List<GoodsLocalizationDTO>();
    }
}
