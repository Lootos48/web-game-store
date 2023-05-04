using System;

namespace GameStore.DAL.Entities
{
    public abstract class Entity : BaseEntity
    {
        public Guid Id { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DateOfDelete { get; set; }
    }
}
