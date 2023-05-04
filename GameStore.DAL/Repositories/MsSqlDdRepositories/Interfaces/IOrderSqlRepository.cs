using GameStore.DAL.Entities;
using GameStore.DomainModels.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.DAL.Repositories.MsSqlDdRepositories.Interfaces
{
    public interface IOrderSqlRepository : ISqlRepository<OrderEntity>
    {
        Task ChangeCustomer(Guid orderId, Guid customerId);

        Task ChangeOrderStatusAsync(OrderStatus status, Guid orderId);

        Task<OrderEntity> FindOrderByIdAsync(Guid id, string cultureCode = null, bool isIncludeDeleted = false);

        Task<OrderEntity> FindByCustomerIdAsync(Guid id, string cultureCode = null, bool isIncludeDeleted = false);

        Task<List<OrderEntity>> GetBetweenDatesAsync(DateTime minDate, DateTime maxDate, bool isIncludeDeleted = false);

        Task<List<OrderEntity>> GetExpiredOrdersAsync(int ordersAmount, bool isIncludeDeleted = false);

        Task<List<OrderEntity>> GetOrdersAfterDateAsync(DateTime minDate, bool isIncludeDeleted = false);

        Task<List<OrderEntity>> GetOrdersBeforeAsync(DateTime maxDate, bool isIncludeDeleted = false);

        Task<List<OrderEntity>> GetOrdersByOrderStatusAsync(OrderStatus status, bool isIncludeDeleted = false);

        Task ProcessOrderAsync(DateTime expirationDate, Guid id);
    }
}