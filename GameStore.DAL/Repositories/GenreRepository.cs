using AutoMapper;
using GameStore.DAL.Abstractions.Interfaces;
using GameStore.DAL.Entities;
using GameStore.DAL.Entities.Localizations;
using GameStore.DAL.Repositories.MsSqlDdRepositories;
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
    public class GenreRepository : SqlRepository<GenreEntity>, IGenreRepository
    {
        private readonly IMapper _mapper;

        public GenreRepository(
            DbContext context,
            IMapper mapper,
            ILogger<GenreRepository> logger) : base(context, logger) 
        {
            _mapper = mapper;
        }

        public Task<bool> IsGenreUnique(Genre genre)
        {
            return _dbSet.AllAsync(g => g.Name != genre.Name || (g.Id == genre.Id && g.Name == genre.Name));
        }

        public Task CreateAsync(Genre item)
        {
            var entity = _mapper.Map<GenreEntity>(item);
            return CreateEntityAsync(entity);
        }

        public async Task<List<Genre>> GetAllAsync(string localizationCode = null, bool isIncludeDeleted = false)
        {
            var entities = await _dbSet
                .Include(x => x.Localizations.Where(y => y.Localization.CultureCode == localizationCode))
                .Include(x => x.Parent)
                    .ThenInclude(x => x.Localizations.Where(y => y.Localization.CultureCode == localizationCode))
                .Where(g => !g.IsDeleted || isIncludeDeleted)
                .ToListAsync();
            return _mapper.Map<List<Genre>>(entities);
        }

        public async Task<Genre> FindByNameIncludeAllLocalizationsAsync(string name, bool isIncludeDeleted = false)
        {
            GenreEntity genreEntity = await _dbSet
                .Include(g => g.Parent)
                .Include(g => g.Localizations)
                    .ThenInclude(l => l.Localization)
                .FirstOrDefaultAsync(g => g.Name == name && (!g.IsDeleted || isIncludeDeleted));

            return _mapper.Map<Genre>(genreEntity);
        }

        public async Task<Genre> FindByNameAsync(string name, string localizationCode = null, bool isIncludeDeleted = false)
        {
            GenreEntity genreEntity = await _dbSet
                .Include(g => g.Parent)
                    .ThenInclude(p => p.Localizations.Where(x => x.Localization.CultureCode == localizationCode))
                .Include(g => g.Localizations.Where(x => x.Localization.CultureCode == localizationCode))
                    .ThenInclude(l => l.Localization)
                .FirstOrDefaultAsync(g => g.Name == name && (!g.IsDeleted || isIncludeDeleted));

            return _mapper.Map<Genre>(genreEntity);
        }

        public async Task<Genre> FindByIdAsync(Guid id, bool isIncludeDeleted = false)
        {
            var entity = await FindEntityByIdAsync(id, isIncludeDeleted);
            return _mapper.Map<Genre>(entity);
        }

        public async Task<List<Genre>> GetChildGenresAsync(Guid id, bool isIncludeDeleted = false)
        {
            List<GenreEntity> genresEntities = await _dbSet
                .Include(g => g.Parent)
                .Where(g => g.ParentId == id && (!g.IsDeleted || isIncludeDeleted))
                .ToListAsync();

            return _mapper.Map<List<Genre>>(genresEntities);
        }

        public Task BatchUpdateParentsAsync(List<Genre> genres)
        {
            FormattableString query = BuildBatchUpdateQuery(genres);
            return _context.Database.ExecuteSqlInterpolatedAsync(query);
        }

        private static FormattableString BuildBatchUpdateQuery(List<Genre> genres)
        {
            StringBuilder query = new StringBuilder("UPDATE[Genres] SET[ParentId] = CASE[Id]");
            StringBuilder queryPredicate = new StringBuilder("WHERE [Id] IN(");

            object?[] arguments = new object[genres.Count * 2];
            for (int i = 0, argumentCounter = 0; i < genres.Count; i++, argumentCounter += 2)
            {
                query.Append(" WHEN {" + argumentCounter + "} THEN {" + (argumentCounter+1) + "}");
                queryPredicate.Append("{" +argumentCounter + "}, ");

                arguments[argumentCounter] = genres[i].Id;
                arguments[argumentCounter+1] = genres[i].ParentId;
            }

            query.Append(" ELSE [ParentId] END ");

            queryPredicate.Remove(queryPredicate.Length - 2, 2);
            queryPredicate.Append(");");

            query.Append(queryPredicate);

            return FormattableStringFactory.Create(query.ToString(), arguments);
        }

        public Task DeleteAsync(Genre item)
        {
            return DeleteByIdAsync(item.Id);
        }

        public async Task UpdateLocalizationAsync(Genre item, Guid chosenLocalization)
        {
            var editedLocalization = item.Localizations.FirstOrDefault(l => l.LocalizationId == chosenLocalization);
            var localizationEntity = _mapper.Map<GenreLocalizationEntity>(editedLocalization);

            try
            {
                _context.Entry(localizationEntity).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                _context.Entry(localizationEntity).State = EntityState.Added;
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(Genre item)
        {
            var entity = _mapper.Map<GenreEntity>(item);

            await UpdateEntityAsync(entity);
        }
    }
}
