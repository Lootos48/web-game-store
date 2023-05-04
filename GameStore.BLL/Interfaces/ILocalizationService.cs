using GameStore.DomainModels.Models.Localizations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.BLL.Interfaces
{
    public interface ILocalizationService
    {
        Task<List<Localization>> GetAllLocalizationsAsync();

        Task<Localization> GetLocalizationByCultureCodeAsync(string cultureCode);

        Task<Localization> GetLocalizationByIdAsync(Guid id);
    }
}
