using GameStore.DomainModels.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GameStore.PL.DTOs.EditDTOs
{
    public class EditOrderRequestDTO
    {
        [Required]
        public Guid Id { get; set; }

        [DisplayName("CustomerId")]
        public Guid CustomerId { get; set; }

        [DisplayName("OrderDate")]
        public DateTime OrderDate { get; set; }

        [DisplayName("Status")]
        public OrderStatus Status { get; set; }

        public OrderStatus OldStatus { get; set; }

        [DisplayName("ExpirationDateUtc")]
        public DateTime? ExpirationDateUtc { get; set; }

        [DisplayName("ShippedDate")]
        public DateTime? ShippedDate { get; set; }

        [DisplayName("ShipVia")]
        public int? ShipVia { get; set; }

        [DisplayName("Freight")]
        public double? Freight { get; set; }

        [DisplayName("ShipName")]
        public string ShipName { get; set; }

        [DisplayName("ShipAddress")]
        public string ShipAddress { get; set; }

        [DisplayName("ShipCity")]
        public string ShipCity { get; set; }

        [DisplayName("ShipRegion")]
        public string ShipRegion { get; set; }

        [DisplayName("ShipPostalCode")]
        public string ShipPostalCode { get; set; }

        [DisplayName("ShipCountry")]
        public string ShipCountry { get; set; }

        public List<OrderDetailsDTO> OrderDetails { get; set; } = new List<OrderDetailsDTO>();
    }
}
