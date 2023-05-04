using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GameStore.PL.DTOs.CreateDTOs
{
    public class CreateGoodsRequestDTO
    {
        [DisplayName("Key")]
        public string Key { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "NameRequired")]
        public string Name { get; set; }

        [DisplayName("Description")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required(ErrorMessage = "PriceRequired")]
        [DisplayName("Price")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "UnitsInStockRequired")]
        [DisplayName("UnitsInStock")]
        public short UnitsInStock { get; set; }

        public string DistributorId { get; set; }

        [DisplayName("DateOfPublishing")]
        public DateTime? DateOfPublishing { get; set; }

        public List<Guid> Genres { get; set; } = new List<Guid>();

        public List<Guid> PlatformTypes { get; set; } = new List<Guid>();
    }
}
