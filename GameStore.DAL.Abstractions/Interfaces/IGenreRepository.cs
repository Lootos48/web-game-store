using GameStore.DomainModels.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.DAL.Abstractions.Interfaces
{
    public interface IGenreRepository
    {
        Task<Genre> FindByNameAsync(string name, string localizationCode = null, bool isIncludeDeleted = false);

        Task<List<Genre>> GetChildGenresAsync(Guid id, bool isIncludeDeleted = false);

        Task BatchUpdateParentsAsync(List<Genre> genres);

        Task CreateAsync(Genre item);

        Task DeleteAsync(Genre item);

        Task<List<Genre>> GetAllAsync(string localizationCode = null, bool isIncludeDeleted = false);

        Task<Genre> FindByIdAsync(Guid id, bool isIncludeDeleted = false);

        Task UpdateAsync(Genre item);

        Task UpdateLocalizationAsync(Genre item, Guid chosenLocalization);

        Task RecoverAsync(Guid id);

        Task SoftDeleteAsync(Guid id);

        Task DeleteByIdAsync(Guid id);

        Task<bool> IsGenreUnique(Genre genre);

        Task<Genre> FindByNameIncludeAllLocalizationsAsync(string name, bool isIncludeDeleted = false);
    }
}
