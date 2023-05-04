using GameStore.DAL.Entities.MongoEntities;
using GameStore.DAL.SearchPipelines.Enums;
using GameStore.DAL.SearchPipelines.ProductsFilterPipeline.Interfaces;
using GameStore.DomainModels.Enums;
using MongoDB.Driver;

namespace GameStore.DAL.SearchPipelines.ProductsFilterPipeline.Sorters
{
    public class ProductPriceSorter : IProductsFilterPipelineStep
    {
        public ExecutionPriority Priority => ExecutionPriority.BelowAverage;

        public PipelineDefinition<ProductMongoEntity, ProductMongoEntity> Execute(PipelineDefinition<ProductMongoEntity, ProductMongoEntity> pipeline, ProductSearchRequest request)
        {
            var builder = Builders<ProductMongoEntity>.Sort;

            if (request.SortBy == GamesSortType.PriceAscending)
            {
                var ascendingSort = builder.Ascending("UnitPrice");
                return pipeline.Sort(ascendingSort);
            }
            else if (request.SortBy == GamesSortType.PriceDescending)
            {
                var descendingSort = builder.Descending("UnitPrice");
                return pipeline.Sort(descendingSort);
            }

            return pipeline;
        }
    }
}
