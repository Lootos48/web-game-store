using GameStore.DAL.Entities.MongoEntities;
using GameStore.DAL.SearchPipelines.Enums;
using GameStore.DAL.SearchPipelines.ProductsFilterPipeline.Interfaces;
using MongoDB.Driver;

namespace GameStore.DAL.SearchPipelines.ProductsFilterPipeline.Filters
{
    public class ProductExcludeMigratedFilter : IProductsFilterPipelineStep
    {
        public ExecutionPriority Priority => ExecutionPriority.Critical;

        public PipelineDefinition<ProductMongoEntity, ProductMongoEntity> Execute(PipelineDefinition<ProductMongoEntity, ProductMongoEntity> pipeline, ProductSearchRequest request)
        {
            var filter = Builders<ProductMongoEntity>.Filter.Nin(x => x.ProductKey, request.ExcludedProductKeys);

            return pipeline.Match(filter);
        }
    }
}
