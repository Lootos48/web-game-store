using GameStore.DAL.Entities.MongoEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.DAL.Repositories.MongoDbRepositories.Interfaces
{
    public interface IOrderDetailsMongoRepository : IMongoRepository<OrderDetailsMongoEntity>
    {
        Task<OrderDetailsMongoEntity> FindOrderDetails(int orderId, int productId);

        Task<List<OrderDetailsMongoEntity>> FindOrderDetailsByOrderId(int orderId);
    }
}