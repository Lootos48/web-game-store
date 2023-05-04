using GameStore.DomainModels.Enums;
using System;
using System.Collections.Generic;

namespace GameStore.DomainModels.Models
{
    public class Order
    {
        public string Id { get; set; }

        public Guid CustomerId { get; set; }

        public DateTime OrderDate { get; set; }

        public OrderStatus Status { get; set; }

        public DateTime? ExpirationDateUtc { get; set; }

        public DateTime? RequiredDate { get; set; }

        public DateTime? ShippedDate { get; set; }

        public int? ShipVia { get; set; }

        public double? Freight { get; set; }

        public string ShipName { get; set; }

        public string ShipAddress { get; set; }

        public string ShipCity { get; set; }

        public string ShipRegion { get; set; }

        public string ShipPostalCode { get; set; }

        public string ShipCountry { get; set; }

        public Shipper Shipper { get; set; }

        public List<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();

        public decimal TotalPrice
        {
            get
            {
                decimal orderFinalPrice = 0;

                foreach (var orderDetail in OrderDetails)
                {
                    decimal orderSum = orderDetail.Price * orderDetail.Quantity;
                    orderFinalPrice += orderSum - orderSum * (decimal)orderDetail.Discount;
                }

                return orderFinalPrice;
            }
        }
    }
}
