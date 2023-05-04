using GameStore.DomainModels.Models.Localizations;
using System;
using System.Collections.Generic;

namespace GameStore.DomainModels.Models.EditModels
{
    public class EditGenreRequest
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Picture { get; set; }

        public Guid? ParentId { get; set; }

        public Guid? ChosedLocalization { get; set; }

        public List<GenreLocalization> Localizations { get; set; }
    }
}
