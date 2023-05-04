using GameStore.DAL.Entities;
using GameStore.DAL.SearchPipelines.Enums;
using GameStore.DAL.SearchPipelines.GamesFilterPipeline.Interfaces;
using System;
using System.Linq;

namespace GameStore.DAL.SearchPipelines.GamesFilterPipeline.Filters
{
    public class GamePriceRangeFilter : IGamesFilterPipelineStep
    {
        public ExecutionPriority Priority => ExecutionPriority.Average;

        public IQueryable<GameEntity> Execute(IQueryable<GameEntity> gamesQuery, GamesSearchRequest request)
        {
            if (IsRangeValuesNotAssigned(request.MinPrice, request.MaxPrice))
            {
                return gamesQuery;
            }

            decimal minPrice = request.MinPrice;
            decimal maxPrice = request.MaxPrice;

            ValidatePriceRange(minPrice, maxPrice);

            if (maxPrice == 0)
            {
                return gamesQuery.Where(g => g.Price >= minPrice);
            }

            if (minPrice == 0)
            {
                return gamesQuery.Where(g => g.Price <= maxPrice);
            }

            return gamesQuery.Where(g => g.Price >= minPrice && g.Price <= maxPrice);
        }

        private static bool IsRangeValuesNotAssigned(decimal minPrice, decimal maxPrice)
        {
            return minPrice == 0 && maxPrice == 0;
        }

        private static void ValidatePriceRange(decimal minPrice, decimal maxPrice)
        {
            if (maxPrice != 0 && minPrice >= maxPrice)
            {
                throw new ArgumentException("Min price can`t be bigger than max price");
            }

            if (minPrice < 0 || maxPrice < 0)
            {
                throw new ArgumentException("Range cannot be less than zero");
            }
        }
    }
}
