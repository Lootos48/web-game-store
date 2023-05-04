using GameStore.DomainModels.Models;
using GameStore.DomainModels.Models.CreateModels;
using GameStore.DomainModels.Models.EditModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.BLL.Interfaces
{
    public interface IPlatformTypeService
    {
        Task<List<PlatformType>> GetAllPlatformsAsync(string languageCode = null);

        Task<PlatformType> GetPlatformByIdAsync(Guid id);

        Task<PlatformType> GetPlatformByTypeAsync(string type, string localizationCultureCode = null);

        Task<List<Goods>> GetGamesByPlatformAsync(string platformType, string localizationCultureCode);

        Task CreatePlatformAsync(CreatePlatformTypeRequest platformToCreate);

        Task EditPlatformAsync(EditPlatformRequest platformTypeToEdit);

        Task SoftDeletePlatformAsync(Guid id);

        Task HardDeletePlatformAsync(Guid id);
        Task<PlatformType> GetPlatformByTypeIncludeAllLocalizationAsync(string type);
    }
}
