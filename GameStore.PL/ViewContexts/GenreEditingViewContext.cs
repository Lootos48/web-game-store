using GameStore.PL.DTOs;
using GameStore.PL.DTOs.EditDTOs;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace GameStore.PL.ViewContexts
{
    public class GenreEditingViewContext
    {
        public EditGenreRequestDTO GenreToEdit { get; set; }

        public List<GenreDTO> Genres { get; set; } = new List<GenreDTO>();

        public Dictionary<string, Guid> AvailableLocalizations { get; set; } = new Dictionary<string, Guid>();

        public IEnumerable<SelectListItem> GetGenresAsSelectItem()
        {
            return new SelectList(Genres, "Id", "Name");
        }
    }
}
