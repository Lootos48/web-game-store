using GameStore.DAL.Entities;
using GameStore.DAL.Repositories.MsSqlDdRepositories.Interfaces;
using GameStore.DAL.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore.DAL.Repositories.MsSqlDdRepositories
{
    public class PublisherRepository : SqlRepository<PublisherEntity>, IPublisherRepository
    {
        private readonly MongoIntegrationSettings _mongoIntegrationSettings;
        public PublisherRepository(
            DbContext context,
            ILogger<PublisherRepository> logger,
            MongoIntegrationSettings defaultValuesSettings) : base(context, logger)
        {
            _mongoIntegrationSettings = defaultValuesSettings;
        }

        public Task<bool> IsPublisherUnique(PublisherEntity publisher)
        {
            return _dbSet.AllAsync(p => p.CompanyName != publisher.CompanyName ||
            (p.Id == publisher.Id && p.CompanyName == publisher.CompanyName));
        }

        public override Task<List<PublisherEntity>> GetAllEntitiesAsync(bool includesDeletedRows = false)
        {
            return _dbSet.Where(p =>
            p.Description != _mongoIntegrationSettings.TemporaryName
            && (!p.IsDeleted || includesDeletedRows)).ToListAsync();
        }

        public Task ConnectUserToPublisherAsync(Guid userId, Guid publisherId)
        {
            FormattableString query = $@"UPDATE [Publishers] SET [UserId] = {userId} WHERE [Id] = {publisherId}";
            return _context.Database.ExecuteSqlInterpolatedAsync(query);
        }

        public Task DisconnectUserFromPublisherAsync(Guid userId)
        {
            FormattableString query = $@"UPDATE [Publishers] SET [UserId] = NULL WHERE [UserId] = {userId}";
            return _context.Database.ExecuteSqlInterpolatedAsync(query);
        }

        public Task<PublisherEntity> FindByUserIdAsync(Guid userId, bool isIncludeDeleted)
        {
            return _dbSet.Include(p => p.PublishedGames)
                .FirstOrDefaultAsync(p => p.UserId == userId && (!p.IsDeleted || isIncludeDeleted));
        }

        public Task<PublisherEntity> FindByNameAsync(string companyName, bool isIncludeDeleted = false)
        {
            return _dbSet.Include(p => p.PublishedGames)
                .FirstOrDefaultAsync(p => p.CompanyName == companyName && (!p.IsDeleted || isIncludeDeleted));
        }
    }
}
