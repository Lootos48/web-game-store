using GameStore.DomainModels.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.DAL.Abstractions.Interfaces
{
    public interface IOrderDetailsRepository
    {
        Task ChangeProductQuantity(Guid id, int quantity);

        Task<OrderDetails> FindAsync(int orderId, int productId);

        Task<List<OrderDetails>> FindOrdeDetailsByOrderId(string orderId, bool isIncludeDeleted = false);

        Task CreateAsync(OrderDetails item);

        Task DeleteAsync(OrderDetails item);

        Task<List<OrderDetails>> GetAllAsync(bool isIncludeDeleted = false);

        Task<OrderDetails> FindByIdAsync(Guid id, bool isIncludeDeleted = false);

        Task UpdateAsync(OrderDetails item);

        Task RecoverAsync(Guid id);

        Task SoftDeleteAsync(Guid id);

        Task DeleteByIdAsync(Guid id);

        Task SoftDeleteRangeAsync(List<OrderDetails> excessDetails);

        Task ChangeOrder(List<OrderDetails> orderDetails, Guid newOrderId);

        Task BatchChangeOrderDetailsQuantityAsync(List<OrderDetails> orderDetails);
    }
}
