using GameStore.PL.DTOs;
using GameStore.PL.DTOs.CreateDTOs;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace GameStore.PL.ViewContexts
{
    public class GenreCreationViewContext
    {
        public CreateGenreRequestDTO GenreToCreate { get; set; }

        public List<GenreDTO> Genres { get; set; }

        public IEnumerable<SelectListItem> GetGenresAsSelectItem()
        {
            return new SelectList(Genres, "Id", "Name");
        }
    }
}
