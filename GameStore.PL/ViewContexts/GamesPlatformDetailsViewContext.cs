using GameStore.PL.DTOs;
using System.Collections.Generic;

namespace GameStore.PL.ViewContexts
{
    public class GamesPlatformDetailsViewContext
    {
        public PlatformTypeDTO PlatformDTO { get; set; }

        public List<GoodsDTO> GamesOnPlatform { get; set; }
    }
}
