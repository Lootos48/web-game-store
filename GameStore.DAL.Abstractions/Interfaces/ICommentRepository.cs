using GameStore.DomainModels.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.DAL.Abstractions.Interfaces
{
    public interface ICommentRepository
    {
        public Task<List<Comment>> GetByGameIdAsync(Guid gameId, bool isIncludeDeleted = false);

        Task CreateAsync(Comment item);

        Task DeleteAsync(Comment item);

        Task<List<Comment>> GetAllAsync(bool isIncludeDeleted = false);

        Task<Comment> FindByIdAsync(Guid id, bool isIncludeDeleted = false);

        Task UpdateAsync(Comment item);

        Task RecoverAsync(Guid id);

        Task SoftDeleteAsync(Guid id);

        Task DeleteByIdAsync(Guid id);
    }
}
