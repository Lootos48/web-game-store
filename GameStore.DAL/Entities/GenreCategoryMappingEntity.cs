using System;

namespace GameStore.DAL.Entities
{
    public class GenreCategoryMappingEntity : BaseEntity
    {
        public int CategoryId { get; set; }

        public Guid GenreId { get; set; }

        public GenreEntity Genre { get; set; }
    }
}
