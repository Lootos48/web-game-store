using GameStore.PL.DTOs.LocalizationsDTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GameStore.PL.DTOs.EditDTOs
{
    public class EditGoodsRequestDTO
    {
        [Required]
        public string Id { get; set; }

        [DisplayName("Key")]
        public string Key { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "NameRequired")]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [DisplayName("Description")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [DisplayName("Price")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [DisplayName("UnitsInStock")]
        public short UnitsInStock { get; set; }

        [DisplayName("QuantityPerUnit")]
        public string QuantityPerUnit { get; set; }

        [DisplayName("UnitsInOrder")]
        public int UnitsInOrder { get; set; }

        [DisplayName("ReorderLevel")]
        public int ReorderLevel { get; set; }

        [DisplayName("Discontinued")]
        public bool Discontinued { get; set; }

        [DisplayName("DateOfPublishing")]
        public DateTime? DateOfPublishing { get; set; }

        [DisplayName("DateOfAdding")]
        public DateTime DateOfAdding { get; set; }

        public long ViewCount { get; set; }

        public string DistributorId { get; set; }

        public Guid? ChosedLocalization { get; set; }

        public List<GoodsLocalizationDTO> Localizations { get; set; } = new List<GoodsLocalizationDTO>();

        public List<Guid> Genres { get; set; } = new List<Guid>();

        public List<Guid> PlatformTypes { get; set; } = new List<Guid>();
    }
}
