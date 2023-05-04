using GameStore.DAL.Entities.MongoEntities;
using GameStore.DAL.SearchPipelines.Enums;
using GameStore.DAL.SearchPipelines.ProductsFilterPipeline.Interfaces;
using MongoDB.Driver;
using System;

namespace GameStore.DAL.SearchPipelines.ProductsFilterPipeline.Filters
{
    /// <summary>
    /// There is no any field PublishingDate in Mongo Products so it's just stub that realize logic for case if this field was exist
    /// </summary>
    public class ProductSupplierDateFilter : IProductsFilterPipelineStep
    {
        public ExecutionPriority Priority => ExecutionPriority.Average;

        public PipelineDefinition<ProductMongoEntity, ProductMongoEntity> Execute(PipelineDefinition<ProductMongoEntity, ProductMongoEntity> pipeline, ProductSearchRequest request)
        {
            if (request.CountOfDaysBeforePublishingDate == 0)
            {
                return pipeline;
            }

            DateTime minDate = DateTime.UtcNow.AddDays(-request.CountOfDaysBeforePublishingDate);

            var greaterThanDateFilter = Builders<ProductMongoEntity>.Filter.Gte("PublishingDate", minDate);
            var existFilter = Builders<ProductMongoEntity>.Filter.Exists("PublishingDate", true);

            var filter = Builders<ProductMongoEntity>.Filter.And(greaterThanDateFilter, existFilter);

            return pipeline.Match(filter);
        }
    }
}
