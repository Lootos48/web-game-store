using GameStore.DAL.Entities.MongoEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.DAL.Repositories.MongoDbRepositories.Interfaces
{
    public interface IProductMongoRepository : IMongoRepository<ProductMongoEntity>
    {
        Task CreateProductKey();

        Task<ProductMongoEntity> FindByIdAsync(int productId);

        Task<ProductMongoEntity> FindByKeyAsync(string key);

        Task IncreaseViewCountAsync(int id);

        Task UpdateUnitsInStockAsync(string key, int unitsInStock);

        Task<List<ProductMongoEntity>> FindByCategoryAsync(int categoryId);

        Task BatchUpdateUnitsInStockAsync(List<ProductMongoEntity> productsList);

        Task<List<ProductMongoEntity>> FindByIdsAsync(List<int?> productIds);

        Task<List<ProductMongoEntity>> GetWithFiltersAsync(ProductSearchRequest request);

        Task<int> GetCountOfProductsAsync();

        Task<bool> IsProductUnique(ProductMongoEntity entity);
    }
}