using System;

namespace GameStore.DomainModels.Models
{
    public class PublisherSupplierMapping
    {
        public Guid PublisherId { get; set; }

        public int SupplierId { get; set; }
    }
}
