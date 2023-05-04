using System;

namespace GameStore.DAL.Entities
{
    public class PublisherSupplierMappingEntity : BaseEntity
    {
        public Guid PublisherId { get; set; }

        public int SupplierId { get; set; }
    }
}
