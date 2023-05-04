using AutoMapper;
using GameStore.DAL.Abstractions.Interfaces;
using GameStore.DAL.Entities;
using GameStore.DAL.Entities.MongoEntities;
using GameStore.DAL.Repositories.MongoDbRepositories.Interfaces;
using GameStore.DAL.Repositories.MsSqlDdRepositories.Interfaces;
using GameStore.DAL.Settings;
using GameStore.DAL.Util.CacheManagers.Interfaces;
using GameStore.DomainModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore.DAL.Repositories
{
    public class OrderDetailsRepository : IOrderDetailsRepository
    {
        private readonly IOrderDetailsSqlRepository _orderDetailsSqlRepository;
        private readonly IOrderDetailsMongoRepository _orderDetailsMongoRepository;
        private readonly IGameRepository _gameRepository;

        private readonly MongoIntegrationSettings _mongoIntegrationSettings;

        private readonly IMapper _mapper;
        private readonly ICacheManager _cacheManager;

        public OrderDetailsRepository(
            IOrderDetailsSqlRepository orderDetailsSqlRepository,
            IOrderDetailsMongoRepository orderDetailsMongoRepository,
            IGameRepository gameRepository,
            MongoIntegrationSettings defaultValuesSettings,
            ICacheManager cacheManager,
            IMapper mapper)
        {
            _orderDetailsSqlRepository = orderDetailsSqlRepository;
            _orderDetailsMongoRepository = orderDetailsMongoRepository;
            _gameRepository = gameRepository;
            _mongoIntegrationSettings = defaultValuesSettings;
            _mapper = mapper;
            _cacheManager = cacheManager;
        }

        public Task ChangeProductQuantity(Guid id, int quantity)
        {
            return _orderDetailsSqlRepository.ChangeProductQuantity(id, quantity);
        }

        public async Task<OrderDetails> FindByIdAsync(Guid id, bool isIncludeDeleted = false)
        {
            OrderDetailsEntity orderDetails = await _orderDetailsSqlRepository.FindEntityByIdAsync(id, isIncludeDeleted);

            return _mapper.Map<OrderDetails>(orderDetails);
        }
        public Task SoftDeleteRangeAsync(List<OrderDetails> excessDetails)
        {
            var excessDetailsIds = excessDetails.Select(d => d.Id).ToList();

            return _orderDetailsSqlRepository.SoftDeleteRangeAsync(excessDetailsIds);
        }

        public Task ChangeOrder(List<OrderDetails> orderDetails, Guid newOrderId)
        {
            var detailsIds = orderDetails.Select(d => d.Id).ToList();

            return _orderDetailsSqlRepository.ChangeOrder(detailsIds, newOrderId);
        }

        public async Task<OrderDetails> FindAsync(int orderId, int productId)
        {
            var mongoOrderDetails = await _orderDetailsMongoRepository.FindOrderDetails(orderId, productId);
            return _mapper.Map<OrderDetails>(mongoOrderDetails);
        }

        public async Task<List<OrderDetails>> FindByIdsAsync(List<Guid> ids, bool isIncludeDeleted = false)
        {
            List<OrderDetailsEntity> orderDetails = await _orderDetailsSqlRepository.FindEntitiesByIdsAsync(ids, isIncludeDeleted);

            return _mapper.Map<List<OrderDetails>>(orderDetails);
        }

        public async Task<List<OrderDetails>> FindOrdeDetailsByOrderId(string orderId, bool isIncludeDeleted = false)
        {
            if (Guid.TryParse(orderId, out Guid sqlOrderId))
            {
                List<OrderDetailsEntity> sqlOrderDetails = await _orderDetailsSqlRepository.FindByOrderIdAsync(sqlOrderId, isIncludeDeleted);
                return _mapper.Map<List<OrderDetails>>(sqlOrderDetails);
            }
            else if (int.TryParse(orderId, out int mongoOrderId))
            {
                var mongoOrderDetails = await _orderDetailsMongoRepository.FindOrderDetailsByOrderId(mongoOrderId);
                return _mapper.Map<List<OrderDetails>>(mongoOrderDetails);
            }
            else
            {
                throw new ArgumentException("Id wasn't in correct format");
            }
        }

        public async Task<List<OrderDetails>> GetAllAsync(bool isIncludeDeleted = false)
        {
            var getSqlOrderDetailsTask = _orderDetailsSqlRepository.GetAllEntitiesAsync(isIncludeDeleted);
            var getMongoOrderDetailsTask = _orderDetailsMongoRepository.GetAllAsync();

            return await MergeIntoOrdersDetailsAsync(getSqlOrderDetailsTask, getMongoOrderDetailsTask);
        }

        public Task<List<OrderDetails>> GetAllAsync()
        {
            return GetAllAsync(false);
        }

        public async Task CreateAsync(OrderDetails item)
        {
            if (int.TryParse(item.ProductId, out int productId))
            {
                var goodsProductMappingsList = await _cacheManager.GetGoodsProductMappingsCacheAsync();
                var mapping = goodsProductMappingsList.FirstOrDefault(m => m.ProductId == productId);

                if (mapping == null)
                {
                    Guid temporalGameId = Guid.NewGuid();
                    item.ProductId = temporalGameId.ToString();

                    await PartiallyMoveProductAsTemporaryGame(temporalGameId, productId);
                }
                else
                {
                    item.ProductId = mapping.GameId.ToString();
                    await _cacheManager.UpdateGoodsProductMappingsCacheAsync();
                }

            }
            else if (!Guid.TryParse(item.ProductId, out Guid gameId))
            {
                throw new ArgumentException("Id was'nt in correct format");
            }

            OrderDetailsEntity orderDetails = _mapper.Map<OrderDetailsEntity>(item);
            await _orderDetailsSqlRepository.CreateEntityAsync(orderDetails);
        }

        public Task UpdateAsync(OrderDetails item)
        {
            OrderDetailsEntity orderDetails = _mapper.Map<OrderDetailsEntity>(item);
            return _orderDetailsSqlRepository.UpdateEntityAsync(orderDetails);
        }

        public Task RecoverAsync(Guid id)
        {
            return _orderDetailsSqlRepository.RecoverAsync(id);
        }

        public Task DeleteAsync(Guid id)
        {
            return _orderDetailsSqlRepository.DeleteByIdAsync(id);
        }

        public Task DeleteAsync(OrderDetails item)
        {
            if (item.Id == Guid.Empty)
            {
                throw new InvalidOperationException("Can't delete order details from NorthwindOnlineStore");
            }

            var entity = _mapper.Map<OrderDetailsEntity>(item);
            return _orderDetailsSqlRepository.DeleteEntityAsync(entity);
        }

        public Task SoftDeleteAsync(Guid id)
        {
            return _orderDetailsSqlRepository.SoftDeleteAsync(id);
        }

        private async Task<List<OrderDetails>> MergeIntoOrdersDetailsAsync(Task<List<OrderDetailsEntity>> getSqlOrderDetailsTask,
                                                                   Task<List<OrderDetailsMongoEntity>> getMongoOrderDetailsTask)
        {
            await Task.WhenAll(getSqlOrderDetailsTask, getMongoOrderDetailsTask);

            var sqlOrderDetails = await getSqlOrderDetailsTask;
            var mongoOrderDetails = await getMongoOrderDetailsTask;

            var orderDetails = new List<OrderDetails>();

            orderDetails.AddRange(_mapper.Map<List<OrderDetails>>(sqlOrderDetails));
            orderDetails.AddRange(_mapper.Map<List<OrderDetails>>(mongoOrderDetails));

            return orderDetails;
        }

        private async Task PartiallyMoveProductAsTemporaryGame(Guid temporalGameId, int productId)
        {
            GameEntity temporal = new GameEntity()
            {
                Id = temporalGameId,
                Name = _mongoIntegrationSettings.TemporaryName
            };

            await _gameRepository.CreateEntityAsync(temporal);

            await CreateTemporaryGameProductRelation(productId, temporalGameId);
        }

        private async Task CreateTemporaryGameProductRelation(int productId, Guid temporaryGameId)
        {
            GoodsProductMapping mapping = new GoodsProductMapping()
            {
                GameId = temporaryGameId,
                ProductId = productId,
                Id = Guid.NewGuid()
            };

            await _cacheManager.AddGoodsProductMappingCache(mapping);
        }

        public Task DeleteByIdAsync(Guid id)
        {
            return _orderDetailsSqlRepository.DeleteByIdAsync(id);
        }

        public Task BatchChangeOrderDetailsQuantityAsync(List<OrderDetails> orderDetails)
        {
            var entities = _mapper.Map<List<OrderDetailsEntity>>(orderDetails);
            return _orderDetailsSqlRepository.BatchChangeProductQuantityAsync(entities);
        }
    }
}
