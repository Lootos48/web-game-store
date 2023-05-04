using GameStore.DAL.Abstractions.Interfaces;
using GameStore.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace GameStore.DAL.Repositories
{
    public class LegacyKeyRepository : ILegacyKeyRepository
    {
        private readonly DbContext _context;
        private readonly DbSet<LegacyKeyEntity> _dbSet;

        public LegacyKeyRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<LegacyKeyEntity>();
        }

        public async Task<bool> IsKeyUnique(string key)
        {
            DbSet<GameEntity> gamesDbSet = _context.Set<GameEntity>();

            bool isNewKeyUniqueInGames = await gamesDbSet.AllAsync(g => g.Key != key);
            bool isNewKeyUniqueInOldKeys = await _dbSet.AllAsync(o => o.LegacyKey != key);

            return isNewKeyUniqueInGames && isNewKeyUniqueInOldKeys;
        }

        public Task CreateAsync(string legacyKey, Guid gameId)
        {
            LegacyKeyEntity entity = new LegacyKeyEntity()
            {
                GameId = gameId,
                LegacyKey = legacyKey
            };

            _dbSet.Add(entity);
            return _context.SaveChangesAsync();
        }

        public async Task<Guid> FindGameIdAsync(string legacyKey)
        {
            LegacyKeyEntity legacyKeyEntity = await _dbSet.FirstOrDefaultAsync(ok => ok.LegacyKey == legacyKey);

            if (legacyKeyEntity is null)
            {
                return Guid.Empty;
            }

            return legacyKeyEntity.GameId;
        }
    }
}
