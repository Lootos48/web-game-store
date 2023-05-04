using GameStore.DomainModels.Models;
using GameStore.DomainModels.Models.CreateModels;
using GameStore.DomainModels.Models.EditModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.BLL.Interfaces
{
    public interface IOrderDetailsService
    {
        public Task<List<OrderDetails>> GetAllAsync();
        Task<OrderDetails> GetOrderDetailsByIdAsync(Guid id);
        public Task CreateOrderDetailsAsync(CreateOrderDetailsRequest orderDetailsToCreate);
        public Task EditOrderDetailsAsync(EditOrderDetailsRequest orderDetailsToEdit);
        Task SoftDeleteOrderDetailsAsync(Guid id);
        Task HardDeleteOrderDetailsAsync(Guid id);
    }
}
