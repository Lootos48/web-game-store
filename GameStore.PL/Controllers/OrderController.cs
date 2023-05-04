using AutoMapper;
using GameStore.BLL.Interfaces;
using GameStore.BLL.PaymentMethods;
using GameStore.BLL.Workflows.Interfaces;
using GameStore.DomainModels.Enums;
using GameStore.DomainModels.Models;
using GameStore.DomainModels.Models.EditModels;
using GameStore.PL.DTOs;
using GameStore.PL.DTOs.EditDTOs;
using GameStore.PL.Factories.Interfaces;
using GameStore.PL.Util.Authorization;
using GameStore.PL.ViewContexts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace GameStore.PL.Controllers
{
    [Route("[controller]")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;
        private readonly IOrderContextFactory _contextFactory;
        private readonly IInvoiceGenerationWorkflow _workflow;
        private readonly IMapper _mapper;

        private string CurrentLanguage => CultureInfo.CurrentCulture.Name;

        public OrderController(
            IOrderService orderService,
            IPaymentService paymentService,
            IOrderContextFactory factory,
            IInvoiceGenerationWorkflow workflow,
            IMapper mapper)
        {
            _orderService = orderService;
            _paymentService = paymentService;
            _contextFactory = factory;
            _workflow = workflow;
            _mapper = mapper;
        }

        [PermissionLevel(UserRoles.Manager)]
        [HttpGet]
        public async Task<IActionResult> IndexAsync([FromQuery] OrdersFiltrationByDatesViewContext context)
        {
            if (!context.MinDate.HasValue && !context.MaxDate.HasValue)
            {
                context.MinDate = DateTime.UtcNow.AddDays(-30);
            }

            List<Order> orders = await _orderService.GetGameStoreOrdersBetweenDatesAsync(context.MinDate, context.MaxDate);
            List<OrderDTO> ordersDTO = _mapper.Map<List<OrderDTO>>(orders);

            context.Orders = ordersDTO;

            return View("Index", context);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetOrdersHistoryAsync([FromQuery] OrdersFiltrationByDatesViewContext context)
        {
            if (!context.MinDate.HasValue && !context.MaxDate.HasValue)
            {
                context.MaxDate = DateTime.UtcNow.AddDays(-30);
            }

            List<Order> orders = await _orderService.GetBetweenDatesAsync(context.MinDate, context.MaxDate);
            List<OrderDTO> ordersDTO = _mapper.Map<List<OrderDTO>>(orders);

            context.Orders = ordersDTO;

            return View("History", context);
        }

        [HttpPost("change-order-quantity")]
        public async Task<IActionResult> MadeOrderDetailsQuantityChangings(OrderDTO order)
        {
            var orderDetails = _mapper.Map<List<OrderDetails>>(order.OrderDetails);

            await _orderService.BatchChangeProductQuantityAsync(orderDetails);

            return RedirectToAction("GetMakingOrderView", new { id = Guid.Parse(order.Id) });
        }

        [HttpGet("{id}/make-order")]
        public async Task<IActionResult> GetMakingOrderViewAsync(Guid id)
        {
            Order foundedOrder = await _orderService.GetOrderByIdAsync(id);

            var orderToEditDTO = _mapper.Map<EditOrderRequestDTO>(foundedOrder);
            if (!string.IsNullOrEmpty(User.Identity.Name))
            {
                orderToEditDTO.ShipName = User.Identity.Name;
            }

            OrdersEditingViewContext context = await _contextFactory.BuildOrdersEditingViewContext(orderToEditDTO);

            return View("Ordering", context);
        }

        [HttpPost("make-order")]
        public async Task<IActionResult> MakeOrderAsync(OrdersEditingViewContext context)
        {
            EditOrderRequest orderToMake = _mapper.Map<EditOrderRequest>(context.OrderToEdit);

            orderToMake.OldStatus = context.OrderToEdit.Status;
            orderToMake.OrderDate = DateTime.UtcNow;

            await _orderService.EditOrderAsync(orderToMake);

            var order = await _orderService.GetOrderByIdAsync(orderToMake.Id, CurrentLanguage);

            var orderDto = _mapper.Map<OrderDTO>(order);

            return View("OrderSubmiting", orderDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> DetailsAsync(Guid id)
        {
            Order foundedOrder = await _orderService.GetOrderByIdAsync(id, CurrentLanguage);
            OrderDTO ordersDTO = _mapper.Map<OrderDTO>(foundedOrder);

            return View("Details", ordersDTO);
        }

        [PermissionLevel(UserRoles.Manager)]
        [HttpGet("update/{id}")]
        public async Task<IActionResult> GetEditingViewAsync(string id)
        {
            Order foundedOrder = await _orderService.GetOrderByIdAsync(Guid.Parse(id));
            if (foundedOrder.Status == OrderStatus.Completed)
            {
                return Forbid();
            }

            var orderToEditDTO = _mapper.Map<EditOrderRequestDTO>(foundedOrder);
            if (!string.IsNullOrEmpty(User.Identity.Name))
            {
                orderToEditDTO.ShipName = User.Identity.Name;
            }

            OrdersEditingViewContext context = await _contextFactory.BuildOrdersEditingViewContext(orderToEditDTO);

            return View("Edit", context);
        }

        [PermissionLevel(UserRoles.Manager)]
        [HttpPost("update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync(OrdersEditingViewContext context)
        {
            EditOrderRequest editRequest = _mapper.Map<EditOrderRequest>(context.OrderToEdit);

            await _orderService.EditOrderAsync(editRequest);

            return RedirectToAction("Details", new { id = context.OrderToEdit.Id });
        }

        [PermissionLevel(UserRoles.Manager)]
        [HttpGet("remove/{id:guid}")]
        public async Task<IActionResult> GetDeletingViewAsync(Guid id)
        {
            Order foundedOrder = await _orderService.GetOrderByIdAsync(id);
            OrderDTO orderToDelete = _mapper.Map<OrderDTO>(foundedOrder);

            return View("Delete", orderToDelete);
        }

        [PermissionLevel(UserRoles.Manager)]
        [HttpPost("remove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAsync(OrderDTO order)
        {
            if (!Guid.TryParse(order.Id, out Guid orderId))
            {
                return NotFound();
            }

            await _orderService.SoftDeleteOrderAsync(orderId);
            return RedirectToAction("Index");
        }

        [HttpPost("{id:guid}/payment/{paymentMethod}")]
        public async Task<IActionResult> PayAsync(Guid id, PaymentType paymentMethod)
        {
            await _paymentService.PayAsync(id, paymentMethod);

            return RedirectToAction("CloseOrder", new { id = id });
        }

        [HttpGet("{id:guid}/payment/success")]
        public async Task<IActionResult> CloseOrderAsync(Guid id)
        {
            await _orderService.CloseOrderAsync(id);

            return View("Success");
        }

        [HttpGet("{id:guid}/payment/bank")]
        public async Task<IActionResult> GetBankInvoiceAsync(Guid id)
        {
            var file = await _workflow.GetBankInvoiceAsync(id);

            await _orderService.StartOrderProcessingAsync(id, PaymentType.Bank);

            return File(file, "application/pdf", $"invoice-{id}.pdf");
        }

        [HttpGet("{id:guid}/payment/ibox")]
        public async Task<IActionResult> GetIBoxPaymentViewAsync(Guid id)
        {
            var context = await _contextFactory.BuildOrderIBoxPaymentViewContext(id);

            await _orderService.StartOrderProcessingAsync(id, PaymentType.IBox);

            return View("IBoxPayment", context);
        }

        [HttpGet("{id:guid}/payment/visa")]
        public async Task<IActionResult> GetVisaPaymentViewAsync(Guid id)
        {
            await _orderService.StartOrderProcessingAsync(id, PaymentType.Visa);

            return View("VisaPayment", id);
        }
    }
}
