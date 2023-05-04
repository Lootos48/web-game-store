using GameStore.PL.DTOs.EditDTOs;
using GameStore.PL.ViewContexts;
using System;
using System.Threading.Tasks;

namespace GameStore.PL.Factories.Interfaces
{
    public interface IOrderContextFactory
    {
        Task<OrderIBoxPaymentViewContext> BuildOrderIBoxPaymentViewContext(Guid id);
        Task<OrdersEditingViewContext> BuildOrdersEditingViewContext(EditOrderRequestDTO orderToEditDTO);
    }
}
