using GameStore.DomainModels.Models;
using GameStore.DomainModels.Models.CreateModels;
using GameStore.DomainModels.Models.EditModels;
using GameStore.BLL.PaymentMethods;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.BLL.Interfaces
{
    public interface IOrderService
    {
        public Task CreateOrderAsync(CreateOrderRequest orderToCreate);

        public Task<Order> GetOrderByIdAsync(Guid id, string cultureCode = null);

        public Task<List<Order>> GetAllOrdersAsync();

        public Task<bool> CancelExpiredOrdersAsync(int ordersAmount);

        public Task EditOrderAsync(EditOrderRequest orderToEdit);

        Task<Order> GetOrderByCustomerIdAsync(Guid customerId, string cultureCode = null, bool includedDeletedRows = false);

        Task AddGameInOrderAsync(string key, Guid customerId);

        Task ChangeProductQuantityAsync(Guid id, short count);

        public Task StartOrderProcessingAsync(Guid id, PaymentType type);

        public Task CancelOrderAsync(Guid id);

        public Task CloseOrderAsync(Guid id);

        Task SoftDeleteOrderAsync(Guid id);

        Task HardDeleteOrderAsync(Guid id);

        Task<List<Order>> GetBetweenDatesAsync(DateTime? minDate, DateTime? maxDate);

        Task<List<Order>> GetGameStoreOrdersBetweenDatesAsync(DateTime? minDate, DateTime? maxDate, bool includeDeletedRows = false);

        Task MergeGuestOrderAsync(Guid guestId, Guid userId);

        Task BatchChangeProductQuantityAsync(List<OrderDetails> orderDetails);
    }
}
