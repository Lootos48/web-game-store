using GameStore.DAL.Entities.MongoEntities;
using GameStore.DAL.SearchPipelines.Enums;
using GameStore.DAL.SearchPipelines.ProductsFilterPipeline.Interfaces;
using GameStore.DomainModels.Enums;
using MongoDB.Driver;

namespace GameStore.DAL.SearchPipelines.ProductsFilterPipeline.Sorters
{
    public class ProductViewCountSorter : IProductsFilterPipelineStep
    {
        public ExecutionPriority Priority => ExecutionPriority.BelowAverage;

        public PipelineDefinition<ProductMongoEntity, ProductMongoEntity> Execute(PipelineDefinition<ProductMongoEntity, ProductMongoEntity> pipeline, ProductSearchRequest request)
        {
            if (request.SortBy != GamesSortType.MostViewed)
            {
                return pipeline;
            }

            var sort = Builders<ProductMongoEntity>.Sort.Descending("ViewCount");

            return pipeline.Sort(sort);
        }
    }
}
