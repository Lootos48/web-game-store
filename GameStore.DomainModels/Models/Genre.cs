using GameStore.DomainModels.Models.Localizations;
using System;
using System.Collections.Generic;

namespace GameStore.DomainModels.Models
{
    public class Genre
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Genre Parent { get; set; }

        public Guid? ParentId { get; set; }

        public List<Genre> SubGenres { get; set; } = new List<Genre>();

        public List<GenreLocalization> Localizations { get; set; }
    }
}
