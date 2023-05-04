using GameStore.DAL.Entities.MongoEntities;
using System.Threading.Tasks;

namespace GameStore.DAL.Repositories.MongoDbRepositories.Interfaces
{
    public interface IShipperMongoRepository : IMongoRepository<ShipperMongoEntity>
    {
        Task<ShipperMongoEntity> FindById(int id);
    }
}