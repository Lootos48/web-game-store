using GameStore.DAL.Entities;
using GameStore.DAL.SearchPipelines.Enums;
using GameStore.DAL.SearchPipelines.GamesFilterPipeline.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameStore.DAL.SearchPipelines.GamesFilterPipeline.Filters
{
    public class GamePublishersFilter : IGamesFilterPipelineStep
    {
        public ExecutionPriority Priority => ExecutionPriority.Average;

        public IQueryable<GameEntity> Execute(IQueryable<GameEntity> gamesQuery, GamesSearchRequest request)
        {
            if (request.Publishers is null || request.Publishers.Count == 0)
            {
                return gamesQuery;
            }

            var publishersIds = new List<Guid>();
            foreach (var id in request.Publishers)
            {
                if (Guid.TryParse(id, out Guid publisherId))
                {
                    publishersIds.Add(publisherId);
                }
            }

            return gamesQuery.Where(game => publishersIds.Contains(game.PublisherId.Value));
        }
    }
}
