using AutoMapper;
using FluentAssertions;
using GameStore.BLL.Interfaces;
using GameStore.DAL.MappingProfiles;
using GameStore.DomainModels.Models;
using GameStore.PL.Configurations;
using GameStore.PL.Controllers;
using GameStore.PL.DTOs;
using GameStore.PL.MappingProfiles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace GameStore.Tests.GameStorePL.Controllers
{
    public class BasketControllerTest
    {
        private readonly Mock<IOrderService> _orderServiceMock;
        private readonly Mock<GuestCookieSettings> _configMock;
        private readonly IMapper _mapper;
        private readonly BasketController _controller;
        private readonly Mock<ILogger<BasketController>> _logger;

        public BasketControllerTest()
        {
            _orderServiceMock = new Mock<IOrderService>();
            _configMock = new Mock<GuestCookieSettings>();
            _logger = new Mock<ILogger<BasketController>>();

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<PresentationLayerMapperProfile>();
                cfg.AddProfile<EntityDomainMapperProfile>();
            });
            _mapper = new Mapper(configuration);

            _controller = new BasketController
            (
                _orderServiceMock.Object,
                _mapper,
                _configMock.Object,
                _logger.Object
            );
        }

        [Fact]
        public async Task CreateAsync_WithExistedKeyAndUserId_RedirectedToAddGaneIntoBasketWithGameKey()
        {
            string key = "sekiro";
            Guid guid = Guid.NewGuid();

            var result = await _controller.CreateAsync(key, guid);

            var redirectResult = result.Should().NotBeNull()
                .And.BeOfType<RedirectToActionResult>().Subject;

            redirectResult.ActionName.Should().Be("AddGameIntoBasket");

            redirectResult.RouteValues.Should().NotBeNull()
                .And.Contain("key", key);
        }

        [Fact]
        public async Task DetailsByIdAsync_WithExistId_ReturnsDetailsViewWithOrderDTO()
        {
            Order order = new Order { Id = Guid.NewGuid().ToString(), CustomerId = Guid.NewGuid() };
            _orderServiceMock.Setup(x => x.GetOrderByCustomerIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(order);

            var result = await _controller.DetailsAsync(Guid.Empty);

            var viewResult = result.Should().NotBeNull()
                .And.BeOfType<ViewResult>().Subject;

            viewResult.ViewName.Should().Be("Details");

            viewResult.Model.Should().NotBeNull()
                .And.BeOfType<OrderDTO>()
                .Which.Id.Should().Be(order.Id);
        }
    }
}
