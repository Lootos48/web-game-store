using AutoMapper;
using GameStore.BLL.Interfaces;
using GameStore.DomainModels.Models;
using GameStore.PL.DTOs;
using GameStore.PL.DTOs.EditDTOs;
using GameStore.PL.Factories.Interfaces;
using GameStore.PL.ViewContexts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.PL.Factories
{
    public class OrderContextFactory : IOrderContextFactory
    {
        private readonly IOrderService _orderService;
        private readonly IShipperService _shipperService;

        private readonly IMapper _mapper;

        public OrderContextFactory(
            IOrderService orderService,
            IShipperService shipperService,
            IMapper mapper)
        {
            _shipperService = shipperService;
            _orderService = orderService;
            _mapper = mapper;
        }

        public async Task<OrderIBoxPaymentViewContext> BuildOrderIBoxPaymentViewContext(Guid id)
        {
            Order order = await _orderService.GetOrderByIdAsync(id);

            OrderIBoxPaymentViewContext context = new OrderIBoxPaymentViewContext()
            {
                Order = _mapper.Map<OrderDTO>(order),
                TotalPrice = order.TotalPrice
            };

            return context;
        }

        public async Task<OrdersEditingViewContext> BuildOrdersEditingViewContext(EditOrderRequestDTO orderToEditDTO)
        {
            var shippers = await _shipperService.GetAllShippersAsync();

            orderToEditDTO.OldStatus = orderToEditDTO.Status;

            var context = new OrdersEditingViewContext
            {
                OrderToEdit = orderToEditDTO,
                Shippers = _mapper.Map<List<ShipperDTO>>(shippers)
            };

            return context;
        }
    }
}
