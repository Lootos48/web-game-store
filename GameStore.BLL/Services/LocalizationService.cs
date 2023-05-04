using GameStore.BLL.Interfaces;
using GameStore.DAL.Abstractions.Interfaces;
using GameStore.DomainModels.Exceptions;
using GameStore.DomainModels.Models.Localizations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.BLL.Services
{
    public class LocalizationService : ILocalizationService
    {
        private readonly ILocalizationRepository _localizationRepository;

        public LocalizationService(ILocalizationRepository localizationRepository)
        {
            _localizationRepository = localizationRepository;
        }

        public Task<List<Localization>> GetAllLocalizationsAsync()
        {
            return _localizationRepository.GetAllLocalizationsAsync();
        }

        public async Task<Localization> GetLocalizationByIdAsync(Guid id)
        {
            Localization foundLocalization = await _localizationRepository.GetLocalizationByIdAsync(id);

            if (foundLocalization is null)
            {
                throw new NotFoundException($"OrderDetails with id {id} wasn't found");
            }

            return foundLocalization;
        }

        public async Task<Localization> GetLocalizationByCultureCodeAsync(string cultureCode)
        {
            Localization foundLocalization = await _localizationRepository.GetLocalizationByCultureCodeAsync(cultureCode);

            if (foundLocalization is null)
            {
                throw new NotFoundException($"OrderDetails with culture code {cultureCode} wasn't found");
            }

            return foundLocalization;
        }
    }
}
