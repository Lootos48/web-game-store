using AutoMapper;
using GameStore.DAL.Abstractions.Interfaces;
using GameStore.DAL.Entities;
using GameStore.DAL.Entities.MongoEntities;
using GameStore.DAL.Repositories.MongoDbRepositories.Interfaces;
using GameStore.DAL.Repositories.MsSqlDdRepositories.Interfaces;
using GameStore.DAL.Util.CacheManagers.Interfaces;
using GameStore.DAL.Workflows.Interfaces;
using GameStore.DomainModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore.DAL.Repositories
{
    public class DistributorRepository : IDistributorRepository
    {
        private readonly IPublisherRepository _publisherRepository;
        private readonly ISupplierMongoRepository _supplierRepository;

        private readonly IPublisherSupplierMappingWorkflow _publisherSupplierMappingWorkflow;
        private readonly ICacheManager _cacheManager;

        private readonly IMapper _mapper;

        public DistributorRepository(
            IPublisherRepository publisherRepository,
            ISupplierMongoRepository supplierRepository,
            IPublisherSupplierMappingWorkflow publisherSupplierMappingWorkflow,
            ICacheManager cacheManager,
            IMapper mapper)
        {
            _publisherRepository=publisherRepository;
            _supplierRepository=supplierRepository;
            _publisherSupplierMappingWorkflow=publisherSupplierMappingWorkflow;
            _cacheManager=cacheManager;
            _mapper=mapper;
        }

        public async Task<bool> IsDistributorUniqueAsync(Distributor distributor)
        {
            PublisherEntity publisher = _mapper.Map<PublisherEntity>(distributor);

            Task<bool> IsPublisherUniqueTask = _publisherRepository.IsPublisherUnique(publisher);
            Task<bool> IsSupplierUniqueTask = _supplierRepository.IsSupplierUnique(distributor.CompanyName);

            bool[] isDistributorUniqueTaskResult = await Task.WhenAll(IsPublisherUniqueTask, IsSupplierUniqueTask);

            foreach (bool isDistributorUnique in isDistributorUniqueTaskResult)
            {
                if (!isDistributorUnique)
                {
                    return false;
                }
            }

            return true;
        }

        public Task CreateAsync(Distributor item)
        {
            var entity = _mapper.Map<PublisherEntity>(item);

            return _publisherRepository.CreateEntityAsync(entity);
        }

        public async Task<Distributor> FindAsync(string id, bool isIncludeDeleted = false)
        {
            if (Guid.TryParse(id, out Guid publisherId))
            {
                var publisher = await _publisherRepository.FindEntityByIdAsync(publisherId, isIncludeDeleted);

                return _mapper.Map<Distributor>(publisher);
            }

            if (!int.TryParse(id, out int supplierId))
            {
                throw new ArgumentException("Id was'nt in correct format");
            }

            var supplier = await _supplierRepository.FindByIdAsync(supplierId);
            return _mapper.Map<Distributor>(supplier);
        }

        public async Task<List<Distributor>> FindByIdsAsync(List<string> ids, bool isIncludeDeleted = false)
        {
            var supplierIds = new List<int?>();
            var publisherIds = new List<Guid>();

            SplitStringIdsIntoSpecificIdsLists(ids, supplierIds, publisherIds);

            var getPublisherByIdsTask = _publisherRepository.FindEntitiesByIdsAsync(publisherIds, isIncludeDeleted);
            var getSuppliersByIdsTask = _supplierRepository.FindByIdsAsync(supplierIds);

            return await MergeIntoDestributors(getPublisherByIdsTask, getSuppliersByIdsTask);
        }

        public async Task<Distributor> FindByNameAsync(string companyName, bool isIncludeDeleted = false)
        {
            var findPublisherTask = _publisherRepository.FindByNameAsync(companyName, isIncludeDeleted);
            var findSupplierTask = _supplierRepository.FindByCompanyName(companyName);

            await Task.WhenAll(findPublisherTask, findSupplierTask);

            var publisher = await findPublisherTask;
            if (publisher != null)
            {
                return _mapper.Map<Distributor>(publisher);
            }

            var supplier = await findSupplierTask;
            if (supplier is null)
            {
                return null;
            }

            var distributor = _mapper.Map<Distributor>(supplier);

            await SetupUserForSupplierAsync(supplier, distributor);

            return distributor;
        }

        public Task<List<Distributor>> GetAllAsync(bool isIncludeDeleted = false)
        {
            var getPublishersTask = _publisherRepository.GetAllEntitiesAsync(isIncludeDeleted);
            var getSuppliersTask = _supplierRepository.GetAllAsync();

            return MergeIntoDestributors(getPublishersTask, getSuppliersTask);
        }

        public Task RecoverAsync(Guid id)
        {
            return _publisherRepository.RecoverAsync(id);
        }

        public Task DeleteAsync(Guid id)
        {
            return _publisherRepository.DeleteByIdAsync(id);
        }

        public Task DeleteAsync(Distributor distributor)
        {
            if (Guid.TryParse(distributor.Id, out Guid publisherId))
            {
                var entity = _mapper.Map<PublisherEntity>(distributor);

                return _publisherRepository.DeleteEntityAsync(entity);
            }

            if (int.TryParse(distributor.Id, out int supplierId))
            {
                throw new InvalidOperationException("Can't delete supplie from NorthwindOnlineStore");
            }

            throw new ArgumentException("Not correct id format");
        }

        public async Task ConnectUserToDistributorAsync(Guid userId, string distributorId)
        {
            Guid publisherId;
            if (int.TryParse(distributorId, out int supplierId))
            {
                var mapping = await _publisherSupplierMappingWorkflow.GetMappingAsync(supplierId);
                if (mapping is null)
                {
                    mapping = await _publisherSupplierMappingWorkflow.CreateMappingAsync(supplierId);
                }

                publisherId = mapping.PublisherId;
            }
            else if (!Guid.TryParse(distributorId, out publisherId))
            {
                throw new ArgumentException("Id was'nt in correct format");
            }

            await _publisherRepository.ConnectUserToPublisherAsync(userId, publisherId);
        }

        public Task DisconnectUserFromDistributorAsync(Guid userId)
        {
            return _publisherRepository.DisconnectUserFromPublisherAsync(userId);
        }

        public async Task<Distributor> FindByUserIdAsync(Guid UserId, bool isIncludeDeleted = false)
        {
            var publisher = await _publisherRepository.FindByUserIdAsync(UserId, isIncludeDeleted);

            return _mapper.Map<Distributor>(publisher);
        }

        public Task SoftDeleteAsync(Guid id)
        {
            return _publisherRepository.SoftDeleteAsync(id);
        }

        public Task UpdateAsync(Distributor item)
        {
            if (!Guid.TryParse(item.Id, out Guid publisherId))
            {
                throw new InvalidOperationException("Update operation cannot be applied to Northwind suppliers");
            }

            var entity = _mapper.Map<PublisherEntity>(item);

            return _publisherRepository.UpdateEntityAsync(entity);
        }

        private void SplitStringIdsIntoSpecificIdsLists(List<string> ids, List<int?> supplierIds, List<Guid> publisherIds)
        {
            foreach (var id in ids)
            {
                if (Guid.TryParse(id, out Guid gameId))
                {
                    publisherIds.Add(gameId);
                }

                if (int.TryParse(id, out int productId))
                {
                    supplierIds.Add(productId);
                }
            }
        }

        private async Task<List<Distributor>> MergeIntoDestributors(Task<List<PublisherEntity>> getPublisherByIdsTask, Task<List<SupplierMongoEntity>> getSuppliersByIdsTask)
        {
            await Task.WhenAll(getPublisherByIdsTask, getSuppliersByIdsTask);

            var publishers = await getPublisherByIdsTask;
            var suppliers = await getSuppliersByIdsTask;

            List<Distributor> distributorsList = new List<Distributor>();

            var mappedSuppliers = _mapper.Map<List<Distributor>>(suppliers);

            await SetupUsersForSuppliersAsync(mappedSuppliers);

            distributorsList.AddRange(_mapper.Map<List<Distributor>>(publishers));
            distributorsList.AddRange(mappedSuppliers);

            return distributorsList;
        }

        private async Task SetupUserForSupplierAsync(SupplierMongoEntity supplier, Distributor distributor)
        {
            var mapping = await _publisherSupplierMappingWorkflow.GetMappingAsync(supplier.SupplierId.Value);
            if (mapping != null)
            {
                var tempPub = await _publisherRepository.FindEntityByIdAsync(mapping.PublisherId);
                distributor.UserId = tempPub.UserId;
            }
        }

        private async Task SetupUsersForSuppliersAsync(List<Distributor> mappedSuppliers)
        {
            var mappings = await _cacheManager.GetPublisherSupplierMappingsCacheAsync();

            var tempPubsIds = mappings.Select(x => x.PublisherId).ToList();
            var tempPublishers = await _publisherRepository.FindEntitiesByIdsAsync(tempPubsIds);

            foreach (var mapping in mappings)
            {
                var supWithTempPub = mappedSuppliers.FirstOrDefault(x => x.Id == mapping.SupplierId.ToString());
                if (supWithTempPub != null)
                {
                    var tempPub = tempPublishers.FirstOrDefault(x => x.Id == mapping.PublisherId);
                    supWithTempPub.UserId = tempPub.UserId;
                }
            }
        }
    }
}
