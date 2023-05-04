using GameStore.DomainModels.Models;
using GameStore.DomainModels.Models.CreateModels;
using GameStore.DomainModels.Models.EditModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.BLL.Interfaces
{
    public interface IGoodsService
    {
        Task<Goods> GetGoodsByKeyAsync(string key, string localizationCultureCode = null, bool includeDeletedRows = false);

        Task<Goods> GetGoodsByIdAsync(Guid id);

        Task<int> GetCountOfGoodsAsync();

        Task<List<Goods>> GetGoodsAsync(string localizationCultureCode = null);

        Task<string> CreateGoodsAsync(CreateGoodsRequest gameToCreate);

        Task AddGenreToGoodsAsync(Guid gameId, List<Genre> genres);

        Task AddPlatformTypeToGoodsAsync(Guid gameId, List<PlatformType> platformTypesToAdd);

        Task EditGoodsAsync(EditGoodsRequest gameToEdit);

        Task RemoveGenreFromGoodsAsync(Guid gameId, List<Genre> genresToRemove);

        Task RemovePlatformTypeFromGoodsAsync(Guid gameId, List<PlatformType> platformTypesToRemove);

        Task SoftDeleteGoodsAsync(string id);

        Task HardDeleteGoodsAsync(Guid id);

        Task RecoverGoodsAsync(Guid id);

        Task<byte[]> DownloadAsync(string key);

        Task<List<Goods>> GetFilteredGoodsAsync(GoodsSearchRequest filters, string localizationCultureCode = null);

        Task IncreaseViewCountAsync(string gameId);

        Task<Goods> GetGoodsByKeyIncludeAllLocalizationsAsync(string key, bool includeDeletedRows = false);
    }
}
