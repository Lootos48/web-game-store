using AutoMapper;
using FluentAssertions;
using GameStore.BLL.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using GameStore.DAL.MappingProfiles;
using GameStore.BLL.Settings;
using GameStore.DomainModels.Models.CreateModels;
using GameStore.DomainModels.Models;
using GameStore.BLL.Interfaces;
using GameStore.DAL.Abstractions.Interfaces;
using GameStore.DomainModels.Exceptions;
using GameStore.DomainModels.Models.EditModels;
using GameStore.BLL.MappingProfiles;
using GameStore.PL.MappingProfiles;

namespace GameStore.Tests.GameStoreBLL.Services
{
    public class GameServiceTest
    {
        private readonly Mock<IGoodsRepository> _gameRepositoryMock;
        private readonly Mock<DownloadingFileSettings> _configMock;
        private readonly IGoodsService _goodsService;
        private readonly IMapper _mapper;

        public GameServiceTest()
        {
            _gameRepositoryMock = new Mock<IGoodsRepository>();
            _configMock = new Mock<DownloadingFileSettings>();

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<EntityDomainMapperProfile>();
                cfg.AddProfile<BusinessMappingProfile>();
                cfg.AddProfile<PresentationLayerMapperProfile>();
            });
            _mapper = new Mapper(configuration);

