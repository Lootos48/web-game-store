using GameStore.DAL.Entities.Localizations;
using System;
using System.Collections.Generic;

namespace GameStore.DAL.Entities
{
    public class GenreEntity : Entity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Picture { get; set; }

        public GenreEntity Parent { get; set; }

        public Guid? ParentId { get; set; }

        public ICollection<GenreLocalizationEntity> Localizations { get; } = new List<GenreLocalizationEntity>();

        public ICollection<GenreEntity> SubGenres { get; } = new List<GenreEntity>();

        public ICollection<GameGenreEntity> GameGenres { get; } = new List<GameGenreEntity>();
    }
}
