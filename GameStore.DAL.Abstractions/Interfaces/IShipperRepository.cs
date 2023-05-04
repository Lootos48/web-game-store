using GameStore.DomainModels.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.DAL.Abstractions.Interfaces
{
    public interface IShipperRepository
    {
        Task<List<Shipper>> GetAllAsync();

        Task<Shipper> GetByIdAsync(int id);
    }
}
