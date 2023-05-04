using AutoMapper;
using GameStore.BLL.Interfaces;
using GameStore.DomainModels.Models;
using GameStore.DomainModels.Models.CreateModels;
using GameStore.DomainModels.Models.EditModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStore.DomainModels.Exceptions;
using GameStore.DAL.Abstractions.Interfaces;

namespace GameStore.BLL.Services
{
    public class OrderDetailsService : IOrderDetailsService
    {
        private readonly IOrderDetailsRepository _orderDetailsRepository;
        private readonly IMapper _mapper;

        public OrderDetailsService(
            IOrderDetailsRepository orderDetailsRepository, 
            IMapper mapper)
        {
            _orderDetailsRepository = orderDetailsRepository;
            _mapper = mapper;
        }

        public Task CreateOrderDetailsAsync(CreateOrderDetailsRequest orderDetailsToCreate)
        {
            OrderDetails createOrderDetails = _mapper.Map<OrderDetails>(orderDetailsToCreate);

            return _orderDetailsRepository.CreateAsync(createOrderDetails);
        }

        public async Task<OrderDetails> GetOrderDetailsByIdAsync(Guid id)
        {
            OrderDetails foundedOrderDetails = await _orderDetailsRepository.FindByIdAsync(id);
            if (foundedOrderDetails is null)
            {
                throw new NotFoundException($"Order details with that id wasn`t found: {id}");
            }

            return foundedOrderDetails;
        }

        public Task<List<OrderDetails>> GetAllAsync()
        {
            return _orderDetailsRepository.GetAllAsync();
        }

        public Task EditOrderDetailsAsync(EditOrderDetailsRequest orderDetailsToEdit)
        {
            OrderDetails editOrderDetails = _mapper.Map<OrderDetails>(orderDetailsToEdit);

            return _orderDetailsRepository.UpdateAsync(editOrderDetails);
        }

        public async Task HardDeleteOrderDetailsAsync(Guid id)
        {
            try
            {
                await _orderDetailsRepository.DeleteByIdAsync(id);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException($"OrderDetails with id {id} wasn't found");
            }
        }

        public async Task SoftDeleteOrderDetailsAsync(Guid id)
        {
            try
            {
                await _orderDetailsRepository.SoftDeleteAsync(id);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException($"OrderDetails with id {id} wasn't found");
            }
        }
    }
}
