using AutoMapper;
using GameStore.DAL.Abstractions.Interfaces;
using GameStore.DAL.Entities;
using GameStore.DomainModels.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.DAL.Repositories
{
    public class GoodsProductRepository : IGoodsProductRepository
    {
        protected readonly DbContext _context;
        protected readonly DbSet<GoodsProductMappingEntity> _dbSet;
        protected readonly IMapper _mapper;
        protected readonly ILogger<GoodsProductRepository> _logger;

        public GoodsProductRepository(
            DbContext context,
            IMapper mapper,
            ILogger<GoodsProductRepository> logger)
        {
            _context = context;
            _dbSet = context.Set<GoodsProductMappingEntity>();
            _mapper = mapper;
            _logger = logger;
        }

        public Task CreateAsync(GoodsProductMapping newMapping)
        {
            var mappingEntity = _mapper.Map<GoodsProductMappingEntity>(newMapping);

            _dbSet.Add(mappingEntity);
            return _context.SaveChangesAsync();
        }

        public async Task<List<GoodsProductMapping>> GetAllAsync()
        {
            List<GoodsProductMappingEntity> entityList = await _dbSet.ToListAsync();

            return _mapper.Map<List<GoodsProductMapping>>(entityList);
        }

        public async Task<GoodsProductMapping> GetByGameKeyAsync(string gameKey)
        {
            GoodsProductMappingEntity entity = await _dbSet
                .FirstOrDefaultAsync(gp => gp.GameKey == gameKey);

            return _mapper.Map<GoodsProductMapping>(entity);
        }

        public async Task<GoodsProductMapping> GetByGameIdAsync(Guid id)
        {
            GoodsProductMappingEntity entity = await _dbSet
                .FirstOrDefaultAsync(gp => gp.GameId == id);

            return _mapper.Map<GoodsProductMapping>(entity);
        }

        public Task DeleteRangeAsync(List<GoodsProductMapping> goodsProducts)
        {
            List<GoodsProductMappingEntity> entityList = _mapper.Map<List<GoodsProductMappingEntity>>(goodsProducts);

            FormattableString query = BuildRangeDeletingQuery(entityList);

            return _context.Database.ExecuteSqlInterpolatedAsync(query);
        }

        private static FormattableString BuildRangeDeletingQuery(List<GoodsProductMappingEntity> goodsProductToDelete)
        {
            object[] arguments = new object[goodsProductToDelete.Count * 2];
            StringBuilder query = new StringBuilder("DELETE FROM [GoodsProduct] WHERE ");

            for (int i = 0; i < goodsProductToDelete.Count; i++)
            {
                query.Append("[GameKey] = {" + i + "} OR");
                arguments[i] = goodsProductToDelete[i].GameKey;
            }

            query.Remove(query.Length - 2, 2);
            query.Append(";");

            return FormattableStringFactory.Create(query.ToString(), arguments);
        }

        public Task UpdateAsync(GoodsProductMapping mapping)
        {
            GoodsProductMappingEntity entity = _mapper.Map<GoodsProductMappingEntity>(mapping);

            _context.Entry(entity).State = EntityState.Modified;
            return _context.SaveChangesAsync();
        }
    }
}
