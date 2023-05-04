using GameStore.DAL.Entities.MongoEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.DAL.Repositories.MongoDbRepositories.Interfaces
{
    public interface ISupplierMongoRepository : IMongoRepository<SupplierMongoEntity>
    {
        Task<SupplierMongoEntity> FindByCompanyName(string companyName);

        Task<SupplierMongoEntity> FindByIdAsync(int supplierId);

        Task<List<SupplierMongoEntity>> FindByIdsAsync(List<int?> suppliers);
        
        Task<bool> IsSupplierUnique(string companyName);
    }
}