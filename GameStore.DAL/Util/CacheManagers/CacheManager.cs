using GameStore.DAL.Abstractions.Interfaces;
using GameStore.DAL.Util.CacheManagers.Interfaces;
using GameStore.DomainModels.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.DAL.Util.CacheManagers
{
    public class CacheManager : ICacheManager
    {
        private readonly IGoodsProductRepository _goodsProductRepository;
        private readonly IGenreCategoryRepository _genreCategoryRepository;
        private readonly IPublisherSupplierRepository _publisherSupplierRepository;

        private readonly IMemoryCache _cache;

        public CacheManager(
            IGoodsProductRepository goodsProductRepository,
            IGenreCategoryRepository genreCategoryRepository,
            IPublisherSupplierRepository publisherSupplierRepository,
            IMemoryCache cache)
        {
            _goodsProductRepository = goodsProductRepository;
            _genreCategoryRepository = genreCategoryRepository;
            _publisherSupplierRepository = publisherSupplierRepository;
            _cache = cache;
        }

        public async Task AddPublisherSupplierMappingCache(PublisherSupplierMapping newMapping)
        {
            var publisherSupplierMappings = await GetPublisherSupplierMappingsCacheAsync();
            publisherSupplierMappings.Add(newMapping);

            await _publisherSupplierRepository.CreateAsync(newMapping);
            _cache.Set(typeof(GenreCategoryMapping), publisherSupplierMappings);
        }

        public async Task AddGoodsProductMappingCache(GoodsProductMapping newMapping)
        {
            var goodsProductMappings = await GetGoodsProductMappingsCacheAsync();
            goodsProductMappings.Add(newMapping);

            await _goodsProductRepository.CreateAsync(newMapping);
            _cache.Set(typeof(GenreCategoryMapping), goodsProductMappings);
        }

        public async Task AddGenreCategoryMappingCache(GenreCategoryMapping newMapping)
        {
            var _genreCategoryMappings = await GetGenreCategoryMappingCacheAsync();
            _genreCategoryMappings.Add(newMapping);

            await _genreCategoryRepository.CreateAsync(newMapping);
            _cache.Set(typeof(GenreCategoryMapping), _genreCategoryMappings);
        }

        public async Task UpdatePublisherSupplierCategoryMappingsCacheAsync()
        {
            var publisherSupplierMappings = await _publisherSupplierRepository.GetAllAsync();
            _cache.Set(typeof(PublisherSupplierMapping), publisherSupplierMappings);
        }

        public async Task UpdateGenreCategoryMappingsCacheAsync()
        {
            var genreCategoryMappings = await _genreCategoryRepository.GetAllAsync();
            _cache.Set(typeof(GenreCategoryMapping), genreCategoryMappings);
        }

        public async Task UpdateGoodsProductMappingsCacheAsync()
        {
            var goodsProductMappings = await _goodsProductRepository.GetAllAsync();
            _cache.Set(typeof(GoodsProductMapping), goodsProductMappings);
        }

        public async Task<List<GoodsProductMapping>> GetGoodsProductMappingsCacheAsync()
        {
            if (_cache.TryGetValue(typeof(GoodsProductMapping), out List<GoodsProductMapping> goodsProductMappings))
            {
                return goodsProductMappings;
            }

            await UpdateGoodsProductMappingsCacheAsync();
            return _cache.Get<List<GoodsProductMapping>>(typeof(GoodsProductMapping));
        }

        public async Task<List<GenreCategoryMapping>> GetGenreCategoryMappingCacheAsync()
        {
            if (_cache.TryGetValue(typeof(GenreCategoryMapping), out List<GenreCategoryMapping> genreCategoryMappings))
            {
                return genreCategoryMappings;
            }

            await UpdateGenreCategoryMappingsCacheAsync();
            return _cache.Get<List<GenreCategoryMapping>>(typeof(GenreCategoryMapping));
        }

        public async Task<List<PublisherSupplierMapping>> GetPublisherSupplierMappingsCacheAsync()
        {
            if (_cache.TryGetValue(typeof(PublisherSupplierMapping), out List<PublisherSupplierMapping> publisherSupplierMappings))
            {
                return publisherSupplierMappings;
            }

            await UpdatePublisherSupplierCategoryMappingsCacheAsync();
            return _cache.Get<List<PublisherSupplierMapping>>(typeof(PublisherSupplierMapping));
        }

        public async Task UpdateGoodsProductMappingAsync(GoodsProductMapping mapping)
        {
            await _goodsProductRepository.UpdateAsync(mapping);
            await UpdateGoodsProductMappingsCacheAsync();
        }
    }
}
