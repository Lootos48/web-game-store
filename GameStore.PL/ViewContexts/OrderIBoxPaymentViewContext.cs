using GameStore.PL.DTOs;
using System.ComponentModel.DataAnnotations;

namespace GameStore.PL.ViewContexts
{
    public class OrderIBoxPaymentViewContext
    {
        public OrderDTO Order { get; set; }

        [DataType(DataType.Currency)]
        public decimal TotalPrice { get; set; }
    }
}
