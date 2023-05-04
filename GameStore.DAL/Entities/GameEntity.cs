using GameStore.DAL.Entities.Localizations;
using System;
using System.Collections.Generic;

namespace GameStore.DAL.Entities
{
    public class GameEntity : Entity
    {
        public string Key { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public short UnitsInStock { get; set; }

        public bool Discontinued { get; set; }

        public string QuantityPerUnit { get; set; }

        public int? UnitsOnOrder { get; set; }

        public int? ReorderLevel { get; set; }

        public DateTime? DateOfPublishing { get; set; }

        public DateTime DateOfAdding { get; set; }

        public long ViewCount { get; set; }

        public ICollection<CommentEntity> Comments { get; } = new List<CommentEntity>();

        public ICollection<GamePlatformTypeEntity> GamesPlatformTypes { get; } = new List<GamePlatformTypeEntity>();

        public ICollection<GameGenreEntity> GameGenres { get; } = new List<GameGenreEntity>();

        public ICollection<GameLocalizationEntity> Localizations { get; set; }

        public PublisherEntity Publisher { get; set; }

        public Guid? PublisherId { get; set; }
    }
}
