using GameStore.DAL.Entities.MongoEntities;
using GameStore.DAL.SearchPipelines.Enums;
using GameStore.DAL.SearchPipelines.ProductsFilterPipeline.Interfaces;
using GameStore.DomainModels.Enums;
using MongoDB.Driver;

namespace GameStore.DAL.SearchPipelines.ProductsFilterPipeline.Sorters
{
    public class ProductNewnessSorter : IProductsFilterPipelineStep
    {
        public ExecutionPriority Priority => ExecutionPriority.BelowAverage;

        public PipelineDefinition<ProductMongoEntity, ProductMongoEntity> Execute(PipelineDefinition<ProductMongoEntity, ProductMongoEntity> pipeline, ProductSearchRequest request)
        {
            if (request.SortBy != GamesSortType.Newest)
            {
                return pipeline;
            }

            return pipeline;
        }
    }
}
