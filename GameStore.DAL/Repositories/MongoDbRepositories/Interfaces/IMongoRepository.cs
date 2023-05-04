using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.DAL.Repositories.MongoDbRepositories.Interfaces
{
    public interface IMongoRepository<TEntity> where TEntity : class, new()
    {
        Task<List<TEntity>> GetAllAsync();
    }
}