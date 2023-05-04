using AutoMapper;
using GameStore.BLL.Interfaces;
using GameStore.DomainModels.Models;
using GameStore.DomainModels.Models.Localizations;
using GameStore.PL.DTOs;
using GameStore.PL.DTOs.CreateDTOs;
using GameStore.PL.DTOs.EditDTOs;
using GameStore.PL.Factories.Interfaces;
using GameStore.PL.Util.Localizers;
using GameStore.PL.ViewContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore.PL.Factories
{
    public class GenreContextFactory : IGenreContextFactory
    {
        private readonly IGenreService _genreService;
        private readonly ILocalizationService _localizationService;
        private readonly IMapper _mapper;

        public GenreContextFactory(
            IGenreService genreService,
            ILocalizationService localizationService,
            IMapper mapper)
        {
            _localizationService = localizationService;
            _genreService = genreService;
            _mapper = mapper;
        }

        public async Task<GamesGenreDetailsViewContext> BuildGamesGenreDetailsViewContextAsync(string genre, string localizationCode = null)
        {
            Genre foundedGenre = await _genreService.GetGenreByNameAsync(genre, localizationCode);
            List<Goods> games = await _genreService.GetGamesByGenreAsync(genre, localizationCode);

            GamesGenreDetailsViewContext context = new GamesGenreDetailsViewContext
            {
                Genre = _mapper.Map<GenreDTO>(foundedGenre),
                GamesInGenre = _mapper.Map<List<GoodsDTO>>(games),
            };

            return context;
        }

        public async Task<GenreEditingViewContext> BuildGenreEditingViewContextAsync(string genre, string cultureCode)
        {
            Genre foundedGenre = await _genreService.GetGenreByNameWithAllLocalizationsAsync(genre);
            List<Genre> genres = await _genreService.GetAllGenresAsync(cultureCode);
            List<Localization> localizations = await _localizationService.GetAllLocalizationsAsync();

            SetupGenreLocalizations(localizations, foundedGenre);

            Dictionary<string, Guid> availableLocalizations = new Dictionary<string, Guid>();
            foreach (var localization in localizations)
            {
                availableLocalizations.Add(localization.CultureCode, localization.Id);
            }

            genres = genres.Where(x => x.Id != foundedGenre.Id).ToList();
            List<GenreDTO> genresDtos = LocalizeGenres(genres);

            GenreEditingViewContext context = new GenreEditingViewContext
            {
                GenreToEdit = _mapper.Map<EditGenreRequestDTO>(foundedGenre),
                Genres = genresDtos,
                AvailableLocalizations = availableLocalizations
            };

            return context;
        }

        public async Task<GenreCreationViewContext> BuildGenreCreationViewContextAsync()
        {
            CreateGenreRequestDTO genreCreate = new CreateGenreRequestDTO();
            List<Genre> genres = await _genreService.GetAllGenresAsync();

            GenreCreationViewContext context = new GenreCreationViewContext
            {
                GenreToCreate = genreCreate,
                Genres = _mapper.Map<List<GenreDTO>>(genres)
            };

            return context;
        }

        private List<GenreDTO> LocalizeGenres(List<Genre> genres)
        {
            var genresDtos = _mapper.Map<List<GenreDTO>>(genres);
            genresDtos.ForEach(genre =>
            {
                genre.Name = FieldLocalizer.GetLocalizedField(g => g.Name, genre);
            });
            return genresDtos;
        }

        private static void SetupGenreLocalizations(List<Localization> localizations, Genre foundedGenre)
        {
            foreach (var language in localizations)
            {
                if (!foundedGenre.Localizations.Any(x => x.Localization.CultureCode == language.CultureCode && language.CultureCode != "en-US"))
                {
                    foundedGenre.Localizations.Add(new GenreLocalization
                    {
                        LocalizationId = language.Id,
                        Localization = language,
                        Id = Guid.NewGuid(),
                        GenreId = foundedGenre.Id,
                        Genre = foundedGenre
                    });
                }
            }
        }
    }
}
