using System;

namespace GameStore.DomainModels.Models.CreateModels
{
    public class CreateOrderRequest
    {
        public Guid CustomerId { get; set; }

        public DateTime OrderDate { get; set; }
    }
}
