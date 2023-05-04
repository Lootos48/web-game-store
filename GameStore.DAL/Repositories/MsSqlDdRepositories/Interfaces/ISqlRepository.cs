using GameStore.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.DAL.Repositories.MsSqlDdRepositories.Interfaces
{
    public interface ISqlRepository<TEntity> where TEntity : Entity
    {
        Task CreateEntityAsync(TEntity entity);

        Task DeleteEntityAsync(TEntity entity);

        Task<List<TEntity>> GetAllEntitiesAsync(bool includesDeletedRows = false);

        Task<TEntity> FindEntityByIdAsync(Guid id, bool includesDeletedRows = false);

        Task<List<TEntity>> FindEntitiesByIdsAsync(List<Guid> ids, bool isIncludeDeleted = false);

        Task UpdateEntityAsync(TEntity entity);

        Task RecoverAsync(Guid id);

        Task SoftDeleteAsync(Guid id);

        Task DeleteByIdAsync(Guid id);
    }
}
