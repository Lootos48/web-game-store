using System;

namespace GameStore.DomainModels.Models
{
    public class GameGenre
    {
        public Guid GameId { get; set; }

        public Goods Game { get; set; }

        public Guid GenreId { get; set; }

        public Genre Genre { get; set; }
    }
}
