using GameStore.DAL.Entities;
using GameStore.DAL.SearchPipelines.Enums;
using System.Linq;

namespace GameStore.DAL.SearchPipelines.GamesFilterPipeline.Interfaces
{
    public interface IGamesFilterPipelineStep
    {
        public ExecutionPriority Priority { get; }

        public IQueryable<GameEntity> Execute(IQueryable<GameEntity> gamesQuery, GamesSearchRequest request);
    }
}
