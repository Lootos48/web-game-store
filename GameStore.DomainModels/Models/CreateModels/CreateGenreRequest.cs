using System;

namespace GameStore.DomainModels.Models.CreateModels
{
    public class CreateGenreRequest
    {
        public string Name { get; set; }

        public Guid? ParentId { get; set; }

        public string Description { get; set; }

        public string Picture { get; set; }
    }
}
