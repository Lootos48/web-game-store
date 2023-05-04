using GameStore.DAL.Entities.MongoEntities;
using GameStore.DAL.Repositories.MongoDbRepositories.Interfaces;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.DAL.Repositories.MongoDbRepositories
{
    public class OrderDetailsMongoRepository : MongoRepository<OrderDetailsMongoEntity>, IOrderDetailsMongoRepository
    {
        public OrderDetailsMongoRepository(IMongoDatabase mongodb) : base(mongodb) { }

        public Task<List<OrderDetailsMongoEntity>> FindOrderDetailsByOrderId(int orderId)
        {
            return _collection.Find(x => x.OrderId == orderId).ToListAsync();
        }

        public Task<OrderDetailsMongoEntity> FindOrderDetails(int orderId, int productId)
        {
            return _collection.Find(x => x.OrderId == orderId && x.ProductId == productId).FirstOrDefaultAsync();
        }
    }
}
