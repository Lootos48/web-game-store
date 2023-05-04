using AutoMapper;
using GameStore.BLL.Interfaces;
using GameStore.DomainModels.Models;
using GameStore.DomainModels.Models.CreateModels;
using GameStore.DomainModels.Models.EditModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStore.DomainModels.Exceptions;
using GameStore.DAL.Abstractions.Interfaces;

namespace GameStore.BLL.Services
{
    public class PlatformTypeService : IPlatformTypeService
    {
        private readonly IPlatformTypeRepository _platformTypeRepository;
        private readonly IGoodsRepository _gameRepository;
        private readonly IMapper _mapper;

        public PlatformTypeService(
            IPlatformTypeRepository platformTypeRepository,
            IGoodsRepository gameRepository, 
            IMapper mapper)
        {
            _platformTypeRepository = platformTypeRepository;
            _gameRepository = gameRepository;
            _mapper = mapper;
        }

        public async Task CreatePlatformAsync(CreatePlatformTypeRequest platformToCreate)
        {
            PlatformType createPlatformType = _mapper.Map<PlatformType>(platformToCreate);

            bool isUnique = await _platformTypeRepository.IsPlatformTypeUnique(createPlatformType);
            if (!isUnique)
            {
                throw new NotUniqueException("Platform with that type is already exist");
            }

            await _platformTypeRepository.CreateAsync(createPlatformType);
        }

        public async Task<PlatformType> GetPlatformByIdAsync(Guid id)
        {
            PlatformType foundedPlatformType = await _platformTypeRepository.FindByIdAsync(id);
            if (foundedPlatformType is null)
            {
                throw new NotFoundException($"PlatformType with that id wasn`t found: {id}");
            }

            return foundedPlatformType;
        }

        public async Task<PlatformType> GetPlatformByTypeAsync(string type, string localizationCultureCode = null)
        {
            PlatformType foundedPlatformType = await _platformTypeRepository.FindByTypeAsync(type, localizationCultureCode);

            if (foundedPlatformType is null)
            {
                throw new NotFoundException($"Platform with type {type} wasn't found");
            }

            return foundedPlatformType;
        }

        public async Task<PlatformType> GetPlatformByTypeIncludeAllLocalizationAsync(string type)
        {
            PlatformType foundedPlatformType = await _platformTypeRepository.FindByTypeIncludeAllLocalizationsAsync(type);

            if (foundedPlatformType is null)
            {
                throw new NotFoundException($"Platform with type {type} wasn't found");
            }

            return foundedPlatformType;
        }

        public Task<List<PlatformType>> GetAllPlatformsAsync(string languageCode = null)
        {
            return _platformTypeRepository.GetAllAsync(languageCode);
        }

        public async Task<List<Goods>> GetGamesByPlatformAsync(string platformType, string localizationCultureCode)
        {
            PlatformType foundedPlatformType = await _platformTypeRepository.FindByTypeAsync(platformType);
            if (foundedPlatformType is null)
            {
                throw new NotFoundException($"PlatformType with that Type wasn`t found: {foundedPlatformType}");
            }

            var gamesSequence = await _gameRepository.FindByPlatformTypeAsync(foundedPlatformType.Id, localizationCultureCode);

            return gamesSequence;
        }

        public async Task EditPlatformAsync(EditPlatformRequest platformTypeToEdit)
        {
            PlatformType chengedPlatformType = _mapper.Map<PlatformType>(platformTypeToEdit);

            bool isUnique = await _platformTypeRepository.IsPlatformTypeUnique(chengedPlatformType);
            if (!isUnique)
            {
                throw new NotUniqueException("Platform with that type is already exist");
            }

            if (platformTypeToEdit.ChosedLocalization.HasValue)
            {
                await _platformTypeRepository.UpdateLocalizationAsync(chengedPlatformType, platformTypeToEdit.ChosedLocalization.Value);
            }
            else
            {
                await _platformTypeRepository.UpdateAsync(chengedPlatformType);
            }
        }

        public async Task HardDeletePlatformAsync(Guid id)
        {
            try
            {
                await _platformTypeRepository.DeleteByIdAsync(id);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException($"Platform type with id {id} wasn't found");
            }
        }

        public async Task SoftDeletePlatformAsync(Guid id)
        {
            try
            {
                await _platformTypeRepository.SoftDeleteAsync(id);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException($"Platform type with id {id} wasn't found");
            }
        }
    }
}
