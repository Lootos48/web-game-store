using GameStore.DAL.Entities;
using GameStore.DAL.Entities.Localizations;
using GameStore.DAL.Repositories.MsSqlDdRepositories.Interfaces;
using GameStore.DAL.SearchPipelines.GamesFilterPipeline.Interfaces;
using GameStore.DAL.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.DAL.Repositories.MsSqlDdRepositories
{
    public class GameRepository : SqlRepository<GameEntity>, IGameRepository
    {
        private readonly IGamesFiltersPipeline _filtersPipeline;
        private readonly MongoIntegrationSettings _mongoIntegrationSettings;
        private readonly DbSet<GameGenreEntity> _gameGenreDbSet;
        private readonly DbSet<GamePlatformTypeEntity> _gamePlatformTypeDbSet;

        public GameRepository(
            DbContext context,
            IGamesFiltersPipeline filtersPipeline,
            ILogger<GameRepository> logger,
            MongoIntegrationSettings defaultValuesSettings) : base(context, logger)
        {
            _filtersPipeline = filtersPipeline;
            _mongoIntegrationSettings = defaultValuesSettings;
            _gameGenreDbSet = context.Set<GameGenreEntity>();
            _gamePlatformTypeDbSet = context.Set<GamePlatformTypeEntity>();
        }

        public Task<bool> IsGameUnique(GameEntity game)
        {
            return _dbSet.AllAsync(g => g.Key != game.Key || (g.Id == game.Id && g.Key == game.Key));
        }

        public override Task<List<GameEntity>> GetAllEntitiesAsync(bool includesDeletedRows = false)
        {
            return _dbSet
                .Include(g => g.GameGenres)
                    .ThenInclude(g => g.Genre)
                .Include(g => g.GamesPlatformTypes)
                    .ThenInclude(g => g.PlatformType)
                .Include(g => g.Comments)
                .Include(g => g.Publisher)
                    .Where(g => g.Name != _mongoIntegrationSettings.TemporaryName)
                    .Where(g => !g.IsDeleted || includesDeletedRows)
                .ToListAsync();
        }

        public Task<List<GameEntity>> GetAllEntitiesIncludeLocalization(string cultureCode, bool includesDeletedRows = false)
        {
            return _dbSet
                .Include(g => g.GameGenres)
                    .ThenInclude(g => g.Genre)
                        .ThenInclude(g => g.Localizations.Where(l => l.Localization.CultureCode == cultureCode))
                .Include(g => g.GamesPlatformTypes)
                    .ThenInclude(g => g.PlatformType)
                        .ThenInclude(g => g.Localizations.Where(l => l.Localization.CultureCode == cultureCode))
                .Include(g => g.Comments)
                .Include(g => g.Publisher)
                    .Where(g => g.Name != _mongoIntegrationSettings.TemporaryName)
                    .Where(g => !g.IsDeleted || includesDeletedRows)
                .Include(g => g.Localizations.Where(l => l.Localization.CultureCode == cultureCode))
                .ToListAsync();
        }

        public Task<List<GameEntity>> GetWithFiltersAsync(GamesSearchRequest request, string cultureCode = null, bool isIncludeDeleted = false)
        {
            IQueryable<GameEntity> gamesQuery = _dbSet
                .Include(g => g.GameGenres)
                    .ThenInclude(g => g.Genre)
                        .ThenInclude(g => g.Localizations.Where(l => l.Localization.CultureCode == cultureCode))
                .Include(g => g.GamesPlatformTypes)
                    .ThenInclude(g => g.PlatformType)
                        .ThenInclude(g => g.Localizations.Where(l => l.Localization.CultureCode == cultureCode))
                .Include(g => g.Comments)
                .Include(g => g.Publisher)
                    .Where(g => g.Name != _mongoIntegrationSettings.TemporaryName)
                    .Where(g => !g.IsDeleted || isIncludeDeleted)
                .Include(g => g.Localizations.Where(l => l.Localization.CultureCode == cultureCode));

            gamesQuery = _filtersPipeline.Execute(gamesQuery, request);

            return gamesQuery.ToListAsync();
        }

        public override Task<GameEntity> FindEntityByIdAsync(Guid id, bool includesDeletedRows = false)
        {
            return _dbSet
                .Include(g => g.GamesPlatformTypes)
                    .ThenInclude(g => g.PlatformType)
                .Include(g => g.GameGenres)
                    .ThenInclude(g => g.Genre)
                .Include(g => g.Comments)
                .Include(g => g.Publisher)
                .FirstOrDefaultAsync(g => g.Id == id && (!g.IsDeleted || includesDeletedRows));
        }

        public async Task<GameEntity> FindEntityByIdAsNoTrackingAsync(Guid id, bool includesDeletedRows = false)
        {
            var entity = await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.Id == id && (!g.IsDeleted || includesDeletedRows));

            return entity;
        }

        public Task<GameEntity> FindByKeyAsync(string key, string localizationCultureCode = null, bool isIncludeDeleted = false)
        {
            return _dbSet
                .Include(g => g.GamesPlatformTypes)
                    .ThenInclude(g => g.PlatformType)
                        .ThenInclude(g => g.Localizations.Where(l => l.Localization.CultureCode == localizationCultureCode))
                .Include(g => g.GameGenres)
                    .ThenInclude(g => g.Genre)
                        .ThenInclude(g => g.Localizations.Where(l => l.Localization.CultureCode == localizationCultureCode))
                .Include(g => g.Comments)
                .Include(g => g.Publisher)
                .Include(g => g.Localizations.Where(l => l.Localization.CultureCode == localizationCultureCode))
                    .ThenInclude(l => l.Localization)
                .FirstOrDefaultAsync(g => g.Key == key && (!g.IsDeleted || isIncludeDeleted));
        }

        public Task<GameEntity> FindByKeyIncludeAllLocalizationsAsync(string key, bool isIncludeDeleted = false)
        {
            return _dbSet
                .Include(g => g.GamesPlatformTypes)
                    .ThenInclude(g => g.PlatformType)
                .Include(g => g.GameGenres)
                    .ThenInclude(g => g.Genre)
                .Include(g => g.Comments)
                .Include(g => g.Publisher)
                .Include(g => g.Localizations)
                    .ThenInclude(l => l.Localization)
                .FirstOrDefaultAsync(g => g.Key == key && (!g.IsDeleted || isIncludeDeleted));
        }

        public Task<GameEntity> FindByIdIncludeAllLocalizationsAsync(Guid id, bool isIncludeDeleted = false)
        {
            return _dbSet
                .Include(g => g.GamesPlatformTypes)
                    .ThenInclude(g => g.PlatformType)
                .Include(g => g.GameGenres)
                    .ThenInclude(g => g.Genre)
                .Include(g => g.Comments)
                .Include(g => g.Publisher)
                .Include(g => g.Localizations)
                    .ThenInclude(l => l.Localization)
                .FirstOrDefaultAsync(g => g.Id == id && (!g.IsDeleted || isIncludeDeleted));
        }

        public async Task<Guid> FindGameIdByKeyAsync(string key, bool isIncludeDeleted = false)
        {
            GameEntity gameEntity = await _dbSet.AsNoTracking()
                .FirstAsync(g => g.Key == key && (!g.IsDeleted || isIncludeDeleted));

            return gameEntity.Id;
        }

        public async Task<string> FindKeyByGameIdAsync(Guid gameId, bool isIncludeDeleted = false)
        {
            GameEntity gameEntity = await _dbSet.AsNoTracking()
                .FirstAsync(g => g.Id == gameId && (!g.IsDeleted || isIncludeDeleted));

            return gameEntity.Key;
        }

        public Task<List<GameEntity>> FindByGenreAsync(Guid genreId, string localizationCultureCode = null, bool isIncludeDeleted = false)
        {
            return _dbSet
                 .Include(g => g.GameGenres)
                    .ThenInclude(g => g.Genre)
                 .Include(g => g.Publisher)
                .Include(g => g.Localizations.Where(x => x.Localization.CultureCode == localizationCultureCode))
                .Where(g => !g.IsDeleted || isIncludeDeleted)
                .Where(g => g.GameGenres.Any(gg => gg.GenreId == genreId))
                .ToListAsync();
        }

        public Task<List<GameEntity>> FindByPlatformTypeAsync(Guid platformId, string localizationCultureCode = null, bool isIncludeDeleted = false)
        {
            return _dbSet
                 .Include(g => g.GamesPlatformTypes)
                     .ThenInclude(g => g.PlatformType)
                 .Include(g => g.Publisher)
                 .Include(p => p.Localizations.Where(l => l.Localization.CultureCode == localizationCultureCode))
                 .Where(g => !g.IsDeleted || isIncludeDeleted)
                 .Where(g => g.GamesPlatformTypes.Any(gg => gg.PlatformId == platformId))
                 .ToListAsync();
        }

        public async Task UpdateLocalizationAsync(GameLocalizationEntity editedLocalization)
        {
            try
            {
                _context.Entry(editedLocalization).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                _context.Entry(editedLocalization).State = EntityState.Added;
                await _context.SaveChangesAsync();
            }
        }

        public Task BatchUpdateUnitsInStockAsync(List<GameEntity> games)
        {
            if (games.Count == 0)
            {
                return Task.CompletedTask;
            }

            FormattableString query = BuildBatchUpdateQuery(games);
            return _context.Database.ExecuteSqlInterpolatedAsync(query);
        }

        public Task IncreaseViewCountAsync(Guid gameId)
        {
            FormattableString query1 = $@"UPDATE [Games] SET [ViewCount] = [ViewCount]+1 WHERE [Id] = {gameId};";
            return _context.Database.ExecuteSqlInterpolatedAsync(query1);
        }

        public Task<int> GetCountOfGamesAsync(bool isIncludeDeleted = false)
        {
            return _dbSet.CountAsync(g => !g.IsDeleted || isIncludeDeleted);
        }

        public Task CreateRangeGamesGenresAsync(List<GameGenreEntity> gameGenreEntities)
        {
            _gameGenreDbSet.AddRange(gameGenreEntities);

            return _context.SaveChangesAsync();
        }

        public Task CreateRangeGamesPlatformTypesAsync(List<GamePlatformTypeEntity> gamesPlatformTypesEntities)
        {
            _gamePlatformTypeDbSet.AddRange(gamesPlatformTypesEntities);

            return _context.SaveChangesAsync();
        }

        public Task<List<GameGenreEntity>> GetGameGenresByGameIdAsync(Guid gameId)
        {
            return _gameGenreDbSet
                .Where(gg => gg.GameId == gameId)
                .ToListAsync();
        }

        public Task<List<GameGenreEntity>> GetGameGenresByGenreIdAsync(Guid genreId)
        {
            return _gameGenreDbSet
               .Where(gg => gg.GenreId == genreId)
               .ToListAsync();
        }

        public Task DeleteRangeGamesGenresAsync(List<GameGenreEntity> gameGenresToDelete)
        {
            FormattableString query = BuildRangeDeletingQuery(gameGenresToDelete);

            return _context.Database.ExecuteSqlInterpolatedAsync(query);
        }

        public Task<List<GamePlatformTypeEntity>> GetGamesPlatformsGameIdAsync(Guid gameId)
        {
            return _gamePlatformTypeDbSet.Where(gp => gp.GameId == gameId).ToListAsync();
        }

        public Task<List<GamePlatformTypeEntity>> GetGamesPlatformsByPlatformTypeIdAsync(Guid platformTypeId)
        {
            return _gamePlatformTypeDbSet.Where(gp => gp.PlatformId == platformTypeId).ToListAsync();
        }

        public Task DeleteRangeGamesPlatformsAsync(List<GamePlatformTypeEntity> gamePlatformTypes)
        {
            var query = BuildRangeDeletingQuery(gamePlatformTypes);

            return _context.Database.ExecuteSqlInterpolatedAsync(query);
        }

        private static FormattableString BuildRangeDeletingQuery(List<GamePlatformTypeEntity> gamePlatformTypesToDelete)
        {
            StringBuilder query = new StringBuilder("DELETE FROM [GamesPlatformTypes] WHERE ");

            object[] parameters = new object[gamePlatformTypesToDelete.Count * 2];
            for (int i = 0, argumentCounter = 0; i < gamePlatformTypesToDelete.Count; i++, argumentCounter += 2)
            {
                query.Append("[GameId] = {" + argumentCounter + "} AND [PlatformId] = {" + (argumentCounter + 1) + "} OR");

                parameters[argumentCounter] = gamePlatformTypesToDelete[i].GameId;
                parameters[argumentCounter + 1] = gamePlatformTypesToDelete[i].PlatformId;
            }

            query.Remove(query.Length - 2, 2);
            query.Append(";");

            return FormattableStringFactory.Create(query.ToString(), parameters);
        }

        private static FormattableString BuildRangeDeletingQuery(List<GameGenreEntity> gameGenresToDelete)
        {
            StringBuilder query = new StringBuilder("DELETE FROM [GamesGenres] WHERE ");

            object[] parameters = new object[gameGenresToDelete.Count * 2];
            for (int i = 0, argumentCounter = 0; i < gameGenresToDelete.Count; i++, argumentCounter += 2)
            {
                query.Append("[GameId] = {" + argumentCounter + "} AND [GenreId] = {" + (argumentCounter + 1) + "} OR");

                parameters[argumentCounter] = gameGenresToDelete[i].GameId;
                parameters[argumentCounter + 1] = gameGenresToDelete[i].GenreId;
            }

            query.Remove(query.Length - 2, 2);
            query.Append(";");

            return FormattableStringFactory.Create(query.ToString(), parameters);
        }

        private static FormattableString BuildBatchUpdateQuery(List<GameEntity> games)
        {
            StringBuilder query = new StringBuilder("UPDATE [Games] SET[UnitsInStock] = CASE[Id]");
            StringBuilder queryPredicate = new StringBuilder("WHERE [Id] IN(");

            object[] parameters = new object[games.Count * 2];
            for (int i = 0, argumentCounter = 0;  i < games.Count; i++, argumentCounter += 2)
            {
                query.Append(" WHEN {" + argumentCounter + "} THEN {" + (argumentCounter + 1) + "}");
                queryPredicate.Append("{" + argumentCounter + "}, ");

                parameters[argumentCounter] = games[i].Id;
                parameters[argumentCounter + 1] = games[i].UnitsInStock;
            }

            query.Append(" ELSE [UnitsInStock] END ");

            queryPredicate.Remove(queryPredicate.Length - 2, 2);
            queryPredicate.Append(");");

            query.Append(queryPredicate);

            return FormattableStringFactory.Create(query.ToString(), parameters);
        }

        public Task<List<GameEntity>> FindByIdsAsNoTrackingAsync(List<Guid> gamesIds, bool isIncludeDeleted = false)
        {
            return _dbSet.AsNoTracking()
                .Where(e => gamesIds.Contains(e.Id) && (!e.IsDeleted || isIncludeDeleted))
                .ToListAsync();
        }
    }
}
