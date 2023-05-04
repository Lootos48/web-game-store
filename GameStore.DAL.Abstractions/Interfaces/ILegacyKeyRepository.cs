using System;
using System.Threading.Tasks;

namespace GameStore.DAL.Abstractions.Interfaces
{
    public interface ILegacyKeyRepository
    {
        public Task CreateAsync(string legacyKey, Guid gameId);

        public Task<Guid> FindGameIdAsync(string legacyKey);
        Task<bool> IsKeyUnique(string key);
    }
}
