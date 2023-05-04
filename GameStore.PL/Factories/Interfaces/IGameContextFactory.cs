using GameStore.PL.DTOs;
using GameStore.PL.DTOs.EditDTOs;
using GameStore.PL.ViewContexts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.PL.Factories.Interfaces
{
    public interface IGameContextFactory
    {
        Task<GameCreationViewContext> BuildGameCreationContextAsync();

        Task<GameEditingViewContext> BuildGameEditingContextAsync(EditGoodsRequestDTO gameEdit);

        Task<GamesViewsContext> BuildGamesViewContextAsync(List<GoodsDTO> filteredGames = null, string localizationCultureCode = null);
    }
}
