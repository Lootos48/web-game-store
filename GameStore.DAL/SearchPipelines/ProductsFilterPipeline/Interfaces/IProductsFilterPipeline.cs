using GameStore.DAL.Entities.MongoEntities;
using MongoDB.Driver;

namespace GameStore.DAL.SearchPipelines.ProductsFilterPipeline.Interfaces
{
    public interface IProductsFilterPipeline
    {
        public PipelineDefinition<ProductMongoEntity, ProductMongoEntity> Execute(ProductSearchRequest request);
    }
}