using System.ComponentModel;

namespace GameStore.PL.DTOs
{
    public class ShipperDTO
    {
        public int ShipperId { get; set; }

        [DisplayName("CompanyName")]
        public string CompanyName { get; set; }

        [DisplayName("Phone")]
        public string Phone { get; set; }
    }
}
