using GameStore.DAL.Entities;
using GameStore.DAL.Repositories.MsSqlDdRepositories.Interfaces;
using GameStore.DomainModels.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace GameStore.DAL.Repositories.MsSqlDdRepositories
{
    public abstract class SqlRepository<TEntity> : ISqlRepository<TEntity> where TEntity : Entity
    {
        protected readonly DbContext _context;
        protected readonly DbSet<TEntity> _dbSet;
        protected readonly ILogger<SqlRepository<TEntity>> _logger;

        protected SqlRepository(DbContext context, ILogger<SqlRepository<TEntity>> logger)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
            _logger = logger;
        }

        public virtual Task<List<TEntity>> GetAllAsync()
        {
            return GetAllEntitiesAsync(false);
        }

        public virtual Task<List<TEntity>> GetAllEntitiesAsync(bool includesDeletedRows = false)
        {
            return _dbSet
                .Where(x => !x.IsDeleted || includesDeletedRows)
                .ToListAsync();
        }

        public virtual Task<List<TEntity>> FindEntitiesByIdsAsync(List<Guid> ids, bool isIncludeDeleted = false)
        {
            return _dbSet
                .Where(e => ids.Contains(e.Id) && (!e.IsDeleted || isIncludeDeleted))
                .ToListAsync();
        }

        public virtual Task<TEntity> FindEntityByIdAsync(Guid id, bool includesDeletedRows = false)
        {
            return _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && (!x.IsDeleted || includesDeletedRows));
        }

        public Task CreateEntityAsync(TEntity entity)
        {
            _logger.LogInformation("{Action}\nEntity of Type: {EntityType}\n{@Version}", "Create", typeof(TEntity), entity);

            _dbSet.Add(entity);
            return _context.SaveChangesAsync();
        }

        public Task UpdateEntityAsync(TEntity entity)
        {
            var oldVersion = _dbSet.AsNoTracking().FirstOrDefault(e => e.Id == entity.Id);

            _logger.LogInformation("{Action}\nEntity of Type: {EntityType}\n{@Version}\n{@OldVersion}",
                                   "Update",
                                   typeof(TEntity),
                                   entity,
                                   oldVersion);

            _context.Entry(entity).State = EntityState.Modified;

            return _context.SaveChangesAsync();
        }

        public Task SoftDeleteAsync(Guid id)
        {
            return ChangeIsDeletedStatus(id, true);
        }

        public Task RecoverAsync(Guid id)
        {
            return ChangeIsDeletedStatus(id, false);
        }

        public Task DeleteByIdAsync(Guid id)
        {
            string tableName = GetTableName();

            _logger.LogInformation("{Action}\nEntity of Type: {EntityType}\n{@Id}", "Remove", typeof(TEntity), id);

            FormattableString query = $@"DELETE FROM {tableName} WHERE [Id] = {id}";
            return _context.Database.ExecuteSqlInterpolatedAsync(query);
        }

        public Task DeleteEntityAsync(TEntity entity)
        {
            if (entity is null)
            {
                throw new NotFoundException("Entity was not found");
            }

            return DeleteByIdAsync(entity.Id);
        }

        private Task ChangeIsDeletedStatus(Guid id, bool isDeleted)
        {
            FormattableString query = BuidIsDeletedChangingQuery(id, isDeleted);
            return _context.Database.ExecuteSqlInterpolatedAsync(query);
        }

        private FormattableString BuidIsDeletedChangingQuery(Guid id, bool isDeleted)
        {
            string dateOfDelete = isDeleted ? DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") : null;
            string tableName = GetTableName();

            string query = "UPDATE [" + tableName + "] SET [IsDeleted] = {0}, [DateOfDelete] = {1} WHERE [Id] = {2};";

            return FormattableStringFactory.Create(query, isDeleted, dateOfDelete, id);
        }

        private string GetTableName()
        {
            var entityType = _context.Model.FindEntityType(typeof(TEntity));
            var tableName = entityType.GetTableName();
            return tableName;
        }
    }
}
