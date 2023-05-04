using GameStore.DAL.Entities;
using System;
using System.Threading.Tasks;

namespace GameStore.DAL.Repositories.MsSqlDdRepositories.Interfaces
{
    public interface IPublisherRepository : ISqlRepository<PublisherEntity>
    {
        Task ConnectUserToPublisherAsync(Guid userId, Guid publisherId);
        Task DisconnectUserFromPublisherAsync(Guid userId);
        Task<PublisherEntity> FindByNameAsync(string companyName, bool isIncludeDeleted = false);
        Task<PublisherEntity> FindByUserIdAsync(Guid userId, bool isIncludeDeleted);
        Task<bool> IsPublisherUnique(PublisherEntity publisher);
    }
}