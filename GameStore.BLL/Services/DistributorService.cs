using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStore.BLL.Interfaces;
using GameStore.DomainModels.Models;
using GameStore.DomainModels.Models.CreateModels;
using GameStore.DomainModels.Models.EditModels;
using GameStore.DomainModels.Exceptions;
using GameStore.DAL.Abstractions.Interfaces;

namespace GameStore.BLL.Services
{
    public class DistributorService : IDistributorService
    {
        private readonly IDistributorRepository _distributorRepository;
        private readonly IMapper _mapper;

        public DistributorService(
            IDistributorRepository publisherRepository, 
            IMapper mapper)
        {
            _distributorRepository = publisherRepository;
            _mapper = mapper;
        }

        public async Task CreateDistributorAsync(CreateDistributorRequest publisherToCreate)
        {
            Distributor createPublisher = _mapper.Map<Distributor>(publisherToCreate);

            bool isUnique = await _distributorRepository.IsDistributorUniqueAsync(createPublisher);
            if (!isUnique)
            {
                throw new NotUniqueException("Distributor with that company name is already exist");
            }

            await _distributorRepository.CreateAsync(createPublisher);
        }

        public async Task<Distributor> GetDistributorByIdAsync(Guid id)
        {
            Distributor foundedPublisher = await _distributorRepository.FindAsync(id.ToString());
            if (foundedPublisher is null)
            {
                throw new NotFoundException($"Publisher with that id wasn`t found: {id}");
            }

            return foundedPublisher;
        }

        public Task<List<Distributor>> GetAllDistributorsAsync()
        {
            return _distributorRepository.GetAllAsync();
        }

        public async Task<Distributor> GetDistributorByNameAsync(string name)
        {
            Distributor foundedPublisher = await _distributorRepository.FindByNameAsync(name);
            if (foundedPublisher is null)
            {
                throw new NotFoundException($"Publisher with that id wasn`t found: {name}");
            }

            return foundedPublisher;
        }

        public async Task EditDistributorAsync(EditDistributorRequest publisherToEdit)
        {
            Distributor editPublisher = _mapper.Map<Distributor>(publisherToEdit);

            bool isUnique = await _distributorRepository.IsDistributorUniqueAsync(editPublisher);
            if (!isUnique)
            {
                throw new NotUniqueException("Distributor with that company name is already exist");
            }

            await _distributorRepository.UpdateAsync(editPublisher);
        }

        public async Task HardDeleteDistributorAsync(Guid id)
        {
            try
            {
                await _distributorRepository.DeleteAsync(id);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException($"Publisher with id {id} wasn't found");
            }
        }

        public async Task SoftDeleteDistributorAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new InvalidOperationException("This operation can'y be applied to Northwind suppliers");
            }

            try
            {

                await _distributorRepository.SoftDeleteAsync(id);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException($"Publisher with id {id} wasn't found");
            }
        }
    }
}
