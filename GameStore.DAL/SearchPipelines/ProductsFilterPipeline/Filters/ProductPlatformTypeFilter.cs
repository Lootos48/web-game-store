using GameStore.DAL.Entities.MongoEntities;
using GameStore.DAL.SearchPipelines.Enums;
using GameStore.DAL.SearchPipelines.ProductsFilterPipeline.Interfaces;
using MongoDB.Driver;

namespace GameStore.DAL.SearchPipelines.ProductsFilterPipeline.Filters
{
    /// <summary>
    /// There is no any relation in Mongo Products and PlatformType so it's just stub that realize logic for case if this relation was exist
    /// </summary>
    public class ProductPlatformTypeFilter : IProductsFilterPipelineStep
    {
        public ExecutionPriority Priority => ExecutionPriority.Average;

        public PipelineDefinition<ProductMongoEntity, ProductMongoEntity> Execute(PipelineDefinition<ProductMongoEntity, ProductMongoEntity> pipeline, ProductSearchRequest request)
        {
            if (request.PlatformTypes.Count == 0)
            {
                return pipeline;
            }

            var inFilter = Builders<ProductMongoEntity>.Filter.In("ProductPlatformType", request.PlatformTypes);
            var existFilter = Builders<ProductMongoEntity>.Filter.Exists("ProductPlatformType", true);

            var filter = Builders<ProductMongoEntity>.Filter.And(inFilter, existFilter);

            return pipeline.Match(filter);
        }
    }
}
