using AutoMapper;
using GameStore.DAL.Abstractions.Interfaces;
using GameStore.DAL.Entities;
using GameStore.DomainModels.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.DAL.Repositories
{
    public class GenreCategoryRepository : IGenreCategoryRepository
    {
        protected readonly DbContext _context;
        protected readonly DbSet<GenreCategoryMappingEntity> _dbSet;
        protected readonly IMapper _mapper;
        protected readonly ILogger<GenreCategoryRepository> _logger;

        public GenreCategoryRepository(
            DbContext context,
            IMapper mapper,
            ILogger<GenreCategoryRepository> logger)
        {
            _context = context;
            _dbSet = context.Set<GenreCategoryMappingEntity>();
            _mapper = mapper;
            _logger = logger;
        }

        public Task CreateAsync(GenreCategoryMapping newMapping)
        {
            var mappingEntity = _mapper.Map<GenreCategoryMappingEntity>(newMapping);

            _dbSet.Add(mappingEntity);
            return _context.SaveChangesAsync();
        }

        public async Task<List<GenreCategoryMapping>> GetAllAsync()
        {
            List<GenreCategoryMappingEntity> entityList = await _dbSet.Include(e => e.Genre).ToListAsync();

            return _mapper.Map<List<GenreCategoryMapping>>(entityList);
        }

        public async Task<List<GenreCategoryMapping>> GetByCategoryIdAsync(int categoryId)
        {
            var entities = await _dbSet
                .Where(gg => gg.CategoryId == categoryId)
                .ToListAsync();

            return _mapper.Map<List<GenreCategoryMapping>>(entities);
        }

        public async Task<List<GenreCategoryMapping>> GetByGenreIdAsync(Guid genreId)
        {
            var entities = await _dbSet
                .Where(gg => gg.GenreId == genreId)
                .ToListAsync();

            return _mapper.Map<List<GenreCategoryMapping>>(entities);
        }

        public async Task<GenreCategoryMapping> FindAsync(Guid genreId, int categoryId)
        {
            var entity = await _dbSet
                .FirstOrDefaultAsync(gc => gc.GenreId == genreId && gc.CategoryId == categoryId);

            return _mapper.Map<GenreCategoryMapping>(entity);
        }

        public Task DeleteRangeAsync(List<GenreCategoryMapping> genresCategories)
        {
            FormattableString query = BuildRangeDeletingQuery(genresCategories);

            return _context.Database.ExecuteSqlInterpolatedAsync(query);
        }

        private static FormattableString BuildRangeDeletingQuery(List<GenreCategoryMapping> genreCategoryToDelete)
        {
            StringBuilder query = new StringBuilder("DELETE FROM [GenreCategory] WHERE ");

            object[] arguments = new object[genreCategoryToDelete.Count * 2];
            for (int i = 0, argumentCounter = 0; i < genreCategoryToDelete.Count; i++, argumentCounter += 2)
            {
                query.Append("[CategoryId] = {" + argumentCounter + "} AND [GenreId] = {"+ (argumentCounter + 1) +"} OR");

                arguments[argumentCounter] = genreCategoryToDelete[i].CategoryId;
                arguments[argumentCounter + 1] = genreCategoryToDelete[i].GenreId;
            }

            query.Remove(query.Length - 2, 2);
            query.Append(";");

            return FormattableStringFactory.Create(query.ToString(), arguments);
        }
    }
}
