using GameStore.DAL.Entities;
using GameStore.DAL.Repositories.MsSqlDdRepositories.Interfaces;
using GameStore.DAL.Settings;
using GameStore.DAL.Util.CacheManagers.Interfaces;
using GameStore.DAL.Workflows.Interfaces;
using GameStore.DomainModels.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore.DAL.Workflows
{
    public class PublisherSupplierMappingWorkflow : IPublisherSupplierMappingWorkflow
    {
        private readonly IPublisherRepository _publisherRepository;
        private readonly ICacheManager _cacheManager;
        private readonly MongoIntegrationSettings _mongoIntegrationSettings;

        public PublisherSupplierMappingWorkflow(IPublisherRepository publisherRepository,
                                                ICacheManager cacheManager,
                                                MongoIntegrationSettings mongoIntegrationSettings)
        {
            _publisherRepository = publisherRepository;
            _cacheManager = cacheManager;
            _mongoIntegrationSettings = mongoIntegrationSettings;
        }
        //SetupPublisherFromMappings
        public async Task<PublisherSupplierMapping> GetMappingAsync(int supplierId)
        {
            var publishersSuppliersMappingsList = await _cacheManager.GetPublisherSupplierMappingsCacheAsync();

            return publishersSuppliersMappingsList.FirstOrDefault(x => x.SupplierId == supplierId);
        }

        public async Task<PublisherSupplierMapping> CreateMappingAsync(int supplierId)
        {
            Guid tempPublisherId = await CreateTemporalPublisherAsync();

            return await CreatePublisherSupplierMappingAsync(supplierId, tempPublisherId);
        }

        private async Task<PublisherSupplierMapping> CreatePublisherSupplierMappingAsync(int supplierId, Guid tempPublisherId)
        {
            var newMapping = new PublisherSupplierMapping
            {
                SupplierId = supplierId,
                PublisherId = tempPublisherId
            };

            await _cacheManager.AddPublisherSupplierMappingCache(newMapping);

            return newMapping;
        }

        private async Task<Guid> CreateTemporalPublisherAsync()
        {
            var tempPublisher = new PublisherEntity
            {
                Id = Guid.NewGuid(),
                CompanyName = null,
                Description = _mongoIntegrationSettings.TemporaryName
            };
            await _publisherRepository.CreateEntityAsync(tempPublisher);

            return tempPublisher.Id;
        }
    }
}
