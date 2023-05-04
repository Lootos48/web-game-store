using System;

namespace GameStore.DAL.Entities.Localizations
{
    public class LocalizationEntity : BaseEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string CultureCode { get; set; }
    }
}
