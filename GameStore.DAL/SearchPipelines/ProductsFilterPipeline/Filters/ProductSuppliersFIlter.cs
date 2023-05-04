using GameStore.DAL.Entities.MongoEntities;
using GameStore.DAL.SearchPipelines.Enums;
using GameStore.DAL.SearchPipelines.ProductsFilterPipeline.Interfaces;
using MongoDB.Driver;
using System.Collections.Generic;

namespace GameStore.DAL.SearchPipelines.ProductsFilterPipeline.Filters
{
    public class ProductSuppliersFIlter : IProductsFilterPipelineStep
    {
        public ExecutionPriority Priority => ExecutionPriority.Average;

        public PipelineDefinition<ProductMongoEntity, ProductMongoEntity> Execute(PipelineDefinition<ProductMongoEntity, ProductMongoEntity> pipeline, ProductSearchRequest request)
        {
            if (request.Suppliers.Count == 0)
            {
                return pipeline;
            }

            var supplierIds = new List<int?>();
            foreach (var id in request.Suppliers)
            {
                if (int.TryParse(id, out int supplierId))
                {
                    supplierIds.Add(supplierId);
                }
            }

            var supplierFilter = Builders<ProductMongoEntity>.Filter.In(x => x.SupplierId, supplierIds);
            return pipeline.Match(supplierFilter);
        }
    }
}
