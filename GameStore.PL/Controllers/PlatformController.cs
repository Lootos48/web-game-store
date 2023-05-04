using AutoMapper;
using GameStore.BLL.Interfaces;
using GameStore.DomainModels.Enums;
using GameStore.DomainModels.Exceptions;
using GameStore.DomainModels.Models;
using GameStore.DomainModels.Models.CreateModels;
using GameStore.DomainModels.Models.EditModels;
using GameStore.PL.DTOs;
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
    public class PlatformController : Controller
    {
        private readonly IPlatformTypeService _platformTypeService;
        private readonly IMapper _mapper;
        private readonly ILogger<PlatformController> _logger;
        private readonly IPlatformContextFactory _contextFactory;

        public string CurrentLanguage => CultureInfo.CurrentCulture.Name;

        public PlatformController(
            IPlatformTypeService platformTypeService,
            IMapper mapper,
            ILogger<PlatformController> logger,
            IPlatformContextFactory factory)
        {
            _platformTypeService = platformTypeService;
            _mapper = mapper;
            _logger = logger;
            _contextFactory = factory;
        }

        [HttpGet]
        public async Task<IActionResult> IndexAsync()
        {
            List<PlatformType> platforms = await _platformTypeService.GetAllPlatformsAsync(CurrentLanguage);
            List<PlatformTypeDTO> platformsDTO = _mapper.Map<List<PlatformTypeDTO>>(platforms);

            return View("Index", platformsDTO);
        }

        [HttpGet("{type}")]
        [ArgumentDecoderFilter("type")]
        public async Task<IActionResult> DetailsAsync(string type)
        {
            var context = await _contextFactory.BuildGamesPlatformDetailsViewContextAsync(type, CurrentLanguage);

            _logger.LogDebug("Returned game by id: {@GameDTO}", context.GamesOnPlatform);

            return View("Details", context);
        }

        [PermissionLevel(UserRoles.Manager)]
        [HttpGet("new")]
        public IActionResult GetCreationViewAsync()
        {
            return View("Create");
        }

        [PermissionLevel(UserRoles.Manager)]
        [HttpPost("new")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(PlatformTypeDTO platformTypeToCreate)
        {
            CreatePlatformTypeRequest createRequest = _mapper.Map<CreatePlatformTypeRequest>(platformTypeToCreate);

            try
            {
                await _platformTypeService.CreatePlatformAsync(createRequest);
            }
            catch (NotUniqueException ex)
            {
                ModelState.AddModelError("Type", ex.Message);
                return View("Create");
            }

            return RedirectToAction("Details", new { type = createRequest.Type });
        }

        [PermissionLevel(UserRoles.Manager)]
        [HttpGet("update/{type}")]
        [ArgumentDecoderFilter("type")]
        public async Task<IActionResult> GetEditingViewAsync(string type)
        {
            var context = await _contextFactory.BuildPlatformTypeEditingViewContextAsync(type);

            return View("Edit", context);
        }

        [PermissionLevel(UserRoles.Manager)]
        [HttpPost("update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync(PlatformTypeEditingViewContext context)
        {
            EditPlatformRequest editRequest = _mapper.Map<EditPlatformRequest>(context.PlatformType);

            try
            {
                await _platformTypeService.EditPlatformAsync(editRequest);
            }
            catch (NotUniqueException ex)
            {
                ModelState.AddModelError("Type", ex.Message);
                return View("Edit");
            }

            return RedirectToAction("Index");
        }

        [PermissionLevel(UserRoles.Manager)]
        [HttpGet("remove/{type}")]
        [ArgumentDecoderFilter("type")]
        public async Task<IActionResult> GetDeletingViewAsync(string type)
        {
            PlatformType foundedPlatform = await _platformTypeService.GetPlatformByTypeAsync(type, CurrentLanguage);
            PlatformTypeDTO platformToDelete = _mapper.Map<PlatformTypeDTO>(foundedPlatform);

            return View("Delete", platformToDelete);
        }

        [PermissionLevel(UserRoles.Manager)]
        [HttpPost("remove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await _platformTypeService.SoftDeletePlatformAsync(id);
            return RedirectToAction("Index");
        }
    }
}
