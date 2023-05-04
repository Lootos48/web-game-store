using GameStore.DAL.Entities;
using System.Linq;

namespace GameStore.DAL.SearchPipelines.GamesFilterPipeline.Interfaces
{
    public interface IGamesFiltersPipeline
    {
        public IQueryable<GameEntity> Execute(IQueryable<GameEntity> gamesQuery, GamesSearchRequest request);
    }
}
