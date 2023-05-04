using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GameStore.PL.DTOs.EditDTOs
{
    public class EditOrderDetailsRequestDTO
    {
        [Required]
        public Guid Id { get; set; }

        [DisplayName("Price")]
        public decimal Price { get; set; }

        [DisplayName("Quantity")]
        public short Quantity { get; set; }

        [DisplayName("Discount")]
        public double Discount { get; set; }
    }
}
