using GameStore.DomainModels.Models;
using System.Threading.Tasks;

namespace GameStore.DAL.Workflows.Interfaces
{
    public interface IPublisherSupplierMappingWorkflow
    {
        Task<PublisherSupplierMapping> CreateMappingAsync(int supplierId);

        Task<PublisherSupplierMapping> GetMappingAsync(int supplierId);
    }
}