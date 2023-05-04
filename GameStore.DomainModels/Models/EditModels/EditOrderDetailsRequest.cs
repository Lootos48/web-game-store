using System;

namespace GameStore.DomainModels.Models.EditModels
{
    public class EditOrderDetailsRequest
    {
        public Guid Id { get; set; }

        public decimal Price { get; set; }

        public short Quantity { get; set; }

        public double Discount { get; set; }
    }
}
