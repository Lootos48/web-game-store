using AutoMapper;
using GameStore.DAL.Abstractions.Interfaces;
using GameStore.DAL.Entities;
using GameStore.DAL.Repositories.MsSqlDdRepositories;
using GameStore.DomainModels.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore.DAL.Repositories
{
    public class CommentRepository : SqlRepository<CommentEntity>, ICommentRepository
    {
        private readonly IMapper _mapper;

        public CommentRepository(
            DbContext context,
            IMapper mapper,
            ILogger<CommentRepository> logger) : base(context, logger) 
        {
            _mapper = mapper;
        }

        public Task CreateAsync(Comment item)
        {
            var entity = _mapper.Map<CommentEntity>(item);
            return CreateEntityAsync(entity);
        }

        public async Task<Comment> FindByIdAsync(Guid id, bool isIncludeDeleted = false)
        {
            var entity = await _dbSet
                .Include(x => x.Author)
                .FirstOrDefaultAsync(x => x.Id == id && (!x.IsDeleted || isIncludeDeleted));

            return _mapper.Map<Comment>(entity);
        }

        public async Task<List<Comment>> GetAllAsync(bool isIncludeDeleted = false)
        {
            var entities = await GetAllEntitiesAsync(isIncludeDeleted);
            return _mapper.Map<List<Comment>>(entities);
        }

        public async Task<List<Comment>> GetByGameIdAsync(Guid gameId, bool isIncludeDeleted = false)
        {
            List<CommentEntity> commentsEntities = await _dbSet
                .Include(x => x.Author)
                .Where(c => c.GameId == gameId && (!c.IsDeleted || isIncludeDeleted))
                .ToListAsync();

            return _mapper.Map<List<Comment>>(commentsEntities);
        }

        public Task UpdateAsync(Comment item)
        {
            var entity = _mapper.Map<CommentEntity>(item);
            return UpdateEntityAsync(entity);
        }

        public Task DeleteAsync(Comment item)
        {
            return DeleteByIdAsync(item.Id);
        }
    }
}
