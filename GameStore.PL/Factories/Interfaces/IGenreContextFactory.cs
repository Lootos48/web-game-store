using GameStore.PL.ViewContexts;
using System.Threading.Tasks;

namespace GameStore.PL.Factories.Interfaces
{
    public interface IGenreContextFactory
    {
        Task<GenreCreationViewContext> BuildGenreCreationViewContextAsync();

        Task<GamesGenreDetailsViewContext> BuildGamesGenreDetailsViewContextAsync(string genre, string localizationCode = null);

        Task<GenreEditingViewContext> BuildGenreEditingViewContextAsync(string genre, string cultureCode);
    }
}
