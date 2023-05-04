using GameStore.PL.DTOs;
using GameStore.PL.DTOs.EditDTOs;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace GameStore.PL.ViewContexts
{
    public class GameEditingViewContext
    {
        public EditGoodsRequestDTO GameEditDTO { get; set; }

        public List<DistributorDTO> Publishers { get; set; }

        public List<GenreDTO> Genres { get; set; }

        public List<PlatformTypeDTO> PlatformTypes { get; set; }

        public Guid? DistributorUserId { get; set; }

        public Dictionary<string, Guid> AvailableLocalizations { get; set; } = new Dictionary<string, Guid>();

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
