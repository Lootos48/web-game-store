using GameStore.DomainModels.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.DAL.Abstractions.Interfaces
{
    public interface IGoodsProductRepository
    {
        Task DeleteRangeAsync(List<GoodsProductMapping> goodsProducts);

        Task<GoodsProductMapping> GetByGameIdAsync(Guid id);

        Task<GoodsProductMapping> GetByGameKeyAsync(string gameKey);

        Task<List<GoodsProductMapping>> GetAllAsync();

        Task UpdateAsync(GoodsProductMapping mapping);

        Task CreateAsync(GoodsProductMapping newMapping);
    }
}