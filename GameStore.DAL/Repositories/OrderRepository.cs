using AutoMapper;
using GameStore.DAL.Abstractions.Interfaces;
using GameStore.DAL.Entities;
using GameStore.DAL.Entities.MongoEntities;
using GameStore.DAL.Repositories.MongoDbRepositories.Interfaces;
using GameStore.DAL.Repositories.MsSqlDdRepositories.Interfaces;
using GameStore.DAL.Util.CacheManagers.Interfaces;
using GameStore.DomainModels.Enums;
using GameStore.DomainModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore.DAL.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IOrderSqlRepository _orderSqlRepository;
        private readonly IOrderMongoRepository _orderMongoRepository;
        private readonly IProductMongoRepository _productRepository;
        private readonly IShipperMongoRepository _shipperMongoRepository;

        private readonly ICacheManager _cacheManager;
        private readonly IMapper _mapper;

        public OrderRepository(
            IOrderSqlRepository orderSqlRepository,
            IOrderMongoRepository orderMongoRepository,
            IShipperMongoRepository shipperMongoRepository,
            IProductMongoRepository productRepository,
            ICacheManager cacheManager,
            IMapper mapper)
        {
            _orderSqlRepository = orderSqlRepository;
            _orderMongoRepository = orderMongoRepository;
            _shipperMongoRepository = shipperMongoRepository;
            _productRepository = productRepository;
            _mapper = mapper;
            _cacheManager = cacheManager;
        }

        public async Task<List<Order>> GetSqlOrdersBetweenDatesAsync(DateTime? minDate, DateTime? maxDate, bool includeDeletedRows = false)
        {
            List<OrderEntity> orders = new List<OrderEntity>();
            if (!minDate.HasValue && maxDate.HasValue)
            {
                orders = await _orderSqlRepository.GetOrdersBeforeAsync(maxDate.Value);
            }
            else if (minDate.HasValue && !maxDate.HasValue)
            {
                orders = await _orderSqlRepository.GetOrdersAfterDateAsync(minDate.Value);
            }
            else if (minDate.HasValue && maxDate.HasValue)
            {
                orders = await _orderSqlRepository.GetBetweenDatesAsync(minDate.Value, maxDate.Value, includeDeletedRows);
            }

            orders = orders.OrderByDescending(o => o.OrderDate).ToList();
            return _mapper.Map<List<Order>>(orders);
        }

        public async Task<Order> FindByCustomerIdAsync(Guid id, string cultureCode = null, bool isIncludeDeleted = false)
        {
            OrderEntity entity = await _orderSqlRepository.FindByCustomerIdAsync(id, cultureCode, isIncludeDeleted);

            var order = _mapper.Map<Order>(entity);

            await SetupOrder(order);

            return order;
        }

        public async Task<List<Order>> GetOrdersByOrderStatusAsync(OrderStatus status, bool isIncludeDeleted = false)
        {
            List<OrderEntity> ordersEntity = await _orderSqlRepository.GetOrdersByOrderStatusAsync(status, isIncludeDeleted);

            var orders = _mapper.Map<List<Order>>(ordersEntity);

            return await SetupOrdersShippers(orders);
        }

        public async Task<List<Order>> GetExpiredOrdersAsync(int ordersAmount, bool isIncludeDeleted = false)
        {
            List<OrderEntity> orders = await _orderSqlRepository.GetExpiredOrdersAsync(ordersAmount, isIncludeDeleted);

            return _mapper.Map<List<Order>>(orders);
        }

        public Task ChangeOrderStatusAsync(OrderStatus status, Guid orderId)
        {
            return _orderSqlRepository.ChangeOrderStatusAsync(status, orderId);
        }

        public Task ProcessOrderAsync(DateTime expirationDate, Guid id)
        {
            return _orderSqlRepository.ProcessOrderAsync(expirationDate, id);
        }

        public Task<List<Order>> GetAllAsync(bool isIncludeDeleted = false)
        {
            var getSqlOrderTask = _orderSqlRepository.GetAllEntitiesAsync(isIncludeDeleted);
            var getMongoOrderTask = _orderMongoRepository.GetAllAsync();

            return MergeIntoOrders(getSqlOrderTask, getMongoOrderTask);
        }

        public Task<List<Order>> GetBetweenDatesAsync(DateTime minDate, DateTime maxDate)
        {
            var getSqlOrderTask = _orderSqlRepository.GetBetweenDatesAsync(minDate, maxDate);
            var getMongoOrderTask = _orderMongoRepository.GetBetweenDatesAsync(minDate, maxDate);

            return MergeIntoOrders(getSqlOrderTask, getMongoOrderTask);
        }

        public Task<List<Order>> GetOrdersBeforeDateAsync(DateTime maxDate)
        {
            Task<List<OrderEntity>> getSqlORderTask = _orderSqlRepository.GetOrdersBeforeAsync(maxDate);
            Task<List<OrderMongoEntity>> getMongoOrderTask = _orderMongoRepository.GetOrdersBeforeAsync(maxDate);

            return MergeIntoOrders(getSqlORderTask, getMongoOrderTask);
        }

        public Task<List<Order>> GetOrdersAfterDateAsync(DateTime minDate)
        {
            Task<List<OrderEntity>> getSqlORderTask = _orderSqlRepository.GetOrdersAfterDateAsync(minDate);
            Task<List<OrderMongoEntity>> getMongoOrderTask = _orderMongoRepository.GetOrdersAfterAsync(minDate);

            return MergeIntoOrders(getSqlORderTask, getMongoOrderTask);
        }

        public async Task<Order> FindAsync(string id, string cultureCode = null, bool isIncludeDeleted = false)
        {
            Order order;
            if (Guid.TryParse(id, out Guid sqlOrderId))
            {
                var orderEntity = await _orderSqlRepository.FindOrderByIdAsync(sqlOrderId, cultureCode, isIncludeDeleted);
                order = _mapper.Map<Order>(orderEntity);
            }
            else if (!int.TryParse(id, out int mongoOrderId))
            {
                throw new ArgumentException("Id wasn't in correct format");
            }
            else
            {
                var orderMongo = await _orderMongoRepository.FindById(mongoOrderId);
                order = _mapper.Map<Order>(orderMongo);
            }

            await SetupOrder(order);

            return order;
        }

        public Task<List<Order>> FindByIdsAsync(List<string> ids, bool isIncludeDeleted = false)
        {
            var mongoIds = new List<int?>();
            var sqlIds = new List<Guid>();

            SetupListsWithSpecificIds(ids, mongoIds, sqlIds);

            var getSqlOrderByIdsTask = _orderSqlRepository.FindEntitiesByIdsAsync(sqlIds, isIncludeDeleted);
            var getMongoOrderByIdsTask = _orderMongoRepository.FindByIdsAsync(mongoIds);

            return MergeIntoOrders(getSqlOrderByIdsTask, getMongoOrderByIdsTask);
        }

        public Task CreateAsync(Order item)
        {
            OrderEntity order = _mapper.Map<OrderEntity>(item);

            return _orderSqlRepository.CreateEntityAsync(order);
        }

        public Task ChangeCustomer(Guid orderId, Guid customerId)
        {
            return _orderSqlRepository.ChangeCustomer(orderId, customerId);
        }

        public Task UpdateAsync(Order item)
        {
            if (!Guid.TryParse(item.Id, out Guid result))
            {
                throw new InvalidOperationException("This operation cannot be applied to Northwind orders");
            }

            OrderEntity order = _mapper.Map<OrderEntity>(item);
            return _orderSqlRepository.UpdateEntityAsync(order);
        }

        public Task RecoverAsync(Guid id)
        {
            return _orderSqlRepository.RecoverAsync(id);
        }

        public Task SoftDeleteAsync(Guid id)
        {
            return _orderSqlRepository.SoftDeleteAsync(id);
        }

        public Task DeleteAsync(Guid id)
        {
            return _orderSqlRepository.DeleteByIdAsync(id);
        }

        public Task DeleteAsync(Order order)
        {
            if (Guid.TryParse(order.Id, out Guid id))
            {
                var orderEntity = _mapper.Map<OrderEntity>(order);

                return _orderSqlRepository.DeleteEntityAsync(orderEntity);
            }

            if (int.TryParse(order.Id, out int orderId))
            {
                throw new InvalidOperationException("Can't delete order from NorthwindOnlineStore, use a soft delete instead");
            }

            throw new ArgumentException("Not correct id format");
        }

        private static void SetupListsWithSpecificIds(List<string> ids, List<int?> mongoIds, List<Guid> sqlIds)
        {
            foreach (var id in ids)
            {
                if (Guid.TryParse(id, out Guid gameId))
                {
                    sqlIds.Add(gameId);
                }

                if (int.TryParse(id, out int productId))
                {
                    mongoIds.Add(productId);
                }
            }
        }

        private async Task<List<Order>> MergeIntoOrders(Task<List<OrderEntity>> getSqlOrderTask, Task<List<OrderMongoEntity>> getMongoOrderTask)
        {
            await Task.WhenAll(getSqlOrderTask, getMongoOrderTask);

            var sqlOrders = await getSqlOrderTask;
            var mongoOrders = await getMongoOrderTask;

            List<Order> orders = new List<Order>();

            orders.AddRange(_mapper.Map<List<Order>>(sqlOrders));

            await SetupOrdersWithProducts(orders);

            orders.AddRange(_mapper.Map<List<Order>>(mongoOrders));

            return await SetupOrdersShippers(orders);
        }

        private async Task<List<Order>> SetupOrdersShippers(List<Order> orders)
        {
            List<Task> tasks = new List<Task>();
            foreach (var order in orders)
            {
                var task = SetupOrderShipper(order);
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);

            return orders;
        }

        private async Task SetupOrderShipper(Order order)
        {
            if (order.ShipVia.HasValue)
            {
                var shipper = await _shipperMongoRepository.FindById(order.ShipVia.Value);
                order.Shipper = _mapper.Map<Shipper>(shipper);
            }
        }


        private Task SetupOrdersWithProducts(List<Order> orders)
        {
            List<Task> tasks = new List<Task>();
            foreach (var order in orders)
            {
                var task = SetupOrderWithProducts(order);
                tasks.Add(task);
            }

            return Task.WhenAll(tasks);
        }

        private async Task SetupOrderWithProducts(Order order)
        {
            foreach (var orderDetails in order.OrderDetails)
            {
                var goodsProductMappingsList = await _cacheManager.GetGoodsProductMappingsCacheAsync();
                GoodsProductMapping mapping = goodsProductMappingsList
                    .FirstOrDefault(c => c.GameId.ToString() == orderDetails.ProductId);

                if (mapping != null && mapping.ProductId.HasValue && !mapping.IsFullyMigrated)
                {
                    ProductMongoEntity product = await _productRepository.FindByIdAsync(mapping.ProductId.Value);

                    orderDetails.Product = _mapper.Map<Goods>(product);
                    orderDetails.ProductId = mapping.ProductId.Value.ToString();
                }
            }
        }

        private async Task SetupOrder(Order order)
        {
            if (order == null)
            {
                return;
            }

            await SetupOrderWithProducts(order);
            await SetupOrderShipper(order);
        }
    }
}
