using GameStore.DomainModels.Enums;
using GameStore.DomainModels.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.DAL.Abstractions.Interfaces
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetSqlOrdersBetweenDatesAsync(DateTime? minDate, DateTime? maxDate, bool includeDeletedRows = false);

        Task ChangeOrderStatusAsync(OrderStatus status, Guid orderId);

        Task CreateAsync(Order item);

        Task DeleteAsync(Guid id);

        Task DeleteAsync(Order order);

        Task<Order> FindAsync(string id, string cultureCode = null, bool isIncludeDeleted = false);

        Task<Order> FindByCustomerIdAsync(Guid id, string cultureCode = null, bool isIncludeDeleted = false);

        Task<List<Order>> FindByIdsAsync(List<string> ids, bool isIncludeDeleted = false);

        Task<List<Order>> GetBetweenDatesAsync(DateTime minDate, DateTime maxDate);

        Task<List<Order>> GetExpiredOrdersAsync(int ordersAmount, bool isIncludeDeleted = false);

        Task<List<Order>> GetOrdersAfterDateAsync(DateTime minDate);

        Task<List<Order>> GetOrdersBeforeDateAsync(DateTime maxDate);

        Task<List<Order>> GetOrdersByOrderStatusAsync(OrderStatus status, bool isIncludeDeleted = false);

        Task ProcessOrderAsync(DateTime expirationDate, Guid id);

        Task<List<Order>> GetAllAsync(bool isIncludeDeleted = false);

        Task RecoverAsync(Guid id);

        Task SoftDeleteAsync(Guid id);

        Task UpdateAsync(Order item);
        Task ChangeCustomer(Guid orderId, Guid customerId);
    }
}
