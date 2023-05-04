using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GameStore.PL.DTOs.EditDTOs
{
    public class EditDistributorRequestDTO
    {
        [Required]
        public string Id { get; set; }

        [Required(ErrorMessage = "CompanyNameRequired")]
        [DisplayName("CompanyName")]
        [DataType(DataType.Text)]
        public string CompanyName { get; set; }

        [DisplayName("Description")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [DisplayName("Homepage")]
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

        public Guid? UserId { get; set; }
    }
}
