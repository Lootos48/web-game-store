using GameStore.DAL.Entities;
using GameStore.DAL.Entities.Localizations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.DAL.Repositories.MsSqlDdRepositories.Interfaces
{
    public interface IGameRepository : ISqlRepository<GameEntity>
    {
        Task BatchUpdateUnitsInStockAsync(List<GameEntity> games);

        Task DeleteRangeGamesGenresAsync(List<GameGenreEntity> gameGenresToDelete);

        Task<List<GameEntity>> FindByGenreAsync(Guid genreId, string localizationCultureCode = null, bool isIncludeDeleted = false);

        Task<GameEntity> FindByKeyAsync(string key, string localizationCultureCode = null, bool isIncludeDeleted = false);

        Task<List<GameEntity>> FindByPlatformTypeAsync(Guid platformId, string localizationCultureCode = null, bool isIncludeDeleted = false);

        Task<Guid> FindGameIdByKeyAsync(string key, bool isIncludeDeleted = false);

        Task<string> FindKeyByGameIdAsync(Guid gameId, bool isIncludeDeleted = false);

        Task<List<GamePlatformTypeEntity>> GetGamesPlatformsByPlatformTypeIdAsync(Guid platformTypeId);

        Task<int> GetCountOfGamesAsync(bool isIncludeDeleted = false);

        Task<List<GameGenreEntity>> GetGameGenresByGameIdAsync(Guid gameId);

        Task<List<GameGenreEntity>> GetGameGenresByGenreIdAsync(Guid genreId);

        Task<List<GamePlatformTypeEntity>> GetGamesPlatformsGameIdAsync(Guid gameId);

        Task<List<GameEntity>> GetWithFiltersAsync(GamesSearchRequest request, string cultureCode = null, bool isIncludeDeleted = false);

        Task IncreaseViewCountAsync(Guid gameId);

        Task DeleteRangeGamesPlatformsAsync(List<GamePlatformTypeEntity> gamePlatformTypes);

        Task CreateRangeGamesGenresAsync(List<GameGenreEntity> gameGenreEntities);

        Task CreateRangeGamesPlatformTypesAsync(List<GamePlatformTypeEntity> gamesPlatformTypesEntities);

        Task<GameEntity> FindEntityByIdAsNoTrackingAsync(Guid id, bool includesDeletedRows = false);

        Task<List<GameEntity>> FindByIdsAsNoTrackingAsync(List<Guid> gamesIds, bool isIncludeDeleted = false);

        Task<bool> IsGameUnique(GameEntity game);

        Task<List<GameEntity>> GetAllEntitiesIncludeLocalization(string cultureCode, bool includesDeletedRows = false);

        Task<GameEntity> FindByKeyIncludeAllLocalizationsAsync(string key, bool isIncludeDeleted = false);

        Task<GameEntity> FindByIdIncludeAllLocalizationsAsync(Guid id, bool isIncludeDeleted = false);

        Task UpdateLocalizationAsync(GameLocalizationEntity editedLocalization);
    }
}