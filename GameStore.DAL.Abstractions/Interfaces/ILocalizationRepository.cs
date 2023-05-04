using GameStore.DomainModels.Models.Localizations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.DAL.Abstractions.Interfaces
{
    public interface ILocalizationRepository
    {
        Task<List<Localization>> GetAllLocalizationsAsync();

        Task<Localization> GetLocalizationByCultureCodeAsync(string cultureCode);

        Task<Localization> GetLocalizationByIdAsync(Guid id);
    }
}
