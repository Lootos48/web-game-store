using AutoMapper;
using GameStore.BLL.Interfaces;
using GameStore.DomainModels.Models;
using GameStore.DomainModels.Models.Localizations;
using GameStore.PL.DTOs;
using GameStore.PL.DTOs.EditDTOs;
using GameStore.PL.Factories.Interfaces;
using GameStore.PL.ViewContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore.PL.Factories
{
    public class PlatformContextFactory : IPlatformContextFactory
    {
        private readonly IPlatformTypeService _platformTypeService;
        private readonly ILocalizationService _localizationService;

        private readonly IMapper _mapper;

        public PlatformContextFactory(
            IPlatformTypeService platformTypeService,
            ILocalizationService localizationService,
            IMapper mapper)
        {
            _platformTypeService = platformTypeService;
            _localizationService = localizationService;
            _mapper = mapper;
        }

        public async Task<PlatformTypeEditingViewContext> BuildPlatformTypeEditingViewContextAsync(string type)
        {
            var platformType = await _platformTypeService.GetPlatformByTypeIncludeAllLocalizationAsync(type);
            var localizations = await _localizationService.GetAllLocalizationsAsync();

            SetupPlatformTypeLocalizations(localizations, platformType);

            Dictionary<string, Guid> availableLocalizations = new Dictionary<string, Guid>();
            foreach (var localization in localizations)
            {
                availableLocalizations.Add(localization.CultureCode, localization.Id);
            }

            var context = new PlatformTypeEditingViewContext
            {
                PlatformType = _mapper.Map<EditPlatformTypeRequestDTO>(platformType),
                AvailableLocalizations = availableLocalizations
            };

            return context;
        }

        public async Task<GamesPlatformDetailsViewContext> BuildGamesPlatformDetailsViewContextAsync(string platform, string localizationCultureCode)
        {
            List<Goods> resultGames = await _platformTypeService.GetGamesByPlatformAsync(platform, localizationCultureCode);
            PlatformType resultPlatforms = await _platformTypeService.GetPlatformByTypeAsync(platform, localizationCultureCode);

            GamesPlatformDetailsViewContext context = new GamesPlatformDetailsViewContext
            {
                PlatformDTO = _mapper.Map<PlatformTypeDTO>(resultPlatforms),
                GamesOnPlatform = _mapper.Map<List<GoodsDTO>>(resultGames)
            };

            return context;
        }

        private static void SetupPlatformTypeLocalizations(List<Localization> localizations, PlatformType platformType)
        {
            foreach (var language in localizations)
            {
                if (!platformType.Localizations.Any(x => x.Localization.CultureCode == language.CultureCode && language.CultureCode != "en-US"))
                {
                    platformType.Localizations.Add(new PlatformTypeLocalization
                    {
                        LocalizationId = language.Id,
                        Localization = language,
                        Id = Guid.NewGuid(),
                        PlatformTypeId = platformType.Id,
                        PlatformType = platformType
                    });
                }
            }
        }
    }
}
