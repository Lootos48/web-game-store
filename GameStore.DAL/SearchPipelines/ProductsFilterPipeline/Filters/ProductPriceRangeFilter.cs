using GameStore.DAL.Entities.MongoEntities;
using GameStore.DAL.SearchPipelines.Enums;
using GameStore.DAL.SearchPipelines.ProductsFilterPipeline.Interfaces;
using MongoDB.Driver;
using System;

namespace GameStore.DAL.SearchPipelines.ProductsFilterPipeline.Filters
{
    public class ProductPriceRangeFilter : IProductsFilterPipelineStep
    {
        public ExecutionPriority Priority => ExecutionPriority.Average;

        public PipelineDefinition<ProductMongoEntity, ProductMongoEntity> Execute(PipelineDefinition<ProductMongoEntity, ProductMongoEntity> pipeline, ProductSearchRequest request)
        {
            if (IsRangeValuesNotAssigned(request.MinPrice, request.MaxPrice))
            {
                return pipeline;
            }

            decimal minPrice = request.MinPrice;
            decimal maxPrice = request.MaxPrice;

            ValidatePriceRange(minPrice, maxPrice);

            var lowPriceFilter = Builders<ProductMongoEntity>.Filter.Gte(p => p.UnitPrice, request.MinPrice);
            if (maxPrice == 0)
            {
                return pipeline.Match(lowPriceFilter);
            }

            var highPriceFilter = Builders<ProductMongoEntity>.Filter.Lte(p => p.UnitPrice, request.MaxPrice);
            if (minPrice == 0)
            {
                return pipeline.Match(highPriceFilter);
            }

            var filter = Builders<ProductMongoEntity>.Filter.And(lowPriceFilter, highPriceFilter);

            return pipeline.Match(filter);
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