            _goodsService = new GoodsService(
                _gameRepositoryMock.Object,
                _configMock.Object,
                _mapper);
        }

        [Fact]
        public async Task SaveGameAsync_GameRequestWithKey_ReturnsGameKey()
        {
            string expectedKey = "game";
            CreateGoodsRequest createGameRequest = new CreateGoodsRequest { Key = expectedKey };

            _gameRepositoryMock.Setup(x => x.IsGoodsUnique(It.IsAny<Goods>()))
                .ReturnsAsync(true);

            var actual = await _goodsService.CreateGoodsAsync(createGameRequest);

            _gameRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Goods>()));
            actual.Should().Be(expectedKey);
        }

        [Fact]
        public async Task SaveGameAsync_GameRequestWithName_ReturnsGameKey()
        {
            string expectedKey = "game";
            CreateGoodsRequest createGameRequest = new CreateGoodsRequest { Name = "@G a_m,e" };

            _gameRepositoryMock.Setup(x => x.IsGoodsUnique(It.IsAny<Goods>()))
                .ReturnsAsync(true);

            var actual = await _goodsService.CreateGoodsAsync(createGameRequest);

            _gameRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Goods>()));
            actual.Should().Be(expectedKey);
        }

        [Fact]
        public void CreateGameAsync_GameRequestWithoutNameAndKey_ThrowsArgumentException()
        {
            _goodsService.Invoking(x => x.CreateGoodsAsync(new CreateGoodsRequest()))
                .Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public void AddGenreToGameAsync_EnteredWithotGuid_ThrowsArgumentException()
        {
            var guid = Guid.Empty;
            var genres = new List<Genre>() { new Genre() };

            _goodsService.Invoking(x => x.AddGenreToGoodsAsync(guid, genres))
                .Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public void AddGenreToGameAsync_EnteredWithEmptyList_ThrowsArgumentException()
        {
            var guid = Guid.NewGuid();
            var genres = new List<Genre>() { };

            _goodsService.Invoking(x => x.AddGenreToGoodsAsync(guid, genres))
                .Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task AddGenreToGameAsync_EnteredWithCorrectParameters_CalledCreateRangeAsync()
        {
            var guid = Guid.NewGuid();
            var genres = new List<Genre>() { new Genre() };

            await _goodsService.AddGenreToGoodsAsync(guid, genres);

            _gameRepositoryMock.Verify(x => x.CreateRangeGamesGenresAsync(It.IsAny<List<GameGenre>>()));
        }

        [Fact]
        public async Task FindByKeyAsync_EnteredWithCorrectKey_ReturnsGameKey()
        {
            string expectedKey = "sekiro";
            _gameRepositoryMock.Setup(x => x.FindByKeyAsync(expectedKey, It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(new Goods() { Key = expectedKey });

            var actual = await _goodsService.GetGoodsByKeyAsync(expectedKey);

            actual.Should().NotBeNull()
                .And.BeOfType<Goods>()
                .Which.Key.Should().Be(expectedKey);
        }

        [Fact]
        public void GetGameByKeyAsync_EnteredOldKeyOfNotExistedGame_ThrowsNotFoundException()
        {
            Guid guid = Guid.Empty;

            _goodsService.Invoking(x => x.GetGoodsByKeyAsync(""))
                .Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public void FindByKeyAsync_EnteredWithInvalidKey_ThrowsNotFoundException()
        {
            _goodsService.Invoking(x => x.GetGoodsByKeyAsync(""))
                .Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task GetAllGamesAsync_GetAsyncMethodCalled()
        {
            await _goodsService.GetGoodsAsync();

            _gameRepositoryMock.Verify(x => x.GetAllAsync(It.IsAny<string>(), It.IsAny<bool>()));
        }

        [Fact]
        public async Task FindGameAsync_EnteredCorrectId_ReturnsGameWithEnteredId()
        {
            Guid id = Guid.NewGuid();
            Goods game = new Goods()
            {
                Id = id.ToString()
            };
            _gameRepositoryMock.Setup(x => x.FindAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(game);

            var actual = await _goodsService.GetGoodsByIdAsync(id);

            actual.Should().NotBeNull()
                .And.BeOfType<Goods>()
                .Which.Id.Should().Be(id.ToString());
        }

        [Fact]
        public void FindGameAsync_EnteredInvalidId_ThrowsNotFoundException()
        {
            _goodsService.Invoking(x => x.GetGoodsByIdAsync(Guid.Empty))
                .Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task GetCountOfGamesAsync_ReturnsCorrectCount()
        {
            var games = new List<Goods>() { new Goods(), new Goods() };
            _gameRepositoryMock.Setup(x => x.GetCountOfGoods(It.IsAny<bool>()))
                .ReturnsAsync(2);

            var actual = await _goodsService.GetCountOfGoodsAsync();

            actual.Should().Be(2);
        }

        [Fact]
        public async Task EditGameAsync_EnteredWithNotEditedKey_CalledUpdateAsync()
        {
            Guid guid = Guid.NewGuid();
            string key = "sekiro";
            EditGoodsRequest gameEditRequest = new EditGoodsRequest()
            {
                Id = guid.ToString(),
                Key = key,
                Genres = new List<Genre>(),
                PlatformTypes = new List<PlatformType>()
            };

            _gameRepositoryMock.Setup(x => x.IsGoodsUnique(It.IsAny<Goods>()))
                .ReturnsAsync(true);

            _gameRepositoryMock.Setup(x => x.GetGamesGenresByGameIdAsync(guid))
                .ReturnsAsync(new List<GameGenre>());
            _gameRepositoryMock.Setup(x => x.GetGamePlatformByGameIdAsync(guid))
                .ReturnsAsync(new List<GamePlatformType>());
            _gameRepositoryMock.Setup(x => x.FindGameIdByKeyAsync(gameEditRequest.Key, It.IsAny<bool>()))
                .ReturnsAsync(guid);

            await _goodsService.EditGoodsAsync(gameEditRequest);

            _gameRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Goods>()));
        }

        [Fact]
        public async Task AddPlatformTypeToGameAsync_EnteredCorrectParameters_CalledCreateRangeAsync()
        {
            Guid guid = Guid.NewGuid();
            var platforms = new List<PlatformType>() { new PlatformType() };

            await _goodsService.AddPlatformTypeToGoodsAsync(guid, platforms);

            _gameRepositoryMock.Verify(x => x.CreateRangeGamesPlatformTypesAsync(It.IsAny<List<GamePlatformType>>()));
        }

        [Fact]
        public void AddPlatformTypeToGameAsync_EnteredEmptyGuid_ThrowsNotFoundException()
        {
            Guid guid = Guid.Empty;
            var platforms = new List<PlatformType>() { new PlatformType() };

            _goodsService.Invoking(x => x.AddPlatformTypeToGoodsAsync(guid, platforms))
                .Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public void AddPlatformTypeToGameAsync_EnteredEmptyList_ThrowsArgumentException()
        {
            Guid guid = Guid.NewGuid();
            var platforms = new List<PlatformType>();

            _goodsService.Invoking(x => x.AddPlatformTypeToGoodsAsync(guid, platforms))
                .Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task RemoveGenreFromGameAsync_EnteredCorrectParameters_CalledDeleteRangeAsync()
        {
            Guid guid = Guid.NewGuid();
            var genres = new List<Genre>() { new Genre() };

            await _goodsService.RemoveGenreFromGoodsAsync(guid, genres);

            _gameRepositoryMock.Verify(x => x.DeleteRangeGamesGenresAsync(It.IsAny<List<GameGenre>>()));
        }

        [Fact]
        public void RemoveGenreFromGameAsync_EnteredEmptyGuid_ThrowsArgumentException()
        {
            Guid guid = Guid.Empty;
            var genres = new List<Genre>() { new Genre() };

            _goodsService.Invoking(x => x.RemoveGenreFromGoodsAsync(guid, genres))
                .Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public void RemoveGenreFromGameAsync_EnteredEmptyList_ThrowsArgumentException()
        {
            Guid guid = Guid.NewGuid();
            var genres = new List<Genre>();

            _goodsService.Invoking(x => x.RemoveGenreFromGoodsAsync(guid, genres))
                .Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task RemovePlatformTypeFromGameAsync_EnteredCorrectParameters_CalledDeleteRangeAsync()
        {
            Guid guid = Guid.NewGuid();
            var platforms = new List<PlatformType>() { new PlatformType() };

            await _goodsService.RemovePlatformTypeFromGoodsAsync(guid, platforms);

            _gameRepositoryMock.Verify(x => x.DeleteRangeGamesPlatformTypesAsync(It.IsAny<List<GamePlatformType>>()));
        }

        [Fact]
        public void RemovePlatformTypeFromGameAsync_EnteredEmptyGuid_ThrowsArgumentException()
        {
            Guid guid = Guid.Empty;
            var platforms = new List<PlatformType>() { new PlatformType() };

            _goodsService.Invoking(x => x.RemovePlatformTypeFromGoodsAsync(guid, platforms))
                .Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public void RemovePlatformTypeFromGameAsync_EnteredEmptyList_ThrowsArgumentException()
        {
            Guid guid = Guid.NewGuid();
            var platforms = new List<PlatformType>();

            _goodsService.Invoking(x => x.RemovePlatformTypeFromGoodsAsync(guid, platforms))
                .Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task SoftDeleteGameAsync_EnteredCorrectId_CalledUpdateAsync()
        {
            _gameRepositoryMock.Setup(x => x.FindAsync(Guid.Empty.ToString(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(new Goods());

            await _goodsService.SoftDeleteGoodsAsync(Guid.NewGuid().ToString());

            _gameRepositoryMock.Verify(x => x.SoftDeleteAsync(It.IsAny<string>()));
        }

        [Fact]
        public void SoftDeleteGameAsync_EnteredInvalidId_ThrowsNotFoundException()
        {
            _goodsService.Invoking(x => x.SoftDeleteGoodsAsync(Guid.NewGuid().ToString()))
                .Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task HardDeleteAsync_EnteredCorrectId_CalledDeleteAsync()
        {
            _gameRepositoryMock.Setup(x => x.FindAsync(Guid.Empty.ToString(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(new Goods());

            await _goodsService.HardDeleteGoodsAsync(Guid.Empty);

            _gameRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<Guid>()));
        }

        [Fact]
        public void HardDeleteAsync_EnteredInvalidId_ThrowsNotFoundException()
        {
            _goodsService.Invoking(x => x.HardDeleteGoodsAsync(Guid.Empty))
                .Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task RecoverAsync_CalledUpdateAsync()
        {
            await _goodsService.RecoverGoodsAsync(Guid.NewGuid());

            _gameRepositoryMock.Verify(x => x.RecoverAsync(It.IsAny<Guid>()));
        }
    }
}
