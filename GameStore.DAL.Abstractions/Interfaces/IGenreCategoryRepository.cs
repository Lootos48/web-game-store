using GameStore.DomainModels.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.DAL.Abstractions.Interfaces
{
    public interface IGenreCategoryRepository
    {
        Task DeleteRangeAsync(List<GenreCategoryMapping> genresCategories);

        Task<GenreCategoryMapping> FindAsync(Guid genreId, int categoryId);

        Task<List<GenreCategoryMapping>> GetByCategoryIdAsync(int categoryId);

        Task<List<GenreCategoryMapping>> GetByGenreIdAsync(Guid genreId);

        Task<List<GenreCategoryMapping>> GetAllAsync();

        Task CreateAsync(GenreCategoryMapping newMapping);
    }
}