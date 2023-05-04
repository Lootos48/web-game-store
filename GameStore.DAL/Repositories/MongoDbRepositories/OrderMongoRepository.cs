using GameStore.DAL.Entities.MongoEntities;
using GameStore.DAL.Repositories.MongoDbRepositories.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.DAL.Repositories.MongoDbRepositories
{
    public class OrderMongoRepository : MongoRepository<OrderMongoEntity>, IOrderMongoRepository
    {
        public OrderMongoRepository(IMongoDatabase mongodb) : base(mongodb) { }

        public Task<OrderMongoEntity> FindById(int orderId)
        {
            return _collection.Find(x => x.OrderId == orderId).FirstOrDefaultAsync();
        }

        public Task<List<OrderMongoEntity>> FindByIdsAsync(List<int?> mongoIds)
        {
            var filter = Builders<OrderMongoEntity>.Filter.In(x => x.OrderId, mongoIds);

            return _collection.Find(filter).ToListAsync();
        }

        public Task<List<OrderMongoEntity>> GetBetweenDatesAsync(DateTime minDate, DateTime maxDate)
        {
            var greaterFilter = new BsonDocument("$expr",
                    new BsonDocument("$gte",
                        new BsonArray
                        {
                            new BsonDocument("$toDate", "$OrderDate"),
                            minDate
                        }
                    )
                );

            var lowerFilter = new BsonDocument("$expr",
                    new BsonDocument("$lte",
                        new BsonArray
                        {
                            new BsonDocument("$toDate", "$OrderDate"),
                            maxDate
                        }
                    )
                );

            var filter = new BsonDocument("$and",
                    new BsonArray
                    {
                        greaterFilter,
                        lowerFilter
                    });

            return _collection.Find(filter).ToListAsync();
        }

        public Task<List<OrderMongoEntity>> GetOrdersAfterAsync(DateTime minDate)
        {
            var greaterFilter = new BsonDocument("$expr",
                    new BsonDocument("$gte",
                        new BsonArray
                        {
                            new BsonDocument("$toDate", "$OrderDate"),
                            minDate
                        }
                    )
                );

            return _collection.Find(greaterFilter).ToListAsync();
        }

        public Task<List<OrderMongoEntity>> GetOrdersBeforeAsync(DateTime maxDate)
        {
            var lowerFilter = new BsonDocument("$expr",
                    new BsonDocument("$lte",
                        new BsonArray
                        {
                            new BsonDocument("$toDate", "$OrderDate"),
                            maxDate
                        }
                    )
                );

            return _collection.Find(lowerFilter).ToListAsync();
        }
    }
}
