using AutoMapper;
using GameStore.DAL.Abstractions.Interfaces;
using GameStore.DAL.Entities.Localizations;
using GameStore.DomainModels.Models.Localizations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.DAL.Repositories
{
    public class LocalizationRepository : ILocalizationRepository
    {
        private readonly DbSet<LocalizationEntity> _dbSet;
        private readonly DbContext _context;

        private readonly IMapper _mapper;

        public LocalizationRepository(
            DbContext context,
            IMapper mapper)
        {
            _context = context;
            _dbSet = context.Set<LocalizationEntity>();
            _mapper = mapper;
        }

        public async Task<List<Localization>> GetAllLocalizationsAsync()
        {
            var entities = await _dbSet.ToListAsync();
            return _mapper.Map<List<Localization>>(entities);
        }

        public async Task<Localization> GetLocalizationByIdAsync(Guid id)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(x => x.Id == id);
            return _mapper.Map<Localization>(entity);
        }

        public async Task<Localization> GetLocalizationByCultureCodeAsync(string cultureCode)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(x => x.CultureCode == cultureCode);
            return _mapper.Map<Localization>(entity);
        }
    }
}
