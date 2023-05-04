using GameStore.DAL.Attributes;
using GameStore.DAL.Repositories.MongoDbRepositories.Interfaces;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore.DAL.Repositories.MongoDbRepositories
{
    public abstract class MongoRepository<TEntity> : IMongoRepository<TEntity> where TEntity : class, new()
    {
        protected readonly IMongoCollection<TEntity> _collection;
        protected readonly IMongoQueryable<TEntity> _queryable;

        protected MongoRepository(IMongoDatabase mongodb)
        {
            CollectionNameAttribute attr = (CollectionNameAttribute)typeof(TEntity).GetCustomAttributes(typeof(CollectionNameAttribute), false).First();

            _collection = mongodb.GetCollection<TEntity>(attr.Name);
            _queryable = _collection.AsQueryable();
        }

        public virtual Task<List<TEntity>> GetAllAsync()
        {
            return _collection.Find(x => true).ToListAsync();
        }
    }
}
