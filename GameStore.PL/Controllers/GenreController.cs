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
    public class GenreController : Controller
    {
        private readonly IGenreService _genreService;
        private readonly IMapper _mapper;
        private readonly ILogger<GenreController> _logger;
        private readonly IGenreContextFactory _contextFactory;

        public string CurrentLanguage => CultureInfo.CurrentCulture.Name;

        public GenreController(
            IGenreService genreService,
            IMapper mapper,
            ILogger<GenreController> logger,
            IGenreContextFactory contextFactory)
        {
            _genreService=genreService;
            _mapper=mapper;
            _logger=logger;
            _contextFactory=contextFactory;
        }

        [HttpGet]
        public async Task<IActionResult> IndexAsync()
        {
            List<Genre> genres = await _genreService.GetAllGenresAsync(CurrentLanguage);
            List<GenreDTO> genresDTO = _mapper.Map<List<GenreDTO>>(genres);

            return View("Index", genresDTO);
        }

        [HttpGet("{genre}")]
        [ArgumentDecoderFilter("genre")]
        public async Task<IActionResult> DetailsAsync([FromRoute] string genre)
        {
            var context = await _contextFactory.BuildGamesGenreDetailsViewContextAsync(genre, CurrentLanguage);

            _logger.LogDebug("Returned game list by genre {@Genre}: {@GamesDTOList}", genre, context.GamesInGenre);

            return View("Details", context);
        }

        [PermissionLevel(UserRoles.Manager)]
        [HttpGet("new")]
        public async Task<IActionResult> GetCreationViewAsync()
        {
            var context = await _contextFactory.BuildGenreCreationViewContextAsync();
            return View("Create", context);
        }

        [PermissionLevel(UserRoles.Manager)]
        [HttpPost("new")]
        public async Task<IActionResult> CreateAsync(GenreCreationViewContext context)
        {
            CreateGenreRequest createRequest = _mapper.Map<CreateGenreRequest>(context.GenreToCreate);

            try
            {
                await _genreService.CreateGenreAsync(createRequest);
            }
            catch (NotUniqueException ex)
            {
                ModelState.AddModelError("Name", ex.Message);
                context = await _contextFactory.BuildGenreCreationViewContextAsync();
                return View("Create", context);
            }

            return RedirectToAction("Details", new { genre = createRequest.Name });
        }

        [PermissionLevel(UserRoles.Manager)]
        [HttpGet("update/{genre}")]
        [ArgumentDecoderFilter("genre")]
        public async Task<IActionResult> GetEditingViewAsync(string genre)
        {
            var context = await _contextFactory.BuildGenreEditingViewContextAsync(genre, CurrentLanguage);
            return View("Edit", context);
        }

        [PermissionLevel(UserRoles.Manager)]
        [HttpPost("update")]
        public async Task<IActionResult> EditAsync(GenreEditingViewContext context)
        {
            EditGenreRequest editRequest = _mapper.Map<EditGenreRequest>(context.GenreToEdit);

            try
            {
                await _genreService.EditGenreAsync(editRequest);
            }
            catch (NotUniqueException ex)
            {
                ModelState.AddModelError("Name", ex.Message);
                context = await _contextFactory.BuildGenreEditingViewContextAsync(context.GenreToEdit.Name, CurrentLanguage);
                return View("Edit", context);
            }

            return RedirectToAction("Index");
        }

        [PermissionLevel(UserRoles.Manager)]
        [HttpGet("remove/{genre}")]
        [ArgumentDecoderFilter("genre")]
        public async Task<IActionResult> GetDeletingViewAsync(string genre)
        {
            Genre foundedGenre = await _genreService.GetGenreByNameAsync(genre, CurrentLanguage);
            GenreDTO genreToDelete = _mapper.Map<GenreDTO>(foundedGenre);

            return View("Delete", genreToDelete);
        }

        [PermissionLevel(UserRoles.Manager)]
        [HttpPost("remove")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await _genreService.SoftDeleteGenreAsync(id);
            return RedirectToAction("Index");
        }
    }
}
