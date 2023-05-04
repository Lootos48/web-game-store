using GameStore.PL.DTOs;
using System.Collections.Generic;

namespace GameStore.PL.ViewContexts
{
    public class GamesViewsContext
    {
        public List<GenreDTO> GenresFilterOptions { get; set; }

        public List<DistributorDTO> PublishersFilterOptions { get; set; }

        public List<PlatformTypeDTO> PlatformsFilterOptions { get; set; }

        public List<GoodsDTO> Games { get; set; }

        public GoodsSearchRequestDTO Filters { get; set; }
    }
}
