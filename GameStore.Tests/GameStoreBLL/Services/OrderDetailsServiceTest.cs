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

namespace GameStore.Tests.GameStoreBLL.Services
{
    public class OrderDetailsServiceTest
    {
        private readonly Mock<IOrderDetailsRepository> _orderDetailsRepositoryMock;
        private readonly IOrderDetailsService _service;
        private readonly IMapper _mapper;

        public OrderDetailsServiceTest()
        {
            _orderDetailsRepositoryMock = new Mock<IOrderDetailsRepository>();

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<EntityDomainMapperProfile>();
                cfg.AddProfile<BusinessMappingProfile>();
                cfg.AddProfile<PresentationLayerMapperProfile>();
            });
            _mapper = new Mapper(configuration);

            _service = new OrderDetailsService(_orderDetailsRepositoryMock.Object, _mapper);
        }

        [Fact]
        public async Task CreateOrderDetailsAsync_CalledCreateAsyncRepositoryMethod()
        {
            await _service.CreateOrderDetailsAsync(new CreateOrderDetailsRequest());

            _orderDetailsRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<OrderDetails>()));
        }

        [Fact]
        public void FindOrderDetailsAsync_EnteredInvalidId_ThrowNotFoundException()
        {
            _service.Invoking(x => x.GetOrderDetailsByIdAsync(Guid.Empty))
                .Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task GetAsync_ReturnedOrderDetailsList()
        {
            _orderDetailsRepositoryMock.Setup(x => x.GetAllAsync(It.IsAny<bool>()))
                .ReturnsAsync(new List<OrderDetails>() { new OrderDetails() });

            var actual = await _service.GetAllAsync();

            actual.Should().NotBeNull()
                .And.BeOfType<List<OrderDetails>>();
        }

        [Fact]
        public async Task EditOrderDetailsAsync_CalledEditAsyncRepositoryMethod()
        {
            await _service.EditOrderDetailsAsync(new EditOrderDetailsRequest());

            _orderDetailsRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<OrderDetails>()));
        }

        [Fact]
        public async Task SoftDeleteOrderDetailsAsync_EnteredCorrectId_CalledUpdateAsync()
        {
            _orderDetailsRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
                .ReturnsAsync(new OrderDetails());

            await _service.SoftDeleteOrderDetailsAsync(Guid.Empty);

            _orderDetailsRepositoryMock.Verify(x => x.SoftDeleteAsync(It.IsAny<Guid>()));
        }

        [Fact]
        public void SoftDeleteOrderDetailsAsync_EnteredInvalidId_ThrowNotFoundException()
        {
            _service.Invoking(x => x.SoftDeleteOrderDetailsAsync(Guid.Empty))
                .Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task HardDeleteOrderDetailsAsync_EnteredCorrectId_CalledDeleteAsync()
        {
            _orderDetailsRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
                .ReturnsAsync(new OrderDetails());

            await _service.HardDeleteOrderDetailsAsync(Guid.Empty);

            _orderDetailsRepositoryMock.Verify(x => x.DeleteByIdAsync(It.IsAny<Guid>()));
        }

        [Fact]
        public void HardDeleteOrderDetailsAsync_EnteredInvalidId_ThrowNotFoundException()
        {
            _service.Invoking(x => x.HardDeleteOrderDetailsAsync(Guid.Empty))
                .Should().ThrowAsync<NotFoundException>();
        }
    }
}
