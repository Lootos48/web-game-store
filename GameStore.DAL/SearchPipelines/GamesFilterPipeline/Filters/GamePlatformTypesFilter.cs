using GameStore.DAL.Entities;
using GameStore.DAL.SearchPipelines.Enums;
using GameStore.DAL.SearchPipelines.GamesFilterPipeline.Interfaces;
using System.Linq;

namespace GameStore.DAL.SearchPipelines.GamesFilterPipeline.Filters
{
    public class GamePlatformTypesFilter : IGamesFilterPipelineStep
    {
        public ExecutionPriority Priority => ExecutionPriority.Average;

        public IQueryable<GameEntity> Execute(IQueryable<GameEntity> gamesQuery, GamesSearchRequest request)
        {
            if (request.PlatformTypes.Count == 0)
            {
                return gamesQuery;
            }

            return gamesQuery.Where(g => g.GamesPlatformTypes
                .Any(gg => request.PlatformTypes
                    .Any(i => i == gg.PlatformId)));
        }
    }
}