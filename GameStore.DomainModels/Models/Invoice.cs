namespace GameStore.DomainModels.Models
{
    public class Invoice
    {
        public Order Order { get; set; }

        public decimal TotalPrice { get; set; }
    }
}
