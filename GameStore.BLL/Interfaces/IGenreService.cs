using GameStore.DomainModels.Models;
using GameStore.DomainModels.Models.CreateModels;
using GameStore.DomainModels.Models.EditModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.BLL.Interfaces
{
    public interface IGenreService
    {
        Task<Genre> GetGenreByNameAsync(string genre, string localizationCode = null);

        Task<Genre> GetGenreByIdAsync(Guid id);

        Task<List<Genre>> GetAllGenresAsync(string localizationCultureCode = null);

        Task<List<Goods>> GetGamesByGenreAsync(string genre, string localizationCode);

        Task CreateGenreAsync(CreateGenreRequest genreToCreate);

        Task EditGenreAsync(EditGenreRequest gameToEdit);

        Task HardDeleteGenreAsync(Guid id);

        Task SoftDeleteGenreAsync(Guid id);

        Task<Genre> GetGenreByNameWithAllLocalizationsAsync(string genre);
    }
}
