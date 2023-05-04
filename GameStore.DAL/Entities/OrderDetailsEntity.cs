using System;

namespace GameStore.DAL.Entities
{
    public class OrderDetailsEntity : Entity
    {
        public decimal Price { get; set; }

        public short Quantity { get; set; }

        public double Discount { get; set; }

        public Guid ProductId { get; set; }

        public GameEntity Product { get; set; }

        public OrderEntity Order { get; set; }

        public Guid OrderId { get; set; }
    }
}
