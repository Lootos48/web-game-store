using GameStore.DomainModels.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.DAL.Abstractions.Interfaces
{
    public interface IPublisherSupplierRepository
    {
        Task<List<PublisherSupplierMapping>> GetAllAsync();

        Task<PublisherSupplierMapping> FindAsync(Guid publisherId, int supplierId);

        Task<PublisherSupplierMapping> GetByPublisherAsync(Guid publisherId);

        Task<PublisherSupplierMapping> GetBySupplierIdAsync(int supplierId);

        Task CreateAsync(PublisherSupplierMapping newMapping);
    }
}
