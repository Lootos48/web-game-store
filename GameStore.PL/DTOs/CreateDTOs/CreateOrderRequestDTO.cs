using System;

namespace GameStore.PL.DTOs.CreateDTOs
{
    public class CreateOrderRequestDTO
    {
        public Guid CustomerId { get; set; }

        public DateTime OrderDate { get; set; }
    }
}
