using GameStore.DomainModels.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.DAL.Util.CacheManagers.Interfaces
{
    public interface ICacheManager
    {
        Task AddGenreCategoryMappingCache(GenreCategoryMapping newMapping);

        Task AddGoodsProductMappingCache(GoodsProductMapping newMapping);

        Task AddPublisherSupplierMappingCache(PublisherSupplierMapping newMapping);

        Task<List<GenreCategoryMapping>> GetGenreCategoryMappingCacheAsync();

        Task<List<GoodsProductMapping>> GetGoodsProductMappingsCacheAsync();

        Task<List<PublisherSupplierMapping>> GetPublisherSupplierMappingsCacheAsync();

        Task UpdateGenreCategoryMappingsCacheAsync();

        Task UpdateGoodsProductMappingsCacheAsync();

        Task UpdatePublisherSupplierCategoryMappingsCacheAsync();

        Task UpdateGoodsProductMappingAsync(GoodsProductMapping mapping);
    }
}