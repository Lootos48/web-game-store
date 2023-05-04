using GameStore.DAL.Entities;
using GameStore.DAL.SearchPipelines.Enums;
using GameStore.DAL.SearchPipelines.GamesFilterPipeline.Interfaces;
using System;
using System.Linq;

namespace GameStore.DAL.SearchPipelines.GamesFilterPipeline.Filters
{
    public class GamePaginationFilter : IGamesFilterPipelineStep
    {
        public ExecutionPriority Priority => ExecutionPriority.Low;

        public IQueryable<GameEntity> Execute(IQueryable<GameEntity> gamesQuery, GamesSearchRequest request)
        {
            if (IsRangeValuesNotAssigned(request.CurrentPage, request.ItemsPerPage))
            {
                return gamesQuery;
            }

            ValidatePageValues(request.CurrentPage, request.ItemsPerPage);

            int take = (int) (request.ItemsPerPage * request.CurrentPage * 1.5);

            return gamesQuery.Take(take);
        }

        private static bool IsRangeValuesNotAssigned(int CurrentPage, int ItemsPerPage)
        {
            return CurrentPage == 0 && ItemsPerPage == 0;
        }

        private static void ValidatePageValues(int CurrentPage, int ItemsPerPage)
        {
            if (ItemsPerPage <= 0)
            {
                throw new ArgumentException("Items per page must be greater than zero");
            }

            if (CurrentPage < 0)
            {
                throw new ArgumentException("Current page cannot be less than zero");
            }
        }
    }
}
