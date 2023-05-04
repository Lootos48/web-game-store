using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GameStore.PL.DTOs
{
    public class OrderDetailsDTO
    {
        public Guid Id { get; set; }

        [DisplayName("Price")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [DisplayName("Quantity")]
        public short Quantity { get; set; }

        [DisplayName("Discount")]
        public double Discount { get; set; }

        public GoodsDTO Product { get; set; }

        public string OrderId { get; set; }
    }
}
