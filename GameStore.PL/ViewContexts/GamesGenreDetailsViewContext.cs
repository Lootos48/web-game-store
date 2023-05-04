using GameStore.PL.DTOs;
using System.Collections.Generic;

namespace GameStore.PL.ViewContexts
{
    public class GamesGenreDetailsViewContext
    {
        public GenreDTO Genre { get; set; }

        public List<GoodsDTO> GamesInGenre { get; set; }
    }
}
