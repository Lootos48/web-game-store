using GameStore.DAL.Entities;
using GameStore.DAL.SearchPipelines.Enums;
using GameStore.DAL.SearchPipelines.GamesFilterPipeline.Interfaces;
using System;
using System.Linq;

namespace GameStore.DAL.SearchPipelines.GamesFilterPipeline.Filters
{
    public class GameDateOfPublishingFilter : IGamesFilterPipelineStep
    {
        public ExecutionPriority Priority => ExecutionPriority.Average;

        public IQueryable<GameEntity> Execute(IQueryable<GameEntity> gamesQuery, GamesSearchRequest request)
        {
            if (request.CountOfDaysBeforePublishingDate == 0)
            {
                return gamesQuery;
            }

            DateTime minDate = DateTime.UtcNow.AddDays(-request.CountOfDaysBeforePublishingDate);

            return gamesQuery.Where(g => g.DateOfPublishing >= minDate);
        }
    }
}
