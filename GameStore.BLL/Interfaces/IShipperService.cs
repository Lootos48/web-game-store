using GameStore.DomainModels.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.BLL.Interfaces
{
    public interface IShipperService
    {
        public Task<List<Shipper>> GetAllShippersAsync();

        public Task<Shipper> GetShipperByIdAsync(int id);
    }
}
