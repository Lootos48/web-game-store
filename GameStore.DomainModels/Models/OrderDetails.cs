using System;

namespace GameStore.DomainModels.Models
{
    public class OrderDetails
    {
        public Guid Id { get; set; }

        public decimal Price { get; set; }

        public short Quantity { get; set; }

        public double Discount { get; set; }

        public Goods Product { get; set; }

        public string ProductId { get; set; }

        public Order Order { get; set; }

        public string OrderId { get; set; }
    }
}
