using AutoMapper;
using GameStore.DAL.Abstractions.Interfaces;
using GameStore.DAL.Entities;
using GameStore.DAL.Entities.Localizations;
using GameStore.DAL.Entities.MongoEntities;
using GameStore.DAL.Repositories.MongoDbRepositories.Interfaces;
using GameStore.DAL.Repositories.MsSqlDdRepositories.Interfaces;
using GameStore.DAL.SearchPipelines.GoodsSearchPipeline.Interfaces;
using GameStore.DAL.Settings;
using GameStore.DAL.Util.CacheManagers.Interfaces;
using GameStore.DAL.Workflows.Interfaces;
using GameStore.DomainModels.Exceptions;
using GameStore.DomainModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore.DAL.Repositories
{
    public class GoodsRepository : IGoodsRepository
    {
        private readonly IGameRepository _gameRepository;
        private readonly IProductMongoRepository _productRepository;
        private readonly ISupplierMongoRepository _supplierMongoRepository;
        private readonly ILegacyKeyRepository _legacyKeyRepository;
        private readonly IPublisherRepository _publisherRepository;

        private readonly IPublisherSupplierMappingWorkflow _publisherSupplierMappingWorkflow;

        private readonly IMapper _mapper;
        private readonly IGoodsSearchPipeline _goodsSearchPipeline;
        private readonly ICacheManager _cacheManager;

        private readonly MongoIntegrationSettings _mongoIntegrationSettings;

        public GoodsRepository(
            IGameRepository gameRepository,
            IProductMongoRepository productRepository,
            ISupplierMongoRepository supplierMongoRepository,
            ILegacyKeyRepository legacyKeyRepository,
            IPublisherRepository publisherRepository,
            IPublisherSupplierMappingWorkflow publisherSupplierMappingWorkflow,
            IMapper mapper,
            IGoodsSearchPipeline goodsSearchPipeline,
            ICacheManager cacheManager,
            MongoIntegrationSettings mongoIntegrationSettings)
        {
            _gameRepository=gameRepository;
            _productRepository=productRepository;
            _supplierMongoRepository=supplierMongoRepository;
            _legacyKeyRepository=legacyKeyRepository;
            _publisherRepository=publisherRepository;
            _publisherSupplierMappingWorkflow=publisherSupplierMappingWorkflow;
            _mapper=mapper;
            _goodsSearchPipeline=goodsSearchPipeline;
            _cacheManager=cacheManager;
            _mongoIntegrationSettings=mongoIntegrationSettings;
        }

        public async Task<bool> IsGoodsUnique(Goods goods)
        {
            GameEntity game = new GameEntity() { Key = goods.Key, Name = goods.Name };
            ProductMongoEntity product = new ProductMongoEntity() { ProductName = goods.Name, ProductKey = goods.Key };

            if (Guid.TryParse(goods.Id, out Guid gameId))
            {
                game.Id = gameId;

                var mappings = await _cacheManager.GetGoodsProductMappingsCacheAsync();

                var mapping = mappings.FirstOrDefault(x => x.GameKey == goods.Key);
                if (mapping != null)
                {
                    product.ProductId = mapping.ProductId;
                }
            }
            else if (int.TryParse(goods.Id, out int productId))
            {
                product.ProductId = productId;
            }

            Task<bool> isGameUniqueTask = _gameRepository.IsGameUnique(game);
            Task<bool> isProductUniqueTask = _productRepository.IsProductUnique(product);

            bool[] isGoodsUniqueTaskResult = await Task.WhenAll(isGameUniqueTask, isProductUniqueTask);

            foreach (bool isGoodsUnique in isGoodsUniqueTaskResult)
            {
                if (!isGoodsUnique)
                {
                    return false;
                }
            }

            return true;
        }

        public Task<List<Goods>> GetAllAsync(string localizationCulture = null, bool isIncludeDeleted = false)
        {
            var getGamesTask = _gameRepository.GetAllEntitiesIncludeLocalization(localizationCulture, isIncludeDeleted);
            var getProductsTask = _productRepository.GetAllAsync();

            return GetMergedGoodsAsync(getGamesTask, getProductsTask);
        }

        public async Task<List<Goods>> GetWithFiltersAsync(GoodsSearchRequest request, string localizationCulture = null, bool isIncludeDeleted = false)
        {
            var gamesSearchRequest = _mapper.Map<GamesSearchRequest>(request);
            var productsSearchRequest = _mapper.Map<ProductSearchRequest>(request);
            await FillExcludedProductKeysAsync(productsSearchRequest);

            await SetupGameSearchForTemporalPublishersAsync(request.Distributors, gamesSearchRequest);
            await SetupProductSearchForCategoriesAsync(request.Genres, productsSearchRequest);

            Task<List<GameEntity>> getSqlWithFilters = _gameRepository.GetWithFiltersAsync(gamesSearchRequest, localizationCulture);
            Task<List<ProductMongoEntity>> getMongoWithFilters = _productRepository.GetWithFiltersAsync(productsSearchRequest);

            List<Goods> goods = await GetMergedGoodsAsync(getSqlWithFilters, getMongoWithFilters);

            goods = _goodsSearchPipeline.Execute(goods, request);

            return goods;
        }

        public async Task<Goods> FindByKeyAsync(string key, string localizationCultureCode = null, bool isIncludeDeleted = false)
        {
            var goodsProductMappingsList = await _cacheManager.GetGoodsProductMappingsCacheAsync();
            if (goodsProductMappingsList.Any(x => x.GameKey == key))
            {
                var gameEntity = await _gameRepository.FindByKeyAsync(key, localizationCultureCode, isIncludeDeleted);

                return await SetupMigratedGameEntity(gameEntity);
            }

            var entity = await _gameRepository.FindByKeyAsync(key, localizationCultureCode, isIncludeDeleted);
            if (entity is null)
            {
                Guid gameId = await _legacyKeyRepository.FindGameIdAsync(key);
                if (gameId != Guid.Empty)
                {
                    entity = await _gameRepository.FindEntityByIdAsync(gameId, isIncludeDeleted);
                }
            }

            if (entity != null)
            {
                return await FillGamesWithSupplierAsync(entity);
            }

            var product = await _productRepository.FindByKeyAsync(key);
            return await SetupProductIncludes(product);
        }

        public async Task<Goods> FindByKeyIncludeAllLocalizationsAsync(string key, bool includeDeletedRows = false)
        {
            var goodsProductMappingsList = await _cacheManager.GetGoodsProductMappingsCacheAsync();
            if (goodsProductMappingsList.Any(x => x.GameKey == key))
            {
                var gameEntity = await _gameRepository.FindByKeyIncludeAllLocalizationsAsync(key, includeDeletedRows);

                return await SetupMigratedGameEntity(gameEntity);
            }

            var entity = await _gameRepository.FindByKeyIncludeAllLocalizationsAsync(key, includeDeletedRows);
            if (entity is null)
            {
                Guid gameId = await _legacyKeyRepository.FindGameIdAsync(key);
                if (gameId != Guid.Empty)
                {
                    entity = await _gameRepository.FindByIdIncludeAllLocalizationsAsync(gameId, includeDeletedRows);
                }
            }

            if (entity != null)
            {
                return await FillGamesWithSupplierAsync(entity);
            }

            var product = await _productRepository.FindByKeyAsync(key);
            return await SetupProductIncludes(product);
        }

        public async Task<Goods> FindAsync(string id, bool isIncludeDeleted = false, bool asNoTracking = false)
        {
            if (Guid.TryParse(id, out Guid gameId))
            {
                GameEntity game;
                if (asNoTracking)
                {
                    game = await _gameRepository.FindEntityByIdAsNoTrackingAsync(gameId, isIncludeDeleted);
                }
                else
                {
                    game = await _gameRepository.FindEntityByIdAsync(gameId, isIncludeDeleted);
                }

                var goodsProductMappingsList = await _cacheManager.GetGoodsProductMappingsCacheAsync();

                if (goodsProductMappingsList.Any(x => x.GameId == gameId))
                {
                    return await SetupMigratedGameEntity(game);
                }

                return await FillGamesWithSupplierAsync(game);
            }

            if (!int.TryParse(id, out int productId))
            {
                throw new ArgumentException("Not correct id format");
            }

            var product = await _productRepository.FindByIdAsync(productId);

            return await SetupProductIncludes(product);
        }

        public async Task<string> FindKeyByGoodsIdAsync(string id, bool isIncludeDeleted = false)
        {
            if (Guid.TryParse(id, out Guid gameId))
            {
                return await _gameRepository.FindKeyByGameIdAsync(gameId, isIncludeDeleted);
            }

            if (!int.TryParse(id, out int productId))
            {
                throw new ArgumentException("Not correct id format");
            }

            var product = await _productRepository.FindByIdAsync(productId);
            if (product is null)
            {
                throw new NotFoundException("Product with that key wasn't found");
            }

            return product.ProductKey;
        }

        public async Task<Guid> FindGameIdByKeyAsync(string key, bool isIncludeDeleted = false)
        {
            return await _gameRepository.FindGameIdByKeyAsync(key, isIncludeDeleted);
        }

        public async Task<List<Goods>> FindByGenreAsync(Guid genreId, string localizationCultureCode, bool isIncludeDeleted = false)
        {
            var genreCategoryList = await _cacheManager.GetGenreCategoryMappingCacheAsync();
            var mapping = genreCategoryList.FirstOrDefault(x => x.GenreId == genreId);
            if (mapping == null)
            {
                var games = await _gameRepository.FindByGenreAsync(genreId, localizationCultureCode, isIncludeDeleted);

                return _mapper.Map<List<Goods>>(games);
            }

            var findByCategoryTask = _productRepository.FindByCategoryAsync(mapping.CategoryId);
            var findByGenreTask = _gameRepository.FindByGenreAsync(genreId, isIncludeDeleted: isIncludeDeleted);

            return await GetMergedGoodsAsync(findByGenreTask, findByCategoryTask);
        }

        public async Task<List<Goods>> FindByPlatformTypeAsync(Guid platformId, string localizationCultureCode = null, bool isIncludeDeleted = false)
        {
            var games = await _gameRepository.FindByPlatformTypeAsync(platformId, localizationCultureCode, isIncludeDeleted);

            return _mapper.Map<List<Goods>>(games);
        }

        public async Task BatchUpdateUnitsInStockAsync(List<Goods> games)
        {
            var gamesList = new List<GameEntity>();
            var productsList = new List<ProductMongoEntity>();

            await SplitGoodsToGamesAndProducts(games, gamesList, productsList);

            var gameBatchUpdateTask = _gameRepository.BatchUpdateUnitsInStockAsync(gamesList);
            var productBatchUpdateTask = _productRepository.BatchUpdateUnitsInStockAsync(productsList);

            await Task.WhenAll(gameBatchUpdateTask, productBatchUpdateTask);
        }

        public Task IncreaseViewCountAsync(string id)
        {
            if (Guid.TryParse(id, out Guid gameId))
            {
                return _gameRepository.IncreaseViewCountAsync(gameId);
            }

            if (!int.TryParse(id, out int productId))
            {
                throw new ArgumentException("Not correct id format");
            }

            return _productRepository.IncreaseViewCountAsync(productId);
        }

        public Task<List<Goods>> FindByIdsAsync(List<string> ids, bool isIncludeDeleted = false, bool asNoTracking = false)
        {
            var productIds = new List<int?>();
            var gamesIds = new List<Guid>();

            foreach (var id in ids)
            {
                if (Guid.TryParse(id, out Guid gameId))
                {
                    gamesIds.Add(gameId);
                }

                if (int.TryParse(id, out int productId))
                {
                    productIds.Add(productId);
                }
            }

            Task<List<GameEntity>> getGamesByIdsTask;
            if (asNoTracking)
            {
                getGamesByIdsTask = _gameRepository.FindByIdsAsNoTrackingAsync(gamesIds);
            }
            else
            {
                getGamesByIdsTask = _gameRepository.FindEntitiesByIdsAsync(gamesIds, isIncludeDeleted);
            }
            var getProductsByIdsTask = _productRepository.FindByIdsAsync(productIds);

            return GetMergedGoodsAsync(getGamesByIdsTask, getProductsByIdsTask);
        }

        public async Task CreateAsync(Goods item)
        {
            var entity = _mapper.Map<GameEntity>(item);

            await FillGamePublisherAsync(item, entity);

            await _gameRepository.CreateEntityAsync(entity);
        }

        private async Task FillGamePublisherAsync(Goods item, GameEntity entity)
        {
            if (Guid.TryParse(item.DistributorId, out Guid publisherId))
            {
                entity.PublisherId = publisherId;
            }
            else if (int.TryParse(item.DistributorId, out int supplierId))
            {
                await SetupPublisherFromMappings(entity, supplierId);
            }
        }

        public Task UpdateAsync(Goods item)
        {
            if (Guid.TryParse(item.Id, out Guid gameId))
            {
                return UpdateGameAsync(item, gameId);
            }
            else if (int.TryParse(item.Id, out int productId))
            {
                return StartProductMigrationAsync(item, productId);
            }

            throw new ArgumentException("Not correct id format");
        }

        public async Task UpdateLocalizationAsync(Goods item, Guid chosenLocalization)
        {
            var localization = item.Localizations.First(x => x.LocalizationId == chosenLocalization);

            if (Guid.TryParse(item.Id, out Guid gameId))
            {
                localization.GoodsId = gameId;
            }
            else if (int.TryParse(item.Id, out int productId))
            {
                var mappings = await _cacheManager.GetGoodsProductMappingsCacheAsync();
                var mapping = mappings.FirstOrDefault(x => x.ProductId == productId);

                var product = await FindByKeyAsync(item.Key);

                await StartProductMigrationAsync(product, productId);

                Guid migratedProductId = await _gameRepository.FindGameIdByKeyAsync(product.Key);

                if (mapping != null && string.IsNullOrEmpty(mapping.GameKey))
                {
                    await SetupGenreForPartialMigratedProduct(item.Genres, migratedProductId);
                }

                localization.GoodsId = migratedProductId;
            }
            else
            {
                throw new ArgumentException("Not correct id format");
            }

            var localizationEntity = _mapper.Map<GameLocalizationEntity>(localization);
            await _gameRepository.UpdateLocalizationAsync(localizationEntity);
        }

        public Task RecoverAsync(Guid id)
        {
            return _gameRepository.RecoverAsync(id);
        }

        public Task SoftDeleteAsync(Guid id)
        {
            return _gameRepository.SoftDeleteAsync(id);
        }

        public async Task SoftDeleteAsync(string id)
        {
            if (Guid.TryParse(id, out Guid gameId))
            {
                await _gameRepository.SoftDeleteAsync(gameId);
            }
            else if (!int.TryParse(id, out int productId))
            {
                throw new ArgumentException("Not correct id format");
            }
            else
            {
                var product = await _productRepository.FindByIdAsync(productId);
                var productToMigrate = SetupProductForMigration(product);

                productToMigrate.IsDeleted = true;

                await _gameRepository.CreateEntityAsync(productToMigrate);
                await UpdateGoodsProductRelationAsync(product, productToMigrate.Id);
            }
        }

        public Task DeleteAsync(Guid id)
        {
            return _gameRepository.DeleteByIdAsync(id);
        }

        public Task DeleteAsync(Goods good)
        {
            if (Guid.TryParse(good.Id, out Guid id))
            {
                var game = _mapper.Map<GameEntity>(good);

                return _gameRepository.DeleteEntityAsync(game);
            }

            if (int.TryParse(good.Id, out int productId))
            {
                throw new InvalidOperationException("Can't delete product from NorthwindOnlineStore, use a soft delete instead");
            }

            throw new ArgumentException("Not correct id format");
        }

        public async Task<int> GetCountOfGoods(bool isIncludeDeleted = false)
        {
            int productsCount = await _productRepository.GetCountOfProductsAsync();
            int gamesCount = await _gameRepository.GetCountOfGamesAsync(isIncludeDeleted);

            return productsCount + gamesCount;
        }

        public async Task<List<GameGenre>> GetGamesGenresByGameIdAsync(Guid gameId)
        {
            List<GameGenreEntity> gameGenreEntities = await _gameRepository.GetGameGenresByGameIdAsync(gameId);

            return _mapper.Map<List<GameGenre>>(gameGenreEntities);
        }

        public async Task<List<GameGenre>> GetGamesGenresByGenreIdAsync(Guid genreId)
        {
            List<GameGenreEntity> gameGenreEntities = await _gameRepository.GetGameGenresByGenreIdAsync(genreId);

            return _mapper.Map<List<GameGenre>>(gameGenreEntities);
        }

        public Task DeleteRangeGamesGenresAsync(List<GameGenre> gameGenresToDelete)
        {
            var gameGenreEntities = _mapper.Map<List<GameGenreEntity>>(gameGenresToDelete);

            return _gameRepository.DeleteRangeGamesGenresAsync(gameGenreEntities);
        }

        public async Task<List<GamePlatformType>> GetGamePlatformByGameIdAsync(Guid gameId)
        {
            var gamePlatformTypeEntities = await _gameRepository.GetGamesPlatformsGameIdAsync(gameId);

            return _mapper.Map<List<GamePlatformType>>(gamePlatformTypeEntities);
        }

        public async Task<List<GamePlatformType>> GetGamePlatformByPlatformTypeIdAsync(Guid platformTypeId)
        {
            var gamePlatformTypeEntities = await _gameRepository.GetGamesPlatformsByPlatformTypeIdAsync(platformTypeId);

            return _mapper.Map<List<GamePlatformType>>(gamePlatformTypeEntities);
        }

        public Task DeleteRangeGamesPlatformTypesAsync(List<GamePlatformType> gameGenresToDelete)
        {
            var gameGenreEntities = _mapper.Map<List<GamePlatformTypeEntity>>(gameGenresToDelete);

            return _gameRepository.DeleteRangeGamesPlatformsAsync(gameGenreEntities);
        }

        public Task CreateRangeGamesGenresAsync(List<GameGenre> newGameGanreList)
        {
            var gameGenreEntities = _mapper.Map<List<GameGenreEntity>>(newGameGanreList);

            return _gameRepository.CreateRangeGamesGenresAsync(gameGenreEntities);
        }

        public Task CreateRangeGamesPlatformTypesAsync(List<GamePlatformType> newGamePlatformTypeList)
        {
            var gamesPlatformTypesEntities = _mapper.Map<List<GamePlatformTypeEntity>>(newGamePlatformTypeList);

            return _gameRepository.CreateRangeGamesPlatformTypesAsync(gamesPlatformTypesEntities);
        }

        private async Task<Goods> FillGamesWithSupplierAsync(GameEntity entity)
        {
            var goods = _mapper.Map<Goods>(entity);
            var publishersSuppliersMappingsList = await _cacheManager.GetPublisherSupplierMappingsCacheAsync();

            var mapping = publishersSuppliersMappingsList.FirstOrDefault(x => x.PublisherId == entity.PublisherId);
            if (mapping != null)
            {
                goods.DistributorId = mapping.SupplierId.ToString();
                await SetupProductSupplier(mapping.SupplierId, goods);
            }

            return goods;
        }

        private async Task UpdateGoodsProductRelationAsync(ProductMongoEntity product, Guid id)
        {
            List<GoodsProductMapping> goodsProductMappingsList = await _cacheManager.GetGoodsProductMappingsCacheAsync();
            GoodsProductMapping mapping = goodsProductMappingsList.FirstOrDefault(m => m.GameId == id);

            if (mapping == null)
            {
                mapping = _mapper.Map<GoodsProductMapping>(product);
                mapping.GameId = id;
                mapping.GameKey = product.ProductKey;

                await _cacheManager.AddGoodsProductMappingCache(mapping);
            }
            else
            {
                Guid mappingId = mapping.Id;

                mapping = _mapper.Map<GoodsProductMapping>(product);
                mapping.GameId = id;
                mapping.GameKey = product.ProductKey;

                mapping.Id = mappingId;
                await _cacheManager.UpdateGoodsProductMappingAsync(mapping);
            }
        }

        private async Task SplitGoodsToGamesAndProducts(List<Goods> games, List<GameEntity> gamesList, List<ProductMongoEntity> productsList)
        {
            foreach (var goods in games)
            {
                if (Guid.TryParse(goods.Id, out Guid gameId))
                {
                    await HandleMigratedProducts(productsList, goods, gameId);

                    var game = _mapper.Map<GameEntity>(goods);

                    game.Id = gameId;

                    gamesList.Add(game);
                }
                else if (int.TryParse(goods.Id, out int productId))
                {
                    var product = _mapper.Map<ProductMongoEntity>(goods);

                    product.ProductId = productId;

                    productsList.Add(product);
                }
            }
        }

        private async Task HandleMigratedProducts(List<ProductMongoEntity> productsList, Goods goods, Guid gameId)
        {
            var goodsProductMappingsList = await _cacheManager.GetGoodsProductMappingsCacheAsync();
            var mapping = goodsProductMappingsList.FirstOrDefault(x => x.GameId == gameId && x.IsFullyMigrated);

            if (mapping != null)
            {
                var product = _mapper.Map<ProductMongoEntity>(goods);
                product.ProductId = mapping.ProductId;
                productsList.Add(product);
            }
        }

        private async Task StartProductMigrationAsync(Goods item, int productId)
        {
            var product = await _productRepository.FindByIdAsync(productId);
            var productToMigrate = SetupProductForMigration(product, item);

            await SetupDistributorForMigratedProduct(item.DistributorId, product, productToMigrate);

            await FinishProductMigrationAsync(productId, productToMigrate);

            await UpdateGoodsProductRelationAsync(product, productToMigrate.Id);
        }

        private async Task FinishProductMigrationAsync(int productId, GameEntity productToMigrate)
        {
            var goodsProductMappingsList = await _cacheManager.GetGoodsProductMappingsCacheAsync();
            var mapping = goodsProductMappingsList.FirstOrDefault(m => m.ProductId == productId);

            if (mapping == null)
            {
                await _gameRepository.CreateEntityAsync(productToMigrate);
            }
            else
            {
                productToMigrate.Id = mapping.GameId;
                await _gameRepository.UpdateEntityAsync(productToMigrate);
            }
        }

        private async Task SetupDistributorForMigratedProduct(string distributorId, ProductMongoEntity product, GameEntity productToMigrate)
        {
            if (int.TryParse(distributorId, out int supplierId))
            {
                var mapping = await _publisherSupplierMappingWorkflow.GetMappingAsync(supplierId);

                if (mapping is null)
                {
                    mapping = await _publisherSupplierMappingWorkflow.CreateMappingAsync(supplierId);
                }

                productToMigrate.PublisherId = mapping.PublisherId;
            }
            else if (Guid.TryParse(distributorId, out Guid publisherId))
            {
                productToMigrate.PublisherId = publisherId;
            }
            else
            {
                product.SupplierId = null;
            }
        }

        private async Task UpdateGameAsync(Goods item, Guid gameId)
        {
            string legacyKey = await FindKeyByGoodsIdAsync(item.Id);

            var entity = _mapper.Map<GameEntity>(item);
            entity.Id = gameId;

            await SetupDistributorForGameAsync(item, entity);

            await _gameRepository.UpdateEntityAsync(entity);

            if (legacyKey == item.Key)
            {
                return;
            }

            await _legacyKeyRepository.CreateAsync(legacyKey, gameId);

            var mappings = await _cacheManager.GetGoodsProductMappingsCacheAsync();

            var mapping = mappings.FirstOrDefault(x => x.GameKey == legacyKey);
            if (mapping != null)
            {
                mapping.GameKey = entity.Key;
                await _cacheManager.UpdateGoodsProductMappingAsync(mapping);
            }
        }

        private async Task SetupDistributorForGameAsync(Goods item, GameEntity entity)
        {
            if (int.TryParse(item.DistributorId, out int supplierId))
            {
                await SetupPublisherFromMappings(entity, supplierId);
            }
            else if (Guid.TryParse(item.DistributorId, out Guid publisherId))
            {
                entity.PublisherId = publisherId;
            }
        }

        private async Task SetupPublisherFromMappings(GameEntity entity, int supplierId)
        {
            var mapping = await _publisherSupplierMappingWorkflow.GetMappingAsync(supplierId);
            if (mapping == null)
            {
                mapping = await _publisherSupplierMappingWorkflow.CreateMappingAsync(supplierId);
            }

            entity.PublisherId = mapping.PublisherId;
        }

        private async Task<List<Goods>> GetMergedGoodsAsync(Task<List<GameEntity>> sqlGetTask, Task<List<ProductMongoEntity>> MongoGetTask)
        {
            await Task.WhenAll(sqlGetTask, MongoGetTask);

            var games = await sqlGetTask;
            var products = await MongoGetTask;

            List<Goods> goodsList = await SetupFillGamesWithMongoEntitiesAsync(games);

            var mongoGoodsList = await SetupProductsIncludes(products);

            goodsList.AddRange(mongoGoodsList);
            return goodsList;
        }

        private async Task<List<Goods>> SetupFillGamesWithMongoEntitiesAsync(List<GameEntity> games)
        {
            var goodsProductMappingsList = await _cacheManager.GetGoodsProductMappingsCacheAsync();
            var migratedGames = games.Where(x => goodsProductMappingsList.Any(c => c.GameKey == x.Key)).ToList();

            games.RemoveAll(g => migratedGames.Any(mg => mg.Key == g.Key));

            var goodsList = new List<Goods>();
            foreach (var game in games)
            {
                var goods = await FillGamesWithSupplierAsync(game);
                goodsList.Add(goods);
            }

            foreach (var item in migratedGames)
            {
                var goods = await SetupMigratedGameEntity(item);
                goodsList.Add(goods);
            }

            return goodsList;
        }

        private async Task<List<Goods>> SetupProductsIncludes(List<ProductMongoEntity> products)
        {
            List<Goods> goodsList = new List<Goods>();
            foreach (var product in products)
            {
                var goods = await SetupProductIncludes(product);
                goodsList.Add(goods);
            }

            return goodsList;
        }

        private async Task<Goods> SetupMigratedGameEntity(GameEntity game)
        {
            var goods = _mapper.Map<Goods>(game);

            var goodsProductMappingsList = await _cacheManager.GetGoodsProductMappingsCacheAsync();
            var mappings = goodsProductMappingsList.First(x => x.GameKey == game.Key);

            if (mappings.ProductId.HasValue)
            {
                await SetupProductUnitsInStock(mappings.ProductId.Value, goods);
            }

            var publishersSuppliersMappingsList = await _cacheManager.GetPublisherSupplierMappingsCacheAsync();
            var pubSubMapping = publishersSuppliersMappingsList.FirstOrDefault(x => x.PublisherId == game.PublisherId);

            if (pubSubMapping != null)
            {
                await SetupProductSupplier(pubSubMapping.SupplierId, goods);
            }

            return goods;
        }

        private async Task SetupProductUnitsInStock(int productId, Goods goods)
        {
            var product = await _productRepository.FindByIdAsync(productId);
            if (product != null && product.UnitsInStock.HasValue)
            {
                goods.UnitsInStock = (short)product.UnitsInStock.Value;
            }
        }

        private async Task<Goods> SetupProductIncludes(ProductMongoEntity product)
        {
            if (product is null)
            {
                return null;
            }

            var goods = _mapper.Map<Goods>(product);
            goods.DateOfAdding = _mongoIntegrationSettings.NorthwindOnlineStoreDbConnectionDate;

            if (product.CategoryId.HasValue)
            {
                await SetupProductCategory(product.CategoryId.Value, goods);
            }

            if (product.SupplierId.HasValue)
            {
                await SetupProductSupplier(product.SupplierId.Value, goods);
            }

            return goods;
        }

        private async Task SetupProductSupplier(int supplierId, Goods goods)
        {
            var supplier = await _supplierMongoRepository.FindByIdAsync(supplierId);
            goods.Distributor = _mapper.Map<Distributor>(supplier);
            goods.DistributorId = supplierId.ToString();

            var mapping = await _publisherSupplierMappingWorkflow.GetMappingAsync(supplierId);
            if (mapping != null)
            {
                var publisher = await _publisherRepository.FindEntityByIdAsync(mapping.PublisherId);
                goods.Distributor.UserId = publisher.UserId;
            }
        }

        private async Task SetupProductCategory(int categoryId, Goods goods)
        {
            var genreCategoryList = await _cacheManager.GetGenreCategoryMappingCacheAsync();

            var genreCategory = genreCategoryList.FirstOrDefault(x => x.CategoryId == categoryId);
            if (genreCategory == null)
            {
                return;
            }

            goods.Genres.Add(genreCategory.Genre);
        }

        private async Task SetupGenreForPartialMigratedProduct(List<Genre> genres, Guid migratedProductId)
        {
            var gameGenre = new List<GameGenreEntity>();
            foreach (var genre in genres)
            {
                gameGenre.Add(new GameGenreEntity
                {
                    GameId = migratedProductId,
                    GenreId = genre.Id
                });
            }

            await _gameRepository.CreateRangeGamesGenresAsync(gameGenre);
        }

        private GameEntity SetupProductForMigration(ProductMongoEntity product, Goods goods = null)
        {
            GameEntity game;
            Guid newGuid = Guid.NewGuid();
            if (goods is null)
            {
                game = _mapper.Map<GameEntity>(product);
                game.Id = newGuid;
            }
            else
            {
                goods.Id = newGuid.ToString();
                game = _mapper.Map<GameEntity>(goods);
            }

            return game;
        }

        private async Task FillExcludedProductKeysAsync(ProductSearchRequest productsSearchRequest)
        {
            var mappings = await _cacheManager.GetGoodsProductMappingsCacheAsync();
            productsSearchRequest.ExcludedProductKeys = mappings.Select(x => x.GameKey).ToList();
        }

        private async Task SetupProductSearchForCategoriesAsync(List<Guid> chosedGenres, ProductSearchRequest productsSearchRequest)
        {
            foreach (var genreId in chosedGenres)
            {
                var genreCategoryList = await _cacheManager.GetGenreCategoryMappingCacheAsync();
                var mapping = genreCategoryList.FirstOrDefault(x => x.GenreId == genreId);

                if (mapping != null)
                {
                    productsSearchRequest.Categories.Add(mapping.CategoryId.ToString());
                }
            }
        }

        private async Task SetupGameSearchForTemporalPublishersAsync(List<string> chosedDistributors,
                                                                     GamesSearchRequest gamesSearchRequest)
        {
            foreach (var item in chosedDistributors)
            {
                if (int.TryParse(item, out int supplierId))
                {
                    var publishersSuppliersMappingsList = await _cacheManager.GetPublisherSupplierMappingsCacheAsync();
                    var mapping = publishersSuppliersMappingsList.FirstOrDefault(x => x.SupplierId == supplierId);

                    if (mapping != null)
                    {
                        gamesSearchRequest.Publishers.Add(mapping.PublisherId.ToString());
                    }
                }
            }
        }
    }
}
