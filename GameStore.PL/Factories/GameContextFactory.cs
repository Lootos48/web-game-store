using AutoMapper;
using GameStore.BLL.Interfaces;
using GameStore.DomainModels.Models.Localizations;
using GameStore.PL.DTOs;
using GameStore.PL.DTOs.EditDTOs;
using GameStore.PL.DTOs.LocalizationsDTO;
using GameStore.PL.Factories.Interfaces;
using GameStore.PL.ViewContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore.PL.Factories
{
    public class GameContextFactory : IGameContextFactory
    {
        private readonly IDistributorService _publisherService;
        private readonly IPlatformTypeService _platformTypeService;
        private readonly IGenreService _genreService;
        private readonly IGoodsService _gameService;
        private readonly ILocalizationService _localizationService;

        private readonly IMapper _mapper;

        public GameContextFactory(
            IDistributorService publisherService,
            IPlatformTypeService platformTypeService,
            IGenreService genreService,
            IGoodsService gameService,
            ILocalizationService localizationService,
            IMapper mapper)
        {
            _publisherService=publisherService;
            _platformTypeService=platformTypeService;
            _genreService=genreService;
            _gameService=gameService;
            _localizationService=localizationService;
            _mapper=mapper;
        }

        public async Task<GameCreationViewContext> BuildGameCreationContextAsync()
        {
            var genres = await _genreService.GetAllGenresAsync();
            var platforms = await _platformTypeService.GetAllPlatformsAsync();
            var publishers = await _publisherService.GetAllDistributorsAsync();

            GameCreationViewContext context = new GameCreationViewContext
            {
                Genres = _mapper.Map<List<GenreDTO>>(genres),
                PlatformTypes = _mapper.Map<List<PlatformTypeDTO>>(platforms),
                Publishers = _mapper.Map<List<DistributorDTO>>(publishers),
            };

            return context;
        }

        public async Task<GameEditingViewContext> BuildGameEditingContextAsync(EditGoodsRequestDTO gameToEdit)
        {
            var genres = await _genreService.GetAllGenresAsync();
            var platforms = await _platformTypeService.GetAllPlatformsAsync();
            var publishers = await _publisherService.GetAllDistributorsAsync();
            var localizations = await _localizationService.GetAllLocalizationsAsync();

            SetupGameLocalizations(localizations, gameToEdit);

            Dictionary<string, Guid> availableLocalizations = new Dictionary<string, Guid>();
            foreach (var localization in localizations)
            {
                availableLocalizations.Add(localization.CultureCode, localization.Id);
            }

            GameEditingViewContext context = new GameEditingViewContext
            {
                GameEditDTO = gameToEdit,
                Genres = _mapper.Map<List<GenreDTO>>(genres),
                PlatformTypes = _mapper.Map<List<PlatformTypeDTO>>(platforms),
                Publishers = _mapper.Map<List<DistributorDTO>>(publishers),
                AvailableLocalizations = availableLocalizations
            };

            var distributor = publishers.FirstOrDefault(x => x.Id == gameToEdit.DistributorId);
            if (distributor != null )
            {
                context.DistributorUserId = distributor.UserId;
            }

            return context;
        }

        private void SetupGameLocalizations(List<Localization> localizations, EditGoodsRequestDTO gameToEdit)
        {
            foreach (var language in localizations)
            {
                if (!gameToEdit.Localizations.Any(x => x.Localization.CultureCode == language.CultureCode && language.CultureCode != "en-US"))
                {
                    gameToEdit.Localizations.Add(new GoodsLocalizationDTO
                    {
                        LocalizationId = language.Id,
                        Id = Guid.NewGuid(),
                        Localization = _mapper.Map<LocalizationDTO>(language)
                    });
                }
            }
        }

        public async Task<GamesViewsContext> BuildGamesViewContextAsync(List<GoodsDTO> filteredGames = null, string localizationCultureCode = null)
        {
            var context = await SetupGamesViewContextAsync(localizationCultureCode);

            context.Games = filteredGames;

            return context;
        }

        private async Task<GamesViewsContext> SetupGamesViewContextAsync(string localizationCultureCode = null)
        {
            var genres = await _genreService.GetAllGenresAsync(localizationCultureCode);
            var platformTypes = await _platformTypeService.GetAllPlatformsAsync(localizationCultureCode);
            var publishers = await _publisherService.GetAllDistributorsAsync();

            var context = new GamesViewsContext
            {
                GenresFilterOptions = _mapper.Map<List<GenreDTO>>(genres),
                PlatformsFilterOptions = _mapper.Map<List<PlatformTypeDTO>>(platformTypes),
                PublishersFilterOptions = _mapper.Map<List<DistributorDTO>>(publishers)
            };

            return context;
        }
    }
}
