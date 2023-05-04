using GameStore.BLL.Interfaces;
using GameStore.DAL.Abstractions.Interfaces;
using GameStore.DomainModels.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.BLL.Services
{
    public class ShipperService : IShipperService
    {
        private readonly IShipperRepository _shipperRepository;

        public ShipperService(IShipperRepository shipperRepository)
        {
            _shipperRepository = shipperRepository;
        }

        public Task<List<Shipper>> GetAllShippersAsync()
        {
            return _shipperRepository.GetAllAsync();
        }

        public Task<Shipper> GetShipperByIdAsync(int id)
        {
            return _shipperRepository.GetByIdAsync(id);
        }
    }
}
