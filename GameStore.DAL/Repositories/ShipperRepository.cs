using AutoMapper;
using GameStore.DAL.Abstractions.Interfaces;
using GameStore.DAL.Repositories.MongoDbRepositories.Interfaces;
using GameStore.DomainModels.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.DAL.Repositories
{
    public class ShipperRepository : IShipperRepository
    {
        private readonly IShipperMongoRepository _shipperMongoRepository;

        private readonly IMapper _mapper;

        public ShipperRepository(
            IShipperMongoRepository shipperMongoRepository,
            IMapper mapper)
        {
            _shipperMongoRepository=shipperMongoRepository;
            _mapper=mapper;
        }

        public async Task<List<Shipper>> GetAllAsync()
        {
            var entities = await _shipperMongoRepository.GetAllAsync();
            return _mapper.Map<List<Shipper>>(entities);
        }

        public async Task<Shipper> GetByIdAsync(int id)
        {
            var entity = await _shipperMongoRepository.FindById(id);
            return _mapper.Map<Shipper>(entity);
        }
    }
}
