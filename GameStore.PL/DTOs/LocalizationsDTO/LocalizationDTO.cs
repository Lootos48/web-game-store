using System;
using System.ComponentModel;

namespace GameStore.PL.DTOs.LocalizationsDTO
{
    public class LocalizationDTO
    {
        public Guid Id { get; set; }

        [DisplayName("Name")]
        public string Name { get; set; }

        [DisplayName("CultureCode")]
        public string CultureCode { get; set; }
    }
}
