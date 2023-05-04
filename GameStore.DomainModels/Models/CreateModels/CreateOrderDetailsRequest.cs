using System;

namespace GameStore.DomainModels.Models.CreateModels
{
    public class CreateOrderDetailsRequest
    {
        public decimal Price { get; set; }

        public short Quantity { get; set; }

        public double Discount { get; set; }

        public Guid ProductId { get; set; }

        public Guid OrderId { get; set; }
    }
}
