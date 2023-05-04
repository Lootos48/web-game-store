using GameStore.DomainModels.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.DAL.Abstractions.Interfaces
{
    public interface IPlatformTypeRepository
    {
        Task<PlatformType> FindByTypeAsync(string type, string localizationCultureCode = null, bool includeDeleted = false);

        Task CreateAsync(PlatformType item);

        Task DeleteAsync(PlatformType item);

        Task<List<PlatformType>> GetAllAsync(bool isIncludeDeleted = false);

        Task<List<PlatformType>> GetAllAsync(string localizationCode, bool includeDeleted = false);

        Task<PlatformType> FindByIdAsync(Guid id, bool isIncludeDeleted = false);

        Task UpdateAsync(PlatformType item);

        Task RecoverAsync(Guid id);

        Task SoftDeleteAsync(Guid id);

        Task DeleteByIdAsync(Guid id);

        Task<bool> IsPlatformTypeUnique(PlatformType platformType);

        Task UpdateLocalizationAsync(PlatformType item, Guid chosenLocalization);

        Task<PlatformType> FindByTypeIncludeAllLocalizationsAsync(string type, bool includeDeleted = false);
    }
}
