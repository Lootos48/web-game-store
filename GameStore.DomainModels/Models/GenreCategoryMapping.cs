using System;

namespace GameStore.DomainModels.Models
{
    public class GenreCategoryMapping
    {
        public Guid GenreId { get; set; }

        public Genre Genre { get; set; }

        public int CategoryId { get; set; }
    }
}
