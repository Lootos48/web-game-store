using GameStore.DAL.Entities.MongoEntities;
using GameStore.DAL.SearchPipelines.Enums;
using GameStore.DAL.SearchPipelines.ProductsFilterPipeline.Interfaces;
using MongoDB.Driver;
using System.Collections.Generic;

namespace GameStore.DAL.SearchPipelines.ProductsFilterPipeline.Filters
{
    public class ProductCategoryFilter : IProductsFilterPipelineStep
    {
        public ExecutionPriority Priority => ExecutionPriority.Average;

        public PipelineDefinition<ProductMongoEntity, ProductMongoEntity> Execute(PipelineDefinition<ProductMongoEntity, ProductMongoEntity> pipeline, ProductSearchRequest request)
        {
            if (request.Categories.Count == 0)
            {
                return pipeline;
            }

            var categoriesIds = new List<int?>();
            foreach (var id in request.Categories)
            {
                if (int.TryParse(id, out int categoryId))
                {
                    categoriesIds.Add(categoryId);
                }
            }

            var supplierFilter = Builders<ProductMongoEntity>.Filter.In(x => x.CategoryId, categoriesIds);
            return pipeline.Match(supplierFilter);
        }
    }
}
