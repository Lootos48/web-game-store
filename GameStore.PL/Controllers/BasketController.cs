using AutoMapper;
using GameStore.BLL.Interfaces;
using GameStore.DomainModels.Models;
using GameStore.DomainModels.Models.CreateModels;
using GameStore.PL.App_Code;
using GameStore.PL.Configurations;
using GameStore.PL.DTOs;
using GameStore.PL.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace GameStore.PL.Controllers
{
    [Route("[controller]")]
    public class BasketController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly ILogger<BasketController> _logger;

        private readonly GuestCookieSettings _coockieSettings;

        private string CurrentLanguage => CultureInfo.CurrentCulture.Name;

        public BasketController(
            IOrderService orderService,
            IMapper mapper,
            GuestCookieSettings cookieSettings,
            ILogger<BasketController> logger)
        {
            _orderService = orderService;
            _coockieSettings = cookieSettings;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet("new")]
        [ArgumentDecoderFilter("key")]
        public async Task<IActionResult> CreateAsync(string key, Guid userId)
        {
            CreateOrderRequest saveRequest = new CreateOrderRequest()
            {
                CustomerId = userId,
            };
            await _orderService.CreateOrderAsync(saveRequest);

            return RedirectToAction("AddGameIntoBasket", new { key });
        }

        [HttpGet]
        [ArgumentDecoderFilter("key")]
        public async Task<IActionResult> AddGameIntoBasketAsync(string key)
        {
            Guid userId = GetUserIdFromContext();

            await _orderService.AddGameInOrderAsync(key, userId);

            return RedirectToAction("Details", new { id = userId });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> DetailsAsync(Guid id)
        {
            Order foundOrder = await _orderService.GetOrderByCustomerIdAsync(id, CurrentLanguage);

            OrderDTO orderDto = _mapper.Map<OrderDTO>(foundOrder);

            return View("Details", orderDto);
        }

        private Guid GetUserIdFromContext()
        {
            var userId = ClaimsHelper.GetUserId(User.Claims);
            if (userId.HasValue)
            {
                return userId.Value;
            }

            var guestStringId = HttpContext.Request.Cookies[_coockieSettings.GuestIdCookieName];
            if (!Guid.TryParse(guestStringId, out Guid guestId))
            {
                throw new ArgumentException("Cant get user or guest id");
            }

            return guestId;
        }
    }
}
