using GameStore.DAL.Entities.MongoEntities;
using GameStore.DAL.Repositories.MongoDbRepositories.Interfaces;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.DAL.Repositories.MongoDbRepositories
{
    public class SupplierMongoRepository : MongoRepository<SupplierMongoEntity>, ISupplierMongoRepository
    {
        public SupplierMongoRepository(IMongoDatabase mongodb) : base(mongodb) { }

        public Task<SupplierMongoEntity> FindByIdAsync(int supplierId)
        {
            return _collection.Find(x => x.SupplierId == supplierId).FirstOrDefaultAsync();
        }

        public Task<SupplierMongoEntity> FindByCompanyName(string companyName)
        {
            return _collection.Find(x => x.CompanyName == companyName).FirstOrDefaultAsync();
        }

        public Task<List<SupplierMongoEntity>> FindByIdsAsync(List<int?> suppliers)
        {
            var filter = Builders<SupplierMongoEntity>.Filter.In(x => x.SupplierId, suppliers);
            return _collection.Find(filter).ToListAsync();
        }

        public async Task<bool> IsSupplierUnique(string companyName)
        {
            bool result = await _collection.Find(x => x.CompanyName == companyName).AnyAsync();
            return !result;
        }
    }
}
