using GameStore.DomainModels.Enums;
using System;
using System.Collections.Generic;

namespace GameStore.DomainModels.Models.EditModels
{
    public class EditOrderRequest
    {
        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }

        public DateTime OrderDate { get; set; }

        public OrderStatus Status { get; set; }

        public OrderStatus OldStatus { get; set; }

        public DateTime? ExpirationDateUtc { get; set; }

        public DateTime? ShippedDate { get; set; }

        public int? ShipVia { get; set; }

        public double? Freight { get; set; }

        public string ShipName { get; set; }

        public string ShipAddress { get; set; }

        public string ShipCity { get; set; }

        public string ShipRegion { get; set; }

        public string ShipPostalCode { get; set; }

        public string ShipCountry { get; set; }

        public List<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();
    }
}
