using AutoMapper;
using FluentAssertions;
using GameStore.BLL.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using GameStore.DAL.MappingProfiles;
using GameStore.DAL.Abstractions.Interfaces;
using GameStore.BLL.Interfaces;
using GameStore.DomainModels.Models.CreateModels;
using GameStore.DomainModels.Models;
using GameStore.DomainModels.Exceptions;
using GameStore.DomainModels.Models.EditModels;
using GameStore.BLL.MappingProfiles;
using GameStore.PL.MappingProfiles;
using GameStore.DomainModels.Enums;
using System.Linq;

namespace GameStore.Tests.GameStoreBLL.Services
{
    public class OrderServiceTest
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IOrderDetailsRepository> _orderDetailsRepositoryMock;
        private readonly Mock<IGoodsRepository> _gameRepositoryMock;
        private readonly IOrderService _service;
        private readonly IMapper _mapper;

        public OrderServiceTest()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _orderDetailsRepositoryMock = new Mock<IOrderDetailsRepository>();
            _gameRepositoryMock = new Mock<IGoodsRepository>();

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<EntityDomainMapperProfile>();
                cfg.AddProfile<BusinessMappingProfile>();
                cfg.AddProfile<PresentationLayerMapperProfile>();
            });
            _mapper = new Mapper(configuration);

            _service = new OrderService(
                _orderRepositoryMock.Object,
                _orderDetailsRepositoryMock.Object,
                _gameRepositoryMock.Object,
                _mapper);
        }

        [Fact]
        public async Task SaveOrderAsync_CalledCreateAsyncRepositoryMethod()
        {
            await _service.CreateOrderAsync(new CreateOrderRequest());

            _orderRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Order>()));
        }

        [Theory]
        [InlineData(OrderStatus.Processed, OrderStatus.Open)]
        [InlineData(OrderStatus.Shipped, OrderStatus.Open)]
        [InlineData(OrderStatus.Processed, OrderStatus.Canceled)]
        [InlineData(OrderStatus.Shipped, OrderStatus.Canceled)]
        public async Task EditOrderAsync_FromProcessedOrShippedToOpenOrCancelled_CanceledGamesBooking(OrderStatus oldStatus,
                                                                                                      OrderStatus newStatus)
        {
            Guid id = Guid.NewGuid();
            short quantity = 5;
            short unitsInStock = 5;

            Goods product = new Goods
            {
                Id = id.ToString(),
                UnitsInStock = unitsInStock
            };

            List<OrderDetails> orderDetails = new List<OrderDetails>()
            {
                new OrderDetails()
                {
                    Quantity = quantity,
                    Product = product,
                    ProductId = id.ToString()
                }
            };

            EditOrderRequest request = new EditOrderRequest()
            {
                Id = id,
                Status = newStatus,
                OldStatus = oldStatus,
                OrderDetails = orderDetails
            };

            List<Goods> goods = new List<Goods>()
            {
                product
            };

            _orderDetailsRepositoryMock.Setup(x => x.FindOrdeDetailsByOrderId(id.ToString(), It.IsAny<bool>()))
                .ReturnsAsync(orderDetails);

            _gameRepositoryMock.Setup(x => x.FindByIdsAsync(It.IsAny<List<string>>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(goods);

            _gameRepositoryMock.Setup(x => x.FindAsync(id.ToString(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(product);

            await _service.EditOrderAsync(request);

            goods.First().UnitsInStock.Should().Be((short)(quantity+unitsInStock));

            _gameRepositoryMock.Verify(x => x.BatchUpdateUnitsInStockAsync(It.IsAny<List<Goods>>()));
            _orderDetailsRepositoryMock.Verify(x => x.BatchChangeOrderDetailsQuantityAsync(It.IsAny<List<OrderDetails>>()));
            _orderRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Order>()));
        }

        [Theory]
        [InlineData(OrderStatus.Processed, OrderStatus.Processed)]
        [InlineData(OrderStatus.Shipped, OrderStatus.Processed)]
        [InlineData(OrderStatus.Processed, OrderStatus.Shipped)]
        [InlineData(OrderStatus.Shipped, OrderStatus.Shipped)]
        [InlineData(OrderStatus.Processed, OrderStatus.Completed)]
        [InlineData(OrderStatus.Shipped, OrderStatus.Completed)]
        public async Task EditOrderAsync_FromProcessedOrShippedToProcessedOrShippedOrCompleted_BookGames(OrderStatus oldStatus,
                                                                                                         OrderStatus newStatus)
        {
            Guid id = Guid.NewGuid();
            short oldQuantity = 4;
            short quantity = 5;
            short unitsInStock = 5;

            Goods product = new Goods
            {
                Id = id.ToString(),
                UnitsInStock = unitsInStock
            };

            Goods productCurrentlyInDB = new Goods
            {
                Id = id.ToString(),
                UnitsInStock = unitsInStock
            };

            List<OrderDetails> orderDetails = new List<OrderDetails>()
            {
                new OrderDetails()
                {
                    Quantity = quantity,
                    Product = product,
                    ProductId = id.ToString()
                }
            };

            List<OrderDetails> orderDetailsCurrentlyInDB = new List<OrderDetails>()
            {
                new OrderDetails()
                {
                    Quantity = oldQuantity,
                    Product = product,
                    ProductId = id.ToString()
                }
            };

            EditOrderRequest request = new EditOrderRequest()
            {
                Id = id,
                Status = newStatus,
                OldStatus = oldStatus,
                OrderDetails = orderDetails
            };

            List<Goods> goods = new List<Goods>()
            {
                product
            };

            _orderDetailsRepositoryMock.Setup(x => x.FindOrdeDetailsByOrderId(id.ToString(), It.IsAny<bool>()))
                .ReturnsAsync(orderDetailsCurrentlyInDB);

            _gameRepositoryMock.Setup(x => x.FindByIdsAsync(It.IsAny<List<string>>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(goods);

            _gameRepositoryMock.Setup(x => x.FindAsync(id.ToString(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(productCurrentlyInDB);

            await _service.EditOrderAsync(request);

            goods.First().UnitsInStock.Should().Be((short)(unitsInStock + oldQuantity - quantity));

            _gameRepositoryMock.Verify(x => x.BatchUpdateUnitsInStockAsync(It.IsAny<List<Goods>>()));

            _orderDetailsRepositoryMock.Verify(x => x.BatchChangeOrderDetailsQuantityAsync(It.IsAny<List<OrderDetails>>()));
            _orderRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Order>()));
        }

        [Fact]
        public void EditOrderAsync_UnsupportedCombinationOfStatuses_InvalidOperationException()
        {
            _service.Invoking(x => x.EditOrderAsync(new EditOrderRequest()))
                .Should().ThrowAsync<InvalidOperationException>();
        }

        [Theory]
        [InlineData(OrderStatus.Open, OrderStatus.Processed)]
        [InlineData(OrderStatus.Canceled, OrderStatus.Processed)]
        [InlineData(OrderStatus.Open, OrderStatus.Shipped)]
        [InlineData(OrderStatus.Canceled, OrderStatus.Shipped)]
        [InlineData(OrderStatus.Open, OrderStatus.Completed)]
        [InlineData(OrderStatus.Canceled, OrderStatus.Completed)]
        public async Task EditOrderAsync_FromOpenOrCancelledToProcessedOrShippedOrCompleted_BookGames(OrderStatus oldStatus,
                                                                                                      OrderStatus newStatus)
        {
            Guid id = Guid.NewGuid();
            short quantity = 5;
            short unitsInStock = 5;

            Goods product = new Goods
            {
                Id = id.ToString(),
                UnitsInStock = unitsInStock
            };

            Goods productCurrentlyInDB = new Goods
            {
                Id = id.ToString(),
                UnitsInStock = unitsInStock
            };

            List<OrderDetails> orderDetails = new List<OrderDetails>()
            {
                new OrderDetails()
                {
                    Quantity = quantity,
                    Product = product,
                    ProductId = id.ToString()
                }
            };

            EditOrderRequest request = new EditOrderRequest()
            {
                Id = id,
                Status = newStatus,
                OldStatus = oldStatus,
                OrderDetails = orderDetails
            };

            List<Goods> goods = new List<Goods>()
            {
                product
            }; 

            _gameRepositoryMock.Setup(x => x.FindByIdsAsync(It.IsAny<List<string>>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(goods);

            _gameRepositoryMock.Setup(x => x.FindAsync(id.ToString(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(productCurrentlyInDB);

            await _service.EditOrderAsync(request);

            goods.First().UnitsInStock.Should().Be((short)(unitsInStock - quantity));

            _gameRepositoryMock.Verify(x => x.BatchUpdateUnitsInStockAsync(It.IsAny<List<Goods>>()));

            _orderDetailsRepositoryMock.Verify(x => x.BatchChangeOrderDetailsQuantityAsync(It.IsAny<List<OrderDetails>>()));
            _orderRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Order>()));
        }

        [Fact]
        public async Task FindOrderAsync_EnteredCorrectId_ReturnOrderWithEnteredId()
        {
            Guid guid = Guid.NewGuid();
            _orderRepositoryMock.Setup(x => x.FindAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(new Order() { Id = guid.ToString() });

            var actual = await _service.GetOrderByIdAsync(Guid.Empty);

            actual.Should().NotBeNull()
                .And.BeOfType<Order>()
                .Which.Id.Should().Be(guid.ToString());
        }

        [Fact]
        public async Task AddGameIntoCustomerOrderAsync_OrderDetailsIsNotExistYet_CalledCreateRepositoryMethod()
        {
            Goods game = new Goods()
            {
                Id = Guid.NewGuid().ToString(),
                UnitsInStock = 1
            };
            Order order = new Order()
            {
                Id = Guid.NewGuid().ToString(),
                OrderDetails = new List<OrderDetails>()
            };

            _gameRepositoryMock.Setup(x => x.FindByKeyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(game);
            _orderRepositoryMock.Setup(x => x.FindByCustomerIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(order);

            await _service.AddGameInOrderAsync("", Guid.NewGuid());

            _orderDetailsRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<OrderDetails>()));
        }

        [Fact]
        public async Task AddGameIntoCustomerOrderAsync_OrderDetailsIsAlreadyExist_CalledUpdateRepositoryMethod()
        {
            Goods game = new Goods() { Id = Guid.NewGuid().ToString(), UnitsInStock = 2 };
            OrderDetails orderDetails = new OrderDetails() { Product = game, ProductId = game.Id, Quantity = 1 };
            Order order = new Order()
            {
                Id = Guid.NewGuid().ToString(),
                OrderDetails = new List<OrderDetails>() { orderDetails }
            };

            _gameRepositoryMock.Setup(x => x.FindByKeyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(game);
            _gameRepositoryMock.Setup(x => x.FindAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(game);
            _orderRepositoryMock.Setup(x => x.FindByCustomerIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(order);
            _orderDetailsRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
                .ReturnsAsync(orderDetails);

            await _service.AddGameInOrderAsync("", Guid.NewGuid());

            _orderDetailsRepositoryMock.Verify(x => x.ChangeProductQuantity(It.IsAny<Guid>(), It.IsAny<int>()));
        }

        [Fact]
        public void FindOrderAsync_EnteredNotExistedId_ThrowsNotFoundException()
        {
            _service.Invoking(x => x.GetOrderByIdAsync(Guid.Empty))
                .Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task FindByCustomerIdAsync_EnteredCorrectId_ReturnsOkResult()
        {
            Guid guid = Guid.NewGuid();
            _orderRepositoryMock.Setup(x => x.FindByCustomerIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(new Order() { CustomerId = guid });

            var actual = await _service.GetOrderByCustomerIdAsync(Guid.Empty);

            actual.Should().NotBeNull()
                .And.BeOfType<Order>()
                .Which.CustomerId.Should().Be(guid);
        }

        [Fact]
        public void FindByCustomerIdAsync_EnteredInvalidId_ThrowsNotFoundException()
        {
            _service.Invoking(x => x.GetOrderByCustomerIdAsync(Guid.Empty))
                .Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task GetAsync_ReturnedOrderList()
        {
            _orderRepositoryMock.Setup(x => x.GetAllAsync(It.IsAny<bool>()))
                .ReturnsAsync(new List<Order>() { new Order() });

            var actual = await _service.GetAllOrdersAsync();

            actual.Should().NotBeNull()
                .And.NotContainNulls()
                .And.BeOfType<List<Order>>();
        }

        [Fact]
        public async Task SoftDeleteOrderAsync_EnteredCorrectId_CalledUpdateAsync()
        {
            _orderRepositoryMock.Setup(x => x.FindAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(new Order());

            await _service.SoftDeleteOrderAsync(Guid.Empty);

            _orderRepositoryMock.Verify(x => x.SoftDeleteAsync(It.IsAny<Guid>()));
        }

        [Fact]
        public void SoftDeleteOrderAsync_EnteredInvalidId_ThrowsNotFoundException()
        {
            _service.Invoking(x => x.SoftDeleteOrderAsync(Guid.Empty))
                .Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task HardDeleteOrderAsync_EnteredCorrectId_CalledDeleteAsync()
        {
            _orderRepositoryMock.Setup(x => x.FindAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(new Order());

            await _service.HardDeleteOrderAsync(Guid.Empty);

            _orderRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<Guid>()));
        }

        [Fact]
        public void HardDeleteOrderAsync_EnteredInvalidId_ThrowsNotFoundException()
        {
            _service.Invoking(x => x.HardDeleteOrderAsync(Guid.Empty))
                .Should().ThrowAsync<NotFoundException>();
        }

        [Theory]
        [InlineData(5)]
        [InlineData(-10)]
        public async Task ChangeProductQuantityAsync_EnteredCorrectId_CalledUpdateAsync(short count)
        {
            Guid id = Guid.NewGuid();
            _gameRepositoryMock.Setup(x => x.FindAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(new Goods() { Id = id.ToString(), UnitsInStock = 10 });
            _orderDetailsRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
                .ReturnsAsync(new OrderDetails() { Quantity = 0, ProductId = id.ToString() });

            await _service.ChangeProductQuantityAsync(Guid.Empty, count);

            _orderDetailsRepositoryMock.Verify(x => x.ChangeProductQuantity(It.IsAny<Guid>(), It.IsAny<int>()));
        }

        [Fact]
        public void ChangeProductQuantityAsync_EnteredInvalidId_ThrowsNotFoundException()
        {
            _service.Invoking(x => x.ChangeProductQuantityAsync(Guid.Empty, 0))
                .Should().ThrowAsync<NotFoundException>();
        }
    }
}
