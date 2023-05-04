using AutoMapper;
using GameStore.DAL.Abstractions.Interfaces;
using GameStore.DAL.Entities;
using GameStore.DAL.Entities.Localizations;
using GameStore.DAL.Repositories.MsSqlDdRepositories;
using GameStore.DomainModels.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore.DAL.Repositories
{
    public class PlatformTypeRepository : SqlRepository<PlatformTypeEntity>, IPlatformTypeRepository
    {
        private readonly IMapper _mapper;

        public PlatformTypeRepository(
            DbContext context,
            IMapper mapper,
            ILogger<PlatformTypeRepository> logger) : base(context, logger) 
        {
            _mapper = mapper;
        }

        public Task<bool> IsPlatformTypeUnique(PlatformType platformType)
        {
            return _dbSet.AllAsync(p => p.Type != platformType.Type || (p.Id == platformType.Id && p.Type == platformType.Type));
        }

        public Task CreateAsync(PlatformType item)
        {
            var entity = _mapper.Map<PlatformTypeEntity>(item);
            return CreateEntityAsync(entity);
        }

        public async Task<List<PlatformType>> GetAllAsync(bool isIncludeDeleted = false)
        {
            var entities = await GetAllEntitiesAsync();
            return _mapper.Map<List<PlatformType>>(entities);
        }

        public async Task<List<PlatformType>> GetAllAsync(string localizationCode, bool includeDeleted = false)
        {
            var entities = await _dbSet
                .Include(p => p.Localizations.Where(pl => pl.Localization.CultureCode == localizationCode))
                    .Where(p => !p.IsDeleted || includeDeleted)
                .ToListAsync();

            return _mapper.Map<List<PlatformType>>(entities);
        }

        public async Task<PlatformType> FindByIdAsync(Guid id, bool isIncludeDeleted = false)
        {
            var entity = await FindEntityByIdAsync(id, isIncludeDeleted);
            return _mapper.Map<PlatformType>(entity);
        }

        public async Task<PlatformType> FindByTypeAsync(string type, string localizationCultureCode = null, bool includeDeleted = false)
        {
            PlatformTypeEntity platformTypeEntity = await _dbSet
                .Include(p => p.Localizations.Where(pl => pl.Localization.CultureCode == localizationCultureCode))
                .FirstOrDefaultAsync(p => p.Type == type && (!p.IsDeleted || includeDeleted));

            return _mapper.Map<PlatformType>(platformTypeEntity);
        }

        public async Task<PlatformType> FindByTypeIncludeAllLocalizationsAsync(string type, bool includeDeleted = false)
        {
            PlatformTypeEntity platformTypeEntity = await _dbSet
                .Include(p => p.Localizations)
                    .ThenInclude(l => l.Localization)
                .FirstOrDefaultAsync(p => p.Type == type && (!p.IsDeleted || includeDeleted));

            return _mapper.Map<PlatformType>(platformTypeEntity);
        }

        public async Task UpdateLocalizationAsync(PlatformType item, Guid chosenLocalization)
        {
            var editedLocalization = item.Localizations.FirstOrDefault(x => x.LocalizationId == chosenLocalization);

            if (editedLocalization is null)
            {
                return;
            }

            var localizationEntity = _mapper.Map<PlatformTypeLocalizaitionEntity>(editedLocalization);

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

        public Task UpdateAsync(PlatformType item)
        {
            var entity = _mapper.Map<PlatformTypeEntity>(item);
            return UpdateEntityAsync(entity);
        }

        public Task DeleteAsync(PlatformType item)
        {
            return DeleteByIdAsync(item.Id);
        }
    }
}
