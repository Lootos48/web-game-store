using GameStore.PL.DTOs;
using GameStore.PL.DTOs.CreateDTOs;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace GameStore.PL.ViewContexts
{
    public class GameCreationViewContext
    {
        public CreateGoodsRequestDTO GameCreateDTO { get; set; }

        public List<DistributorDTO> Publishers { get; set; }

        public List<GenreDTO> Genres { get; set; }

        public List<PlatformTypeDTO> PlatformTypes { get; set; }

        public IEnumerable<SelectListItem> GetPublishersAsSelectItem()
        {
            return new SelectList(Publishers, "Id", "CompanyName");
        }
        public IEnumerable<SelectListItem> GetGenresAsSelectItem()
        {
            return new MultiSelectList(Genres, "Id", "Name");
        }
        public IEnumerable<SelectListItem> GetPlatformsAsSelectItem()
        {
            return new MultiSelectList(PlatformTypes, "Id", "Type");
        }
    }
}
