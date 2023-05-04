using GameStore.DAL.Entities.MongoEntities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.DAL.Repositories.MongoDbRepositories.Interfaces
{
    public interface IOrderMongoRepository : IMongoRepository<OrderMongoEntity>
    {
        Task<OrderMongoEntity> FindById(int orderId);

        Task<List<OrderMongoEntity>> FindByIdsAsync(List<int?> mongoIds);

        Task<List<OrderMongoEntity>> GetBetweenDatesAsync(DateTime minDate, DateTime maxDate);

        Task<List<OrderMongoEntity>> GetOrdersBeforeAsync(DateTime maxDate);

        Task<List<OrderMongoEntity>> GetOrdersAfterAsync(DateTime minDate);
    }
}