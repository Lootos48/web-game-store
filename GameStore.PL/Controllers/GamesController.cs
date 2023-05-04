using AutoMapper;
using GameStore.BLL.Interfaces;
using GameStore.DomainModels.Enums;
using GameStore.DomainModels.Exceptions;
using GameStore.DomainModels.Models;
using GameStore.DomainModels.Models.CreateModels;
using GameStore.DomainModels.Models.EditModels;
using GameStore.PL.DTOs;
using GameStore.PL.DTOs.EditDTOs;
using GameStore.PL.Factories.Interfaces;
using GameStore.PL.Filters;
using GameStore.PL.Util.Authorization;
using GameStore.PL.ViewContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace GameStore.PL.Controllers
{
    [Route("[controller]")]
    public class GamesController : Controller
    {
        private readonly IGoodsService _gameService;

        private readonly IMapper _mapper;
        private readonly ILogger<GamesController> _logger;
        private readonly IGameContextFactory _contextFactory;

        private string CurrentLanguage => CultureInfo.CurrentCulture.Name;

        public GamesController(
            IGoodsService gameService,
            IMapper mapper,
            ILogger<GamesController> logger,
            IGameContextFactory contextFactory)
        {
            _gameService = gameService;
            _mapper = mapper;
            _logger = logger;
            _contextFactory = contextFactory;
        }

        [HttpGet]
        public async Task<IActionResult> IndexAsync([FromQuery] GoodsSearchRequestDTO filters)
        {
            GoodsSearchRequest gamefilters = _mapper.Map<GoodsSearchRequest>(filters);

            var filteredGames = await _gameService.GetFilteredGoodsAsync(gamefilters, CurrentLanguage);

            List<GoodsDTO> filteredGamesDto = _mapper.Map<List<GoodsDTO>>(filteredGames);
            var context = await _contextFactory.BuildGamesViewContextAsync(filteredGamesDto, CurrentLanguage);

            context.Filters = filters;

            _logger.LogDebug("Returned game list: {@GamesDTOList}", context.Games);

            return View("Index", context);
        }

        [PermissionLevel(UserRoles.Manager)]
        [HttpGet("new")]
        public async Task<IActionResult> GetCreationViewAsync()
        {
            GameCreationViewContext context = await _contextFactory.BuildGameCreationContextAsync();
            return View("Create", context);
        }

        [PermissionLevel(UserRoles.Manager)]
        [HttpPost("new")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(GameCreationViewContext gameToCreate)
        {
            CreateGoodsRequest createRequest = _mapper.Map<CreateGoodsRequest>(gameToCreate.GameCreateDTO);

            string gameKey;
            try
            {
                gameKey = await _gameService.CreateGoodsAsync(createRequest);
            }
            catch (NotUniqueException ex)
            {
                ModelState.AddModelError("Key", ex.Message);
                gameToCreate = await _contextFactory.BuildGameCreationContextAsync();
                return View("Create", gameToCreate);
            }

            _logger.LogDebug("Created game: {@CreatedBySaveDTO}.", gameToCreate);

            return RedirectToAction("DetailsByKey", "Game", new { key = gameKey });
        }

        [PermissionLevel(UserRoles.Publishers)]
        [HttpGet("update/{key}")]
        [ArgumentDecoderFilter("key")]
        public async Task<IActionResult> GetEditingViewAsync(string key)
        {
            Goods foundGame = await _gameService.GetGoodsByKeyIncludeAllLocalizationsAsync(key);

            Guid? publisherId = null;
            if (foundGame.Distributor != null)
            {
                publisherId = foundGame.Distributor.UserId;
            }

            if (!HierarchicalValidator.IsUserManagerOrOwner(publisherId, User))
            {
                return Forbid();
            }

            var gameToEdit = _mapper.Map<EditGoodsRequestDTO>(foundGame);
            GameEditingViewContext gameEditContext = await _contextFactory.BuildGameEditingContextAsync(gameToEdit);

            return View("Edit", gameEditContext);
        }

        [PermissionLevel(UserRoles.Publishers)]
        [HttpPost("update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync(GameEditingViewContext context)
        {
            if (!HierarchicalValidator.IsUserManagerOrOwner(context.DistributorUserId, User))
            {
                return Forbid();
            }

            EditGoodsRequest editRequest = _mapper.Map<EditGoodsRequest>(context.GameEditDTO);
            try
            {
                await _gameService.EditGoodsAsync(editRequest);
            }
            catch (NotUniqueException ex)
            {
                ModelState.AddModelError("Key", ex.Message);
                context = await _contextFactory.BuildGameEditingContextAsync(context.GameEditDTO);
                return View("Edit", context);
            }

            return RedirectToAction("Index");
        }

        [PermissionLevel(UserRoles.Manager)]
        [HttpGet("remove/{key}")]
        [ArgumentDecoderFilter("key")]
        public async Task<IActionResult> GetDeletingViewAsync(string key)
        {
            Goods foundedGame = await _gameService.GetGoodsByKeyAsync(key);

            GoodsDTO gameToDelete = _mapper.Map<GoodsDTO>(foundedGame);
            return View("Delete", gameToDelete);
        }

        [PermissionLevel(UserRoles.Manager)]
        [HttpPost("remove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            await _gameService.SoftDeleteGoodsAsync(id);
            return RedirectToAction("Index");
        }
    }
}
