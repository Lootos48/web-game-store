using GameStore.DAL.Entities.MongoEntities;
using GameStore.DAL.Repositories.MongoDbRepositories.Interfaces;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Threading.Tasks;

namespace GameStore.DAL.Repositories.MongoDbRepositories
{
    public class ShipperMongoRepository : MongoRepository<ShipperMongoEntity>, IShipperMongoRepository
    {
        public ShipperMongoRepository(IMongoDatabase mongodb) : base(mongodb) { }

        public Task<ShipperMongoEntity> FindById(int id)
        {
            return _collection.Find(x => x.ShipperId == id).FirstOrDefaultAsync();
        }
    }
}
