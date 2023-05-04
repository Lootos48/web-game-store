using GameStore.DAL.Entities.MongoEntities;
using GameStore.DAL.SearchPipelines.Enums;
using MongoDB.Driver;

namespace GameStore.DAL.SearchPipelines.ProductsFilterPipeline.Interfaces
{
    public interface IProductsFilterPipelineStep
    {
        public ExecutionPriority Priority { get; }

        public PipelineDefinition<ProductMongoEntity, ProductMongoEntity> Execute(PipelineDefinition<ProductMongoEntity, ProductMongoEntity> pipeline, ProductSearchRequest request);
    }
}
