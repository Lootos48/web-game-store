using GameStore.DomainModels.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.DAL.Abstractions.Interfaces
{
    public interface IDistributorRepository
    {
        Task ConnectUserToDistributorAsync(Guid userId, string distributorId);
        Task CreateAsync(Distributor item);

        Task DeleteAsync(Distributor distributor);

        Task DeleteAsync(Guid id);
        Task DisconnectUserFromDistributorAsync(Guid userId);
        Task<Distributor> FindAsync(string id, bool isIncludeDeleted = false);

        Task<List<Distributor>> FindByIdsAsync(List<string> ids, bool isIncludeDeleted = false);

        Task<Distributor> FindByNameAsync(string companyName, bool isIncludeDeleted = false);
        Task<Distributor> FindByUserIdAsync(Guid UserId, bool isIncludeDeleted = false);
        Task<List<Distributor>> GetAllAsync(bool isIncludeDeleted = false);
        Task<bool> IsDistributorUniqueAsync(Distributor distributor);
        Task RecoverAsync(Guid id);

        Task SoftDeleteAsync(Guid id);

        Task UpdateAsync(Distributor item);
    }
}
