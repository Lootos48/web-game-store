using GameStore.DAL.Abstractions.Interfaces;
using GameStore.DomainModels.Attributes;
using GameStore.DomainModels.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace GameStore.PL.HostedServices
{
    [Singleton]
    public class LaunchCacheService : IHostedService
    {
        private CancellationTokenSource _cancellationTokenSource;

        private readonly IMemoryCache _cache;

        private readonly IGoodsProductRepository _goodsProductRepository;
        private readonly IGenreCategoryRepository _genreCategoryRepository;
        private readonly IPublisherSupplierRepository _publisherSupplierRepository;

        public LaunchCacheService(
            IMemoryCache cache,
            IGoodsProductRepository goodsProductRepository,
            IGenreCategoryRepository genreCategoryRepository,
            IPublisherSupplierRepository publisherSupplierRepository)
        {
            _cache = cache;

            _goodsProductRepository = goodsProductRepository;
            _genreCategoryRepository = genreCategoryRepository;
            _publisherSupplierRepository = publisherSupplierRepository;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource = new CancellationTokenSource();

            await SetupCacheAsync();
        }

        private async Task SetupCacheAsync()
        {
            var goodsProductList = await _goodsProductRepository.GetAllAsync();
            var genreCategoryList = await _genreCategoryRepository.GetAllAsync();
            var publisherSupplierList = await _publisherSupplierRepository.GetAllAsync();

            _cache.Set(typeof(GoodsProductMapping), goodsProductList);
            _cache.Set(typeof(GenreCategoryMapping), genreCategoryList);
            _cache.Set(typeof(PublisherSupplierMapping), publisherSupplierList);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource.Cancel();

            return Task.CompletedTask;
        }
    }
}
