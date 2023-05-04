using AutoMapper;
using FluentAssertions;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Workflows.Interfaces;
using GameStore.DomainModels.Enums;
using GameStore.DomainModels.Models;
using GameStore.DomainModels.Models.EditModels;
using GameStore.PL.Controllers;
using GameStore.PL.DTOs;
using GameStore.PL.DTOs.EditDTOs;
using GameStore.PL.Factories.Interfaces;
using GameStore.PL.MappingProfiles;
using GameStore.PL.ViewContexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace GameStore.Tests.GameStorePL.Controllers
{
    public class OrderControllerTest
    {
        private readonly Mock<IOrderService> _orderServiceMock;
        private readonly Mock<IPaymentService> _paymentServiceMock;
        private readonly Mock<IInvoiceGenerationWorkflow> _invoiceGeneretionWorkflow;
        private readonly Mock<IOrderContextFactory> _orderContextFactory;
        private readonly IMapper _mapper;
        private readonly OrderController _controller;

        public OrderControllerTest()
        {
            _orderServiceMock = new Mock<IOrderService>();
            _paymentServiceMock = new Mock<IPaymentService>();
            _invoiceGeneretionWorkflow = new Mock<IInvoiceGenerationWorkflow>();
            _orderContextFactory = new Mock<IOrderContextFactory>();

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<PresentationLayerMapperProfile>();
            });
            _mapper = new Mapper(configuration);

            _controller = new OrderController(
                _orderServiceMock.Object,
                _paymentServiceMock.Object,
                _orderContextFactory.Object,
                _invoiceGeneretionWorkflow.Object,
                _mapper);


            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.Empty.ToString()),
                new Claim(ClaimTypes.Name, "test"),
                new Claim(ClaimTypes.Email, "test@mail.com"),
                new Claim(ClaimTypes.Role, UserRoles.Administrator.ToString())
            };

            var identity = new ClaimsIdentity(claims, "Test");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var controllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };

            _controller.ControllerContext = controllerContext;
        }

        [Fact]
        public async Task IndexAsync_ReturnsOrderList()
        {
            Guid guid = Guid.NewGuid();
            List<Order> orders = new List<Order>() { new Order { Id = guid.ToString() } };
            _orderServiceMock.Setup(x => x.GetAllOrdersAsync()).ReturnsAsync(orders);

            var context = new OrdersFiltrationByDatesViewContext()
            {
                Orders = _mapper.Map<List<OrderDTO>>(orders)
            };

            var result = await _controller.IndexAsync(context);

            var viewResult = result.Should().NotBeNull()
                .And.BeOfType<ViewResult>().Subject;

            viewResult.ViewName.Should().Be("Index");

            viewResult.Model.Should().NotBeNull()
                .And.BeOfType<OrdersFiltrationByDatesViewContext>();
        }

        [Fact]
        public async Task DetailsAsync_EnteredExistsId_DetailsViewWithOrderDTO()
        {
            Guid guid = Guid.NewGuid();
            Order order = new Order { Id = guid.ToString() };

            _orderServiceMock.Setup(x => x.GetOrderByIdAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(order);

            var result = await _controller.DetailsAsync(Guid.Empty);

            var viewResult = result.Should().NotBeNull()
                .And.BeOfType<ViewResult>().Subject;

            viewResult.ViewName.Should().Be("Details");

            viewResult.Model.Should().NotBeNull()
                .And.BeOfType<OrderDTO>().Which.Id.Should().Be(guid.ToString());
        }

        [Fact]
        public async Task GetEditingViewAsync_EnteredExistsId_EditViewWithOrderEditRequestDTO()
        {
            Guid guid = Guid.NewGuid();
            Order order = new Order { Id = guid.ToString() };
            _orderServiceMock.Setup(x => x.GetOrderByIdAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(order);
            _orderContextFactory.Setup(x => x.BuildOrdersEditingViewContext(It.IsAny<EditOrderRequestDTO>()))
                .ReturnsAsync(new OrdersEditingViewContext() { OrderToEdit = new EditOrderRequestDTO() { Id = guid } });

            var result = await _controller.GetEditingViewAsync(Guid.Empty.ToString());

            var viewResult = result.Should().NotBeNull()
                .And.BeOfType<ViewResult>().Subject;

            viewResult.ViewName.Should().Be("Edit");

            viewResult.Model.Should().NotBeNull()
                .And.BeOfType<OrdersEditingViewContext>()
                .Which.OrderToEdit.Id.Should().Be(guid);
        }

        [Fact]
        public async Task EditAsync_RedirectToActionIndexResult()
        {
            _orderServiceMock.Setup(x => x.EditOrderAsync(It.IsAny<EditOrderRequest>()));

            var context = new OrdersEditingViewContext
            {
                OrderToEdit = new EditOrderRequestDTO()
            };

            var result = await _controller.EditAsync(context);

            var action = result.Should().NotBeNull()
                .And.BeOfType<RedirectToActionResult>().Subject;

            action.ActionName.Should().Be("Details");
        }

        [Fact]
        public async Task GetDeletingViewAsync_EnteredExistsId_DeleteViewWithOrderDTO()
        {
            Guid guid = Guid.NewGuid();
            Order order = new Order { Id = guid.ToString() };
            _orderServiceMock.Setup(x => x.GetOrderByIdAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(order);

            var result = await _controller.GetDeletingViewAsync(Guid.Empty);

            var viewResult = result.Should().NotBeNull()
                .And.BeOfType<ViewResult>().Subject;

            viewResult.ViewName.Should().Be("Delete");

            viewResult.Model.Should().NotBeNull()
                .And.BeOfType<OrderDTO>().Which.Id.Should().Be(guid.ToString());
        }

        [Fact]
        public async Task DeleteAsync_EnteredExistedOrder_RedirectToActionIndexResult()
        {
            var result = await _controller.DeleteAsync(new OrderDTO() { Id = Guid.Empty.ToString() });

            _orderServiceMock.Verify(x => x.SoftDeleteOrderAsync(It.IsAny<Guid>()));

            var action = result.Should().NotBeNull()
                .And.BeOfType<RedirectToActionResult>().Subject;

            action.ActionName.Should().Be("Index");
        }
    }
}
