    using AutoMapper;
using GameStore.BLL.Exceptions;
using GameStore.BLL.Extensions;
using GameStore.BLL.Interfaces;
using GameStore.DomainModels.Models;
using GameStore.DomainModels.Models.CreateModels;
using GameStore.DomainModels.Models.EditModels;
using GameStore.BLL.PaymentMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameStore.DomainModels.Enums;
using GameStore.DomainModels.Exceptions;
using GameStore.DAL.Abstractions.Interfaces;

namespace GameStore.BLL.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDetailsRepository _orderDetailsRepository;
        private readonly IGoodsRepository _gameRepository;
        private readonly IMapper _mapper;

        public OrderService(
            IOrderRepository orderRepository,
            IOrderDetailsRepository orderDetailsRepository,
            IGoodsRepository gameRepository,
            IMapper mapper)
        {
            _orderRepository = orderRepository;
            _orderDetailsRepository = orderDetailsRepository;
            _gameRepository = gameRepository;
            _mapper = mapper;
        }

        public Task<List<Order>> GetGameStoreOrdersBetweenDatesAsync(DateTime? minDate, DateTime? maxDate, bool includeDeletedRows = false)
        {
            return _orderRepository.GetSqlOrdersBetweenDatesAsync(minDate, maxDate, includeDeletedRows);
        }

        public Task CreateOrderAsync(CreateOrderRequest orderToCreate)
        {
            return CreateNewOrderAsync(orderToCreate);
        }

        public async Task<Order> GetOrderByIdAsync(Guid id, string cultureCode = null)
        {
            Order foundedOrder = await _orderRepository.FindAsync(id.ToString(), cultureCode);
            if (foundedOrder is null)
            {
                throw new NotFoundException($"Order with that id wasn`t found: {id}");
            }

            return foundedOrder;
        }

        public Task<List<Order>> GetAllOrdersAsync()
        {
            return _orderRepository.GetAllAsync();
        }

        public async Task<Order> GetOrderByCustomerIdAsync(Guid customerId, string cultureCode = null, bool includedDeletedRows = false)
        {
            var foundedOrder = await _orderRepository.FindByCustomerIdAsync(customerId, cultureCode, includedDeletedRows);
            if (foundedOrder is null)
            {
                CreateOrderRequest orderToCreate = new CreateOrderRequest()
                {
                    CustomerId = customerId
                };
                foundedOrder = await CreateNewOrderAsync(orderToCreate);
            }

            return foundedOrder;
        }

        public async Task EditOrderAsync(EditOrderRequest orderToEdit)
        {
            Order editOrder = _mapper.Map<Order>(orderToEdit);

            var oldOrderDetails = await _orderDetailsRepository.FindOrdeDetailsByOrderId(editOrder.Id);

            if (IsProcessedOrShipped(orderToEdit.OldStatus) && IsOpenOrCancelled(editOrder.Status))
            {
                await CancelGamesBookingAsync(oldOrderDetails);
            }
            else if (IsOpenOrCancelled(orderToEdit.OldStatus) && IsProcessedOrShippedOrCompleted(editOrder.Status))
            {
                await BookGamesAsync(editOrder.OrderDetails);
            }
            else if (IsProcessedOrShipped(orderToEdit.OldStatus) && IsProcessedOrShippedOrCompleted(editOrder.Status))
            {
                await CancelGamesBookingAsync(oldOrderDetails);
                await BookGamesAsync(editOrder.OrderDetails);
            }
            else if (!(IsOpenOrCancelled(orderToEdit.OldStatus) && IsOpenOrCancelled(editOrder.Status)))
            {
                throw new InvalidOperationException("Unsupported order status change");
            }

            await BatchChangeProductQuantityAsync(editOrder.OrderDetails);
            await _orderRepository.UpdateAsync(editOrder);
        }

        private bool IsProcessedOrShippedOrCompleted(OrderStatus status)
        {
            return IsProcessedOrShipped(status) || status == OrderStatus.Completed;
        }

        private bool IsProcessedOrShipped(OrderStatus status)
        {
            return status == OrderStatus.Processed || status == OrderStatus.Shipped;
        }

        private bool IsOpenOrCancelled(OrderStatus status)
        {
            return status == OrderStatus.Open || status == OrderStatus.Canceled;
        }

        public async Task MergeGuestOrderAsync(Guid guestId, Guid userId)
        {
            Order guestOrder = await _orderRepository.FindByCustomerIdAsync(guestId);
            if (guestOrder is null)
            {
                return;
            }

            Order userOrder = await _orderRepository.FindByCustomerIdAsync(userId);
            if (userOrder is null)
            {
                await _orderRepository.ChangeCustomer(Guid.Parse(guestOrder.Id), userId);

                return;
            }

            await _orderDetailsRepository.ChangeOrder(guestOrder.OrderDetails, Guid.Parse(userOrder.Id));

            List<OrderDetails> excessDetails = userOrder.OrderDetails
                .Where(uod => guestOrder.OrderDetails.Any(god => uod.ProductId == god.ProductId)).ToList();

            await _orderDetailsRepository.SoftDeleteRangeAsync(excessDetails);
            await _orderRepository.SoftDeleteAsync(Guid.Parse(guestOrder.Id));
        }

        public async Task HardDeleteOrderAsync(Guid id)
        {
            try
            {
                await _orderRepository.DeleteAsync(id);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException($"order with id {id} wasn't found");
            }
        }

        public async Task SoftDeleteOrderAsync(Guid id)
        {
            try
            {
                await _orderRepository.SoftDeleteAsync(id);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException($"order with id {id} wasn't found");
            }
        }

        public async Task AddGameInOrderAsync(string key, Guid customerId)
        {
            Goods foundGame = await _gameRepository.FindByKeyAsync(key);
            if (foundGame is null)
            {
                throw new InvalidOperationException("No able to buy this game");
            }

            if (foundGame.UnitsInStock < 1)
            {
                throw new NotEnoughUnitsException("Not enough keys on stock to put this game in busket");
            }

            Order foundOrder = await _orderRepository.FindByCustomerIdAsync(customerId);
            if (foundOrder is null)
            {
                CreateOrderRequest createOrderRequest = new CreateOrderRequest { CustomerId = customerId };
                foundOrder = await CreateNewOrderAsync(createOrderRequest);
            }

            OrderDetails orderDetails = foundOrder.OrderDetails.FirstOrDefault(x => x.Product.Id == foundGame.Id);
            if (orderDetails != null)
            {
                await ChangeProductQuantityAsync(orderDetails.Id, 1);
            }
            else
            {
                OrderDetails newOrderDeatils = new OrderDetails()
                {
                    OrderId = foundOrder.Id,
                    Quantity = 1,
                    ProductId = foundGame.Id,
                    Price = foundGame.Price
                };

                await _orderDetailsRepository.CreateAsync(newOrderDeatils);
            }
        }

        public Task<List<Order>> GetBetweenDatesAsync(DateTime? minDate, DateTime? maxDate)
        {
            if (!minDate.HasValue)
            {
                return _orderRepository.GetOrdersBeforeDateAsync(maxDate.Value);
            }

            if (!maxDate.HasValue)
            {
                return _orderRepository.GetOrdersAfterDateAsync(minDate.Value);
            }

            return _orderRepository.GetBetweenDatesAsync(minDate.Value, maxDate.Value);
        }

        public async Task ChangeProductQuantityAsync(Guid id, short count)
        {
            OrderDetails editOrderDetails = await _orderDetailsRepository.FindByIdAsync(id);
            if (editOrderDetails is null)
            {
                throw new NotFoundException($"Order details with that id wasn`t found: {id}");
            }

            if (editOrderDetails.Quantity + count >= 0)
            {
                editOrderDetails.Quantity += count;
            }
            else
            {
                editOrderDetails.Quantity = 0;
            }

            Goods foundGame = await _gameRepository.FindAsync(editOrderDetails.ProductId);

            if (foundGame.UnitsInStock < editOrderDetails.Quantity)
            {
                throw new NotEnoughUnitsException("Not enough keys on stock to put this game in busket");
            }

            await _orderDetailsRepository.ChangeProductQuantity(id, editOrderDetails.Quantity);
        }

        public async Task StartOrderProcessingAsync(Guid id, PaymentType type)
        {
            Order foundOrder = await _orderRepository.FindAsync(id.ToString());
            if (foundOrder is null)
            {
                throw new NotFoundException($"Order with id {id} wasn't found");
            }
            
            await BookGamesAsync(foundOrder.OrderDetails);
            await SetOrderInProcessAsync(type, id);
        }

        public async Task CancelOrderAsync(Guid id)
        {
            Order foundOrder = await _orderRepository.FindAsync(id.ToString());
            if (foundOrder is null)
            {
                throw new NotFoundException($"Order with id {id} wasn't found");
            }

            await CancelOrderAsync(foundOrder);
        }

        public async Task CloseOrderAsync(Guid id)
        {
            Order foundOrder = await _orderRepository.FindAsync(id.ToString());
            if (foundOrder is null)
            {
                throw new NotFoundException($"Order with id {id} wasn't found");
            }

            await _orderRepository.ChangeOrderStatusAsync(OrderStatus.Completed, id);
        }

        public async Task<bool> CancelExpiredOrdersAsync(int ordersAmount)
        {
            List<Order> orders = await _orderRepository.GetExpiredOrdersAsync(ordersAmount);
            if (orders.Count == 0)
            {
                return false;
            }

            foreach (Order order in orders)
            {
                await CancelOrderAsync(order);
            }

            return true;
        }

        private async Task BookGamesAsync(List<OrderDetails> orderDetails)
        {
            List<string> gameIds = orderDetails.Select(x => x.Product.Id).ToList();
            List<Goods> games = await _gameRepository.FindByIdsAsync(gameIds, asNoTracking: true);

            foreach (var orderDetail in orderDetails)
            {
                games.First(game => game.Id == orderDetail.Product.Id)
                    .UnitsInStock -= orderDetail.Quantity;
            }

            await _gameRepository.BatchUpdateUnitsInStockAsync(games);
        }

        private Task SetOrderInProcessAsync(PaymentType type, Guid id)
        {
            DateTime expirationDateUtc = DateTime.UtcNow.CalculateOrderPaymentExpirationDate(type);

            return _orderRepository.ProcessOrderAsync(expirationDateUtc, id);
        }

        private async Task<Order> CreateNewOrderAsync(CreateOrderRequest orderToCreate)
        {
            Order newOrder = BuildOrder(orderToCreate);

            try
            {
                await _orderRepository.CreateAsync(newOrder);
            }
            catch (Exception ex)
            {
                throw;
            }

            return newOrder;
        }

        private Order BuildOrder(CreateOrderRequest newOrder)
        {
            Order Order = _mapper.Map<Order>(newOrder);

            Order.Id = Guid.NewGuid().ToString();
            Order.OrderDate = DateTime.UtcNow;

            return Order;
        }

        private async Task CancelGamesBookingAsync(List<OrderDetails> orderDetails)
        {
            List<string> gameIds = orderDetails.Select(x => x.Product.Id).ToList();
            List<Goods> games = await _gameRepository.FindByIdsAsync(gameIds);

            foreach (var orderDetail in orderDetails)
            {
                games.First(game => game.Id == orderDetail.Product.Id)
                    .UnitsInStock += orderDetail.Quantity;
            }

            await _gameRepository.BatchUpdateUnitsInStockAsync(games);
        }

        private async Task CancelOrderAsync(Order order)
        {
            await CancelGamesBookingAsync(order.OrderDetails);

            await _orderRepository.ChangeOrderStatusAsync(OrderStatus.Canceled, Guid.Parse(order.Id));
        }

        public async Task BatchChangeProductQuantityAsync(List<OrderDetails> orderDetails)
        {
            if (!orderDetails.Any())
            {
                return;
            }

            foreach (var od in orderDetails)
            {
                Goods foundGame = await _gameRepository.FindAsync(od.ProductId, asNoTracking: true);
                if (foundGame is null)
                {
                    throw new NotFoundException("Game with that Id was not found");
                }

                if (foundGame.UnitsInStock < od.Quantity)
                {
                    throw new NotEnoughUnitsException("Not enough keys on stock to put this game in busket");
                }
            }

            await _orderDetailsRepository.BatchChangeOrderDetailsQuantityAsync(orderDetails);
        }
    }
}
