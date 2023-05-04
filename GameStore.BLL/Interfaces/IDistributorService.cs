using GameStore.DomainModels.Models;
using GameStore.DomainModels.Models.CreateModels;
using GameStore.DomainModels.Models.EditModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.BLL.Interfaces
{
    public interface IDistributorService
    {
        public Task<List<Distributor>> GetAllDistributorsAsync();
        Task<Distributor> GetDistributorByNameAsync(string name);
        Task<Distributor> GetDistributorByIdAsync(Guid id);
        public Task CreateDistributorAsync(CreateDistributorRequest publisherToCreate);
        public Task EditDistributorAsync(EditDistributorRequest publisherToEdit);
        Task SoftDeleteDistributorAsync(Guid id);
        Task HardDeleteDistributorAsync(Guid id);
    }
}
