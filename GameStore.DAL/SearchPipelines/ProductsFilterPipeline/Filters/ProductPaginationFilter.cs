using GameStore.DAL.Entities.MongoEntities;
using GameStore.DAL.SearchPipelines.Enums;
using GameStore.DAL.SearchPipelines.ProductsFilterPipeline.Interfaces;
using MongoDB.Driver;
using System;

namespace GameStore.DAL.SearchPipelines.ProductsFilterPipeline.Filters
{
    public class ProductPaginationFilter : IProductsFilterPipelineStep
    {
        public ExecutionPriority Priority => ExecutionPriority.Low;

        public PipelineDefinition<ProductMongoEntity, ProductMongoEntity> Execute(PipelineDefinition<ProductMongoEntity, ProductMongoEntity> pipeline, ProductSearchRequest request)
        {
            if (IsRangeValuesNotAssigned(request.CurrentPage, request.ItemsPerPage))
            {
                return pipeline;
            }

            ValidatePageValues(request.CurrentPage, request.ItemsPerPage);

            int limit = (int)(request.ItemsPerPage * request.CurrentPage * 1.5);

            return pipeline.Limit(limit);
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
