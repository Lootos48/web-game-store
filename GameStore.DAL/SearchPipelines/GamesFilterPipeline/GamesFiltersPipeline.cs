using GameStore.DAL.Entities;
using GameStore.DAL.SearchPipelines.GamesFilterPipeline.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace GameStore.DAL.SearchPipelines.GamesFilterPipeline
{
    public class GamesFiltersPipeline : IGamesFiltersPipeline
    {
        private readonly IEnumerable<IGamesFilterPipelineStep> _pipelineSteps;

        public GamesFiltersPipeline(IEnumerable<IGamesFilterPipelineStep> pipelineSteps)
        {
            _pipelineSteps = SortPipelineSteps(pipelineSteps);
        }

        public IQueryable<GameEntity> Execute(IQueryable<GameEntity> gamesQuery, GamesSearchRequest request)
        {
            foreach (var pipelineStep in _pipelineSteps)
            {
                gamesQuery = pipelineStep.Execute(gamesQuery, request);
            }

            return gamesQuery;
        }

        private static IEnumerable<IGamesFilterPipelineStep> SortPipelineSteps(IEnumerable<IGamesFilterPipelineStep> pipelineSteps)
        {
            return pipelineSteps.OrderByDescending(step => step.Priority);
        }
    }
}
