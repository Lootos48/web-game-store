using AutoMapper;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Settings;
using GameStore.DAL.Abstractions.Interfaces;
using GameStore.DomainModels.Exceptions;
using GameStore.DomainModels.Models;
using GameStore.DomainModels.Models.CreateModels;
using GameStore.DomainModels.Models.EditModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore.BLL.Services
{
    public class GoodsService : IGoodsService
    {
        private readonly IGoodsRepository _goodsRepository;
        private readonly IMapper _mapper;
        private readonly DownloadingFileSettings _downloadingFileConfiguration;

        public GoodsService(
            IGoodsRepository gameRepository,
            DownloadingFileSettings downloadingFileConfiguration,
            IMapper mapper)
        {
            _goodsRepository = gameRepository;
            _downloadingFileConfiguration = downloadingFileConfiguration;
            _mapper = mapper;
        }

        public async Task<Goods> GetGoodsByIdAsync(Guid id)
        {
            Goods foundGame = await _goodsRepository.FindAsync(id.ToString());
            if (foundGame is null)
            {
                throw new NotFoundException($"Game with id {id} wasn't found");
            }

            return foundGame;
        }

        public Task<Goods> GetGoodsByKeyAsync(string key, string localizationCultureCode = null, bool includeDeletedRows = false)
        {
            return _goodsRepository.FindByKeyAsync(key, localizationCultureCode, includeDeletedRows);
        }

        public Task<Goods> GetGoodsByKeyIncludeAllLocalizationsAsync(string key, bool includeDeletedRows = false)
        {
            return _goodsRepository.FindByKeyIncludeAllLocalizationsAsync(key, includeDeletedRows);
        }

        public async Task<string> CreateGoodsAsync(CreateGoodsRequest gameToCreate)
        {
            gameToCreate.Id = Guid.NewGuid();
            gameToCreate.DateOfAdding = DateTime.UtcNow;

            Goods newGame = _mapper.Map<Goods>(gameToCreate);

            if (string.IsNullOrEmpty(gameToCreate.Key))
            {
                newGame.Key = GenerateKey(newGame.Name);
            }

            bool isUnique = await _goodsRepository.IsGoodsUnique(newGame);
            if (!isUnique)
            {
                throw new NotUniqueException("Game with that key is already exist");
            }

            await _goodsRepository.CreateAsync(newGame);
            return newGame.Key;
        }

        public Task EditGoodsAsync(EditGoodsRequest gameToEdit)
        {
            Goods changedGame = _mapper.Map<Goods>(gameToEdit);
            if (gameToEdit.ChosedLocalization.HasValue)
            {
                return _goodsRepository.UpdateLocalizationAsync(changedGame, gameToEdit.ChosedLocalization.Value);
            }

            return UpdateOriginalGoodsAsync(changedGame);
        }

        public async Task RecoverGoodsAsync(Guid id)
        {
            try
            {
                await _goodsRepository.RecoverAsync(id);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException($"Game with id {id} wasn't found");
            }
        }

        public async Task SoftDeleteGoodsAsync(string id)
        {
            try
            {
                await _goodsRepository.SoftDeleteAsync(id);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException($"Game with id {id} wasn't found");
            }
        }

        public async Task HardDeleteGoodsAsync(Guid id)
        {
            try
            {
                await _goodsRepository.DeleteAsync(id);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException($"Game with id {id} wasn't found");
            }
        }

        public async Task IncreaseViewCountAsync(string gameId)
        {
            await _goodsRepository.IncreaseViewCountAsync(gameId);
        }

        public Task<List<Goods>> GetGoodsAsync(string localizationCultureCode = null)
        {
            return _goodsRepository.GetAllAsync(localizationCultureCode);
        }

        public Task<List<Goods>> GetFilteredGoodsAsync(GoodsSearchRequest filters, string localizationCultureCode = null)
        {
            return _goodsRepository.GetWithFiltersAsync(filters, localizationCultureCode);
        }

        public Task AddGenreToGoodsAsync(Guid gameId, List<Genre> genres)
        {
            ValidateChangeForGame(gameId, genres);

            List<GameGenre> newGameGanreList = new List<GameGenre>();

            foreach (var g in genres)
            {
                GameGenre gameGenre = new GameGenre()
                {
                    GameId = gameId,
                    GenreId = g.Id
                };
                newGameGanreList.Add(gameGenre);
            }

            return _goodsRepository.CreateRangeGamesGenresAsync(newGameGanreList);
        }

        public async Task UpdateGenreForGameAsync(Guid gameId, List<Genre> genres)
        {
            List<GameGenre> oldGameGenres = await _goodsRepository.GetGamesGenresByGameIdAsync(gameId);
            List<Genre> newGenres = genres.Where(g => oldGameGenres.All(gg => gg.GenreId != g.Id)).ToList();
            List<GameGenre> removedGenres = oldGameGenres.Where(gg => genres.All(g => g.Id != gg.GenreId)).ToList();

            if (newGenres.Count != 0)
            {
                await AddGenreToGoodsAsync(gameId, newGenres);
            }

            if (removedGenres.Count != 0)
            {
                await _goodsRepository.DeleteRangeGamesGenresAsync(removedGenres);
            }
        }

        public Task RemoveGenreFromGoodsAsync(Guid gameId, List<Genre> genresToRemove)
        {
            ValidateChangeForGame(gameId, genresToRemove);

            List<GameGenre> removeGameGanreList = new List<GameGenre>();

            foreach (var g in genresToRemove)
            {
                GameGenre gameGenre = new GameGenre()
                {
                    GameId = gameId,
                    GenreId = g.Id
                };
                removeGameGanreList.Add(gameGenre);
            }

            return _goodsRepository.DeleteRangeGamesGenresAsync(removeGameGanreList);
        }

        public Task AddPlatformTypeToGoodsAsync(Guid gameId, List<PlatformType> platformTypesToAdd)
        {
            ValidateChangeForGame(gameId, platformTypesToAdd);

            List<GamePlatformType> newGamePlatformTypeList = new List<GamePlatformType>();

            foreach (var p in platformTypesToAdd)
            {
                GamePlatformType gamelatformType = new GamePlatformType()
                {
                    GameId = gameId,
                    PlatformId = p.Id
                };
                newGamePlatformTypeList.Add(gamelatformType);
            }

            return _goodsRepository.CreateRangeGamesPlatformTypesAsync(newGamePlatformTypeList);
        }

        public async Task UpdatePlatformTypeForGameAsync(Guid gameId, List<PlatformType> platformTypes)
        {
            List<GamePlatformType> oldGamePlatformType = await _goodsRepository.GetGamePlatformByGameIdAsync(gameId);
            List<PlatformType> newPlatformTypes = platformTypes.Where(g => oldGamePlatformType.All(gp => gp.PlatformId != g.Id)).ToList();
            List<GamePlatformType> removedPlatformTypes = oldGamePlatformType.Where(gp => platformTypes.All(g => g.Id != gp.PlatformId)).ToList();

            if (newPlatformTypes.Count != 0)
            {
                await AddPlatformTypeToGoodsAsync(gameId, newPlatformTypes);
            }

            if (removedPlatformTypes.Count != 0)
            {
                await _goodsRepository.DeleteRangeGamesPlatformTypesAsync(removedPlatformTypes);
            }
        }

        public Task RemovePlatformTypeFromGoodsAsync(Guid gameId, List<PlatformType> platformTypesToRemove)
        {
            ValidateChangeForGame(gameId, platformTypesToRemove);

            List<GamePlatformType> removeGamePlatformTypeList = new List<GamePlatformType>();

            foreach (var p in platformTypesToRemove)
            {
                GamePlatformType gamePlatformType = new GamePlatformType()
                {
                    GameId = gameId,
                    PlatformId = p.Id
                };
                removeGamePlatformTypeList.Add(gamePlatformType);
            }

            return _goodsRepository.DeleteRangeGamesPlatformTypesAsync(removeGamePlatformTypeList);
        }

        public Task<int> GetCountOfGoodsAsync()
        {
            return _goodsRepository.GetCountOfGoods();
        }

        public Task<byte[]> DownloadAsync(string key)
        {
            string baseDirectory = AppContext.BaseDirectory;
            string newPath = Path.GetFullPath(
                Path.Combine(baseDirectory, _downloadingFileConfiguration.FolderPath + key + _downloadingFileConfiguration.FileExtension));

            if (!File.Exists(newPath))
            {
                throw new NotFoundException($"Download file for game {key} was'nt found");
            }

            return File.ReadAllBytesAsync(newPath);
        }

        private string GenerateKey(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Can't create a game without a name");
            }

            List<char> removeCharsList = new List<char>() { '@', '_', ',', '.', ' ' };

            return string.Concat(name.Split(removeCharsList.ToArray())).ToLower();
        }

        private void ValidateChangeForGame<T>(Guid gameId, List<T> changeList)
        {
            if (gameId == Guid.Empty)
            {
                throw new ArgumentException("No game id was given");
            }

            if (changeList.Count == 0)
            {
                throw new ArgumentException($"list of {typeof(T).Name} is empty");
            }
        }

        private async Task UpdateOriginalGoodsAsync(Goods changedGame)
        {
            bool isUnique = await _goodsRepository.IsGoodsUnique(changedGame);
            if (!isUnique)
            {
                throw new NotUniqueException("Game with that key is already exist");
            }

            await _goodsRepository.UpdateAsync(changedGame);

            Guid goodId = await _goodsRepository.FindGameIdByKeyAsync(changedGame.Key);

            await UpdateGenreForGameAsync(goodId, changedGame.Genres);
            await UpdatePlatformTypeForGameAsync(goodId, changedGame.PlatformTypes);
        }
    }
}
