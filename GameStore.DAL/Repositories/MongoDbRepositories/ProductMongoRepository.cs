using GameStore.DAL.Entities.MongoEntities;
using GameStore.DAL.Repositories.MongoDbRepositories.Interfaces;
using GameStore.DAL.SearchPipelines.ProductsFilterPipeline.Interfaces;
using GameStore.DAL.Util.CacheManagers.Interfaces;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore.DAL.Repositories.MongoDbRepositories
{
    public class ProductMongoRepository : MongoRepository<ProductMongoEntity>, IProductMongoRepository
    {
        private readonly ICacheManager _cacheManager;

        private readonly IProductsFilterPipeline _filtersPipeline;

        public ProductMongoRepository(
            IMongoDatabase mongodb,
            ICacheManager cache,
            IProductsFilterPipeline filtersPipeline) : base(mongodb)
        {
            _cacheManager = cache;

            _filtersPipeline = filtersPipeline;
        }

        public override async Task<List<ProductMongoEntity>> GetAllAsync()
        {
            await CreateProductKey();

            var filter = await GetExcludeMigratedFilter();

            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<int> GetCountOfProductsAsync()
        {
            var filter = await GetExcludeMigratedFilter();

            var productsCount = await _collection.CountDocumentsAsync(filter);

            return (int)productsCount;
        }

        public async Task<ProductMongoEntity> FindByIdAsync(int productId)
        {
            await CreateProductKey();

            return await _collection.Find(x => x.ProductId.Value == productId).FirstOrDefaultAsync();
        }

        public async Task<ProductMongoEntity> FindByKeyAsync(string key)
        {
            await CreateProductKey();

            return await _collection.Find(x => x.ProductKey == key).FirstOrDefaultAsync();
        }

        public async Task<bool> IsProductUnique(ProductMongoEntity entity)
        {
            bool result = await _collection.Find(x => x.ProductKey == entity.ProductKey
                && x.ProductId != entity.ProductId).AnyAsync();
            return !result;
        }

        public Task CreateProductKey()
        {
            var withoutKeysFilter = Builders<ProductMongoEntity>.Filter.Exists(x => x.ProductKey, false);

            var productsWithoutKey = _collection.Find(withoutKeysFilter).ToList();
            if (!productsWithoutKey.Any())
            {
                return Task.CompletedTask;
            }

            var batchUpdateModel = new List<WriteModel<ProductMongoEntity>>();

            foreach (var product in productsWithoutKey)
            {
                product.ProductKey = $"{product.ProductName}_{product.ProductId}";

                var filter = Builders<ProductMongoEntity>.Filter.Eq(x => x.ProductId, product.ProductId);
                var update = Builders<ProductMongoEntity>.Update.Set(x => x.ProductKey, product.ProductKey);

                var updateOne = new UpdateOneModel<ProductMongoEntity>(filter, update);
                batchUpdateModel.Add(updateOne);
            }

            return _collection.BulkWriteAsync(batchUpdateModel);
        }

        public async Task IncreaseViewCountAsync(int id)
        {
            var idFilter = Builders<ProductMongoEntity>.Filter.Eq(x => x.ProductId, id);
            var viewCountExistFilter = Builders<ProductMongoEntity>.Filter.Exists(x => x.ViewCount, true);

            var andFilter = Builders<ProductMongoEntity>.Filter
                .And(new List<FilterDefinition<ProductMongoEntity>> { idFilter, viewCountExistFilter });

            var incrementUpdate = Builders<ProductMongoEntity>.Update.Inc(x => x.ViewCount, 1);

            var product = await _collection.FindOneAndUpdateAsync(andFilter, incrementUpdate);
            if (product == null)
            {
                var setUpdate = Builders<ProductMongoEntity>.Update.Set(x => x.ViewCount, 1);

                await _collection.UpdateOneAsync(idFilter, setUpdate);
            }
        }

        public Task UpdateUnitsInStockAsync(string key, int unitsInStock)
        {
            var filter = Builders<ProductMongoEntity>.Filter.Eq(x => x.ProductKey, key);
            var update = Builders<ProductMongoEntity>.Update.Set(x => x.UnitsInStock, unitsInStock);

            return _collection.UpdateOneAsync(filter, update);
        }

        public Task<List<ProductMongoEntity>> FindByCategoryAsync(int categoryId)
        {
            var filter = Builders<ProductMongoEntity>.Filter.Eq(x => x.CategoryId, categoryId);

            return _collection.Find(filter).ToListAsync();
        }

        public Task BatchUpdateUnitsInStockAsync(List<ProductMongoEntity> productsList)
        {
            if (!productsList.Any())
            {
                return Task.CompletedTask;
            }

            var batchUpdateModel = new List<WriteModel<ProductMongoEntity>>();

            foreach (var product in productsList)
            {
                var filter = Builders<ProductMongoEntity>.Filter.Eq(x => x.ProductKey, product.ProductKey);
                var update = Builders<ProductMongoEntity>.Update.Set(x => x.UnitsInStock, product.UnitsInStock);

                var updateOne = new UpdateOneModel<ProductMongoEntity>(filter, update);

                batchUpdateModel.Add(updateOne);
            }

            return _collection.BulkWriteAsync(batchUpdateModel);
        }

        public Task<List<ProductMongoEntity>> FindByIdsAsync(List<int?> productIds)
        {
            var filter = Builders<ProductMongoEntity>.Filter.In(x => x.ProductId, productIds);
            return _collection.Find(filter).ToListAsync();
        }

        public Task<List<ProductMongoEntity>> GetWithFiltersAsync(ProductSearchRequest request)
        {
            var pipeline = _filtersPipeline.Execute(request);
            return _collection.Aggregate(pipeline).ToListAsync();
        }

        private async Task<FilterDefinition<ProductMongoEntity>> GetExcludeMigratedFilter()
        {
            var goodsProductMappings = await _cacheManager.GetGoodsProductMappingsCacheAsync();
            List<string> excludedProductKeys = goodsProductMappings.Select(x => x.GameKey).ToList();

            return Builders<ProductMongoEntity>.Filter.Nin(x => x.ProductKey, excludedProductKeys);
        }
    }
}
