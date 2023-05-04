using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace GameStore.PL.DTOs.CreateDTOs
{
    public class CreateOrderDetailsRequestDTO
    {
        [DisplayName("Price")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [DisplayName("Quantity")]
        public short Quantity { get; set; }

        [DisplayName("Discount")]
        public double Discount { get; set; }

        public Guid ProductId { get; set; }

        public Guid OrderId { get; set; }
    }
}
