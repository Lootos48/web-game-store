using AutoMapper;
using GameStore.BLL.Interfaces;
using GameStore.DomainModels.Enums;
using GameStore.DomainModels.Models;
using GameStore.DomainModels.Models.CreateModels;
using GameStore.DomainModels.Models.EditModels;
using GameStore.PL.Configurations;
using GameStore.PL.DTOs;
using GameStore.PL.Factories.Interfaces;
using GameStore.PL.Filters;
using GameStore.PL.Util.Authorization;
using GameStore.PL.ViewContexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace GameStore.PL.Controllers
{
    [Route("[controller]")]
    public class GameController : Controller
    {
        private readonly IGoodsService _gameService;
        private readonly ICommentService _commentService;
        private readonly ICommentContextFactory _contextFactory;
        private readonly IUserService _userService;

        private readonly IMapper _mapper;
        private readonly ILogger<GameController> _logger;

        private readonly GuestCookieSettings _coockieSettings;

        public string CurrentLanguage => CultureInfo.CurrentCulture.Name;

        public GameController(
            IGoodsService gameService,
            ICommentService commentService,
            ICommentContextFactory contextFactory,
            IUserService userService,
            IMapper mapper,
            ILogger<GameController> logger,
            GuestCookieSettings coockieSettings)
        {
            _gameService=gameService;
            _commentService=commentService;
            _contextFactory=contextFactory;
            _userService=userService;
            _mapper=mapper;
            _logger=logger;
            _coockieSettings=coockieSettings;
        }

        [HttpGet("{key}/buy")]
        [ArgumentDecoderFilter("key")]
        public async Task<IActionResult> Buy(string key)
        {
            if (HttpContext.User.Identity.IsAuthenticated || HttpContext.Request.Cookies[_coockieSettings.GuestIdCookieName] != null)
            {
                return RedirectToAction("AddGameIntoBasket", "Basket", new { key });
            }

            User guest = new User
            {
                Id = Guid.NewGuid(),
                Role = UserRoles.Guest
            };
            await _userService.CreateAsync(guest);
            Response.Cookies.Append(_coockieSettings.GuestIdCookieName, guest.Id.ToString());

            return RedirectToAction("Create", "Basket", new { key = key, userId = guest.Id });
        }

        [HttpGet("{key}")]
        [ArgumentDecoderFilter("key")]
        public async Task<IActionResult> DetailsByKeyAsync(string key)
        {
            Goods foundGame = await _gameService.GetGoodsByKeyAsync(key, CurrentLanguage, includeDeletedRows: true);
            if (foundGame is null)
            {
                return NotFound();
            }

            GoodsDTO gameDto = _mapper.Map<GoodsDTO>(foundGame);

            await _gameService.IncreaseViewCountAsync(gameDto.Id);

            return View("Details", gameDto);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> DetailsByIdAsync(Guid id)
        {
            Goods foundGame = await _gameService.GetGoodsByIdAsync(id);

            GoodsDTO gameDto = _mapper.Map<GoodsDTO>(foundGame);

            await _gameService.IncreaseViewCountAsync(gameDto.Id);

            return View("Details", gameDto);
        }

        [HttpGet("{key}/comments")]
        [ArgumentDecoderFilter("key")]
        public async Task<IActionResult> GetCommentsAsync(string key)
        {
            var context = await _contextFactory.BuildGameCommentsContextViewAsync(key);

            _logger.LogDebug("Returned coments by id {GameId}: {@CommentsDTOList}", key, context.Comments);

            return View("GetComments", context);
        }

        [HttpGet("{key}/comments/reply/{commentId:guid}")]
        [ArgumentDecoderFilter("key")]
        public async Task<IActionResult> CommentReplyAsync(string key, Guid commentId)
        {
            var context = await _contextFactory.BuildGameCommentsContextViewAsync(key, commentId, CommentType.Reply);

            return View("GetComments", context);
        }

        [HttpGet("{key}/comments/quote/{commentId:guid}")]
        [ArgumentDecoderFilter("key")]
        public async Task<IActionResult> CommentQuoteAsync(string key, Guid commentId)
        {
            var context = await _contextFactory.BuildGameCommentsContextViewAsync(key, commentId, CommentType.Quote);

            return View("GetComments", context);
        }

        [HttpPost("{key}/newcomment")]
        [ArgumentDecoderFilter("key")]
        public async Task<IActionResult> CreateCommentAsync(string key, GameCommentsViewContext context)
        {
            CreateCommentRequest createRequest = _mapper.Map<CreateCommentRequest>(context.CommentToCreate);
            await _commentService.CreateCommentAsync(key, createRequest);

            _logger.LogDebug("Created comment by game id {GameId}: {@NewCommentDTO}", key, context.CommentToCreate);

            return RedirectToAction("GetComments", new { key });
        }

        [PermissionLevel(UserRoles.Moderator)]
        [HttpGet("{key}/comments/edit/{commentId:guid}")]
        [ArgumentDecoderFilter("key")]
        public async Task<IActionResult> GetEditingCommentViewAsync(string key, Guid commentId)
        {
            var context = await _contextFactory.BuildCommentEditingContextViewAsync(key, commentId);

            return View("EditComment", context);
        }

        [PermissionLevel(UserRoles.Moderator)]
        [HttpPost("{key}/comments/edit")]
        [ArgumentDecoderFilter("key")]
        public async Task<IActionResult> EditCommentAsync(string key, CommentEditingViewContext context)
        {
            EditCommentRequest editRequest = _mapper.Map<EditCommentRequest>(context.CommentToEdit);
            await _commentService.EditCommentAsync(editRequest);

            return RedirectToAction("GetComments", new { key });
        }

        [PermissionLevel(UserRoles.Moderator)]
        [HttpGet("{key}/comments/delete/{commentId:guid}")]
        [ArgumentDecoderFilter("key")]
        public async Task<IActionResult> GetDeletingCommentPartialViewAsync(string key, Guid commentId)
        {
            var context = await _contextFactory.BuildCommentDeletingContextViewAsync(key, commentId);

            return PartialView("DeleteComment", context);
        }

        [PermissionLevel(UserRoles.Moderator)]
        [HttpPost("{key}/comments/delete/{commentId:guid}")]
        [ArgumentDecoderFilter("key")]
        public async Task<IActionResult> DeleteCommentAsync(string key, Guid commentId)
        {
            await _commentService.SoftDeleteCommentAsync(commentId);

            return RedirectToAction("GetComments", new { key });
        }

        [HttpGet("{key}/download")]
        [ArgumentDecoderFilter("key")]
        public IActionResult Download(string key)
        {
            GoodsDTO gameToDownload = new GoodsDTO { Key = key };
            return View("Download", gameToDownload);
        }

        [HttpGet("download/{key}")]
        [ArgumentDecoderFilter("key")]
        public async Task<IActionResult> DownloadFileAsync(string key)
        {
            byte[] bytes = await _gameService.DownloadAsync(key);

            _logger.LogDebug("Game downloaded by game key: {id}", key);

            return File(bytes, "text/plain", $"{key}.bin");
        }
    }   
}
