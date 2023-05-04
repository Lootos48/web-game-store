using AutoMapper;
using GameStore.DAL.Abstractions.Interfaces;
using GameStore.DAL.Entities;
using GameStore.DomainModels.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.DAL.Repositories
{
    public class PublisherSupplierRepository : IPublisherSupplierRepository
    {
        protected readonly DbContext _context;
        protected readonly DbSet<PublisherSupplierMappingEntity> _dbSet;
        protected readonly IMapper _mapper;
        protected readonly ILogger<PublisherSupplierRepository> _logger;

        public PublisherSupplierRepository(
            DbContext context,
            IMapper mapper,
            ILogger<PublisherSupplierRepository> logger)
        {
            _context = context;
            _dbSet = context.Set<PublisherSupplierMappingEntity>();
            _mapper = mapper;
            _logger = logger;
        }

        public Task CreateAsync(PublisherSupplierMapping newMapping)
        {
            var mappingEntity = _mapper.Map<PublisherSupplierMappingEntity>(newMapping);

            _dbSet.Add(mappingEntity);
            return _context.SaveChangesAsync();
        }

        public async Task<List<PublisherSupplierMapping>> GetAllAsync()
        {
            List<PublisherSupplierMappingEntity> entityList = await _dbSet.ToListAsync();

            return _mapper.Map<List<PublisherSupplierMapping>>(entityList);
        }

        public async Task<PublisherSupplierMapping> FindAsync(Guid publisherId, int supplierId)
        {
            var mapping = await _dbSet.FirstOrDefaultAsync(x => x.PublisherId == publisherId && x.SupplierId == supplierId);

            return _mapper.Map<PublisherSupplierMapping>(mapping);
        }

        public async Task<PublisherSupplierMapping> GetByPublisherAsync(Guid publisherId)
        {
            var mapping = await _dbSet.FirstOrDefaultAsync(x => x.PublisherId == publisherId);

            return _mapper.Map<PublisherSupplierMapping>(mapping);
        }

        public async Task<PublisherSupplierMapping> GetBySupplierIdAsync(int supplierId)
        {
            var mapping = await _dbSet.FirstOrDefaultAsync(x => x.SupplierId == supplierId);

            return _mapper.Map<PublisherSupplierMapping>(mapping);
        }
    }
}
