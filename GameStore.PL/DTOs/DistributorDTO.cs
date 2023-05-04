using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GameStore.PL.DTOs
{
    public class DistributorDTO
    {
        public string Id { get; set; }

        [DisplayName("CompanyName")]
        public string CompanyName { get; set; }

        [DisplayName("Description")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [DisplayName("HomePage")]
        [DataType(DataType.Url)]
        public string HomePage { get; set; }

        [DisplayName("ContactName")]
        public string ContactName { get; set; }

        [DisplayName("ContactTitle")]
        public string ContactTitle { get; set; }

        [DisplayName("Address")]
        public string Address { get; set; }

        [DisplayName("City")]
        public string City { get; set; }

        [DisplayName("Region")]
        public string Region { get; set; }

        [DisplayName("PostalCode")]
        public string PostalCode { get; set; }

        [DisplayName("Country")]
        public string Country { get; set; }

        [DisplayName("Phone")]
        public string Phone { get; set; }

        [DisplayName("Fax")]
        public string Fax { get; set; }

        public List<string> DistributedGoods { get; set; } = new List<string>();
    }
}
