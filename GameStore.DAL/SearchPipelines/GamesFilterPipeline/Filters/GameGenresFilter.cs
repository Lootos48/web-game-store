using GameStore.DAL.Entities;
using GameStore.DAL.SearchPipelines.Enums;
using GameStore.DAL.SearchPipelines.GamesFilterPipeline.Interfaces;
using System.Linq;

namespace GameStore.DAL.SearchPipelines.GamesFilterPipeline.Filters
{
    public class GameGenresFilter : IGamesFilterPipelineStep
    {
        public ExecutionPriority Priority => ExecutionPriority.Average;

        public IQueryable<GameEntity> Execute(IQueryable<GameEntity> gamesQuery, GamesSearchRequest request)
        {
            if (request.Genres.Count == 0)
            {
                return gamesQuery;
            }

            return gamesQuery.Where(g => g.GameGenres.Any(gg => request.Genres.Any(i => i == gg.GenreId)));
        }
    }
}
