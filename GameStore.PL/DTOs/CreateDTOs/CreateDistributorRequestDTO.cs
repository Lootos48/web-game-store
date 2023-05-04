using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GameStore.PL.DTOs.CreateDTOs
{
    public class CreateDistributorRequestDTO
    {
        [Required(ErrorMessage = "CompanyNameRequired")]
        [DisplayName("CompanyName")]
        [StringLength(150, ErrorMessage = "CompanyNameLength")]
        public string CompanyName { get; set; }

        [DisplayName("Description")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [DisplayName("HomePage")]
        [DataType(DataType.Url)]
        public string HomePage { get; set; }

        public string ContactName { get; set; }

        public string ContactTitle { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string Region { get; set; }

        public string PostalCode { get; set; }

        public string Country { get; set; }

        public string Phone { get; set; }

        public string Fax { get; set; }
    }
}
