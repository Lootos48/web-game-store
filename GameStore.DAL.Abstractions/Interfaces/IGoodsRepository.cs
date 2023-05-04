using GameStore.DomainModels.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.DAL.Abstractions.Interfaces
{
    public interface IGoodsRepository
    {
        Task BatchUpdateUnitsInStockAsync(List<Goods> games);

        Task CreateAsync(Goods item);

        Task DeleteAsync(Goods good);

        Task DeleteAsync(Guid id);

        Task DeleteRangeGamesGenresAsync(List<GameGenre> gameGenresToDelete);

        Task DeleteRangeGamesPlatformTypesAsync(List<GamePlatformType> gameGenresToDelete);

        Task<Goods> FindAsync(string id, bool isIncludeDeleted = false, bool asNoTracking = false);

        Task<List<Goods>> FindByGenreAsync(Guid genreId, string localizationCultureCode, bool isIncludeDeleted = false);

        Task<List<Goods>> FindByIdsAsync(List<string> ids, bool isIncludeDeleted = false, bool asNoTracking = false);

        Task<Goods> FindByKeyAsync(string key, string localizationCultureCode = null, bool isIncludeDeleted = false);

        Task<List<Goods>> FindByPlatformTypeAsync(Guid platformId, string localizationCultureCode = null, bool isIncludeDeleted = false);

        Task<Guid> FindGameIdByKeyAsync(string key, bool isIncludeDeleted = false);

        Task<string> FindKeyByGoodsIdAsync(string id, bool isIncludeDeleted = false);

        Task<int> GetCountOfGoods(bool isIncludeDeleted = false);

        Task<List<GamePlatformType>> GetGamePlatformByGameIdAsync(Guid gameId);

        Task<List<GamePlatformType>> GetGamePlatformByPlatformTypeIdAsync(Guid platformTypeId);

        Task<List<GameGenre>> GetGamesGenresByGameIdAsync(Guid gameId);

        Task<List<GameGenre>> GetGamesGenresByGenreIdAsync(Guid genreId);

        Task<List<Goods>> GetWithFiltersAsync(GoodsSearchRequest request, string localizationCulture = null, bool isIncludeDeleted = false);

        Task IncreaseViewCountAsync(string id);

        Task<List<Goods>> GetAllAsync(string localizationCulture = null, bool isIncludeDeleted = false);

        Task RecoverAsync(Guid id);

        Task SoftDeleteAsync(Guid id);

        Task SoftDeleteAsync(string id);

        Task UpdateAsync(Goods item);

        Task CreateRangeGamesGenresAsync(List<GameGenre> newGameGanreList);

        Task CreateRangeGamesPlatformTypesAsync(List<GamePlatformType> newGamePlatformTypeList);

        Task<bool> IsGoodsUnique(Goods goods);

        Task<Goods> FindByKeyIncludeAllLocalizationsAsync(string key, bool includeDeletedRows = false);

        Task UpdateLocalizationAsync(Goods item, Guid chosenLocalization);
    }
}
