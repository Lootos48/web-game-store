using GameStore.DAL.Entities.MongoEntities;
using GameStore.DAL.SearchPipelines.Enums;
using GameStore.DAL.SearchPipelines.ProductsFilterPipeline.Interfaces;
using MongoDB.Driver;

namespace GameStore.DAL.SearchPipelines.ProductsFilterPipeline.Filters
{
    public class ProductNameFilter : IProductsFilterPipelineStep
    {
        public ExecutionPriority Priority => ExecutionPriority.Average;

        public PipelineDefinition<ProductMongoEntity, ProductMongoEntity> Execute(PipelineDefinition<ProductMongoEntity, ProductMongoEntity> pipeline, ProductSearchRequest request)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                return pipeline;
            }

            var nameFilter = Builders<ProductMongoEntity>.Filter.Regex(x => x.ProductName, @$"/^\w*{request.Name.ToLower()}\w*/i");
            return pipeline.Match(nameFilter);
        }
    }
}
