using GameStore.DAL.Entities;
using GameStore.DAL.SearchPipelines.Enums;
using GameStore.DAL.SearchPipelines.GamesFilterPipeline.Interfaces;
using GameStore.DomainModels.Enums;
using System.Linq;

namespace GameStore.DAL.SearchPipelines.GamesFilterPipeline.Sorters
{
    public class GameCommentCountSorter : IGamesFilterPipelineStep
    {
        public ExecutionPriority Priority => ExecutionPriority.BelowAverage;

        public IQueryable<GameEntity> Execute(IQueryable<GameEntity> gamesQuery, GamesSearchRequest request)
        {
            if (request.SortBy != GamesSortType.MostCommented)
            {
                return gamesQuery;
            }

            return gamesQuery.OrderByDescending(g => g.Comments.Count);
        }
    }
}
