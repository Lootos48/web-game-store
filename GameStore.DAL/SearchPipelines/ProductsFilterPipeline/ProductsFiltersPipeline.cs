using GameStore.DAL.Entities.MongoEntities;
using GameStore.DAL.SearchPipelines.ProductsFilterPipeline.Interfaces;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace GameStore.DAL.SearchPipelines.ProductsFilterPipeline
{
    public class ProductsFiltersPipeline : IProductsFilterPipeline
    {
        private readonly IEnumerable<IProductsFilterPipelineStep> _pipelineSteps;

        public ProductsFiltersPipeline(IEnumerable<IProductsFilterPipelineStep> pipelineSteps)
        {
            _pipelineSteps = SortPipelineSteps(pipelineSteps);
        }

        public PipelineDefinition<ProductMongoEntity, ProductMongoEntity> Execute(ProductSearchRequest request)
        {
            PipelineDefinition<ProductMongoEntity, ProductMongoEntity> pipeline = new EmptyPipelineDefinition<ProductMongoEntity>();

            foreach (var pipelineStep in _pipelineSteps)
            {
                pipeline = pipelineStep.Execute(pipeline, request);
            }

            return pipeline;
        }

        private static IEnumerable<IProductsFilterPipelineStep> SortPipelineSteps(IEnumerable<IProductsFilterPipelineStep> pipelineSteps)
        {
            return pipelineSteps.OrderByDescending(step => step.Priority);
        }
    }
}
