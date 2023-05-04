using AutoMapper;
using FluentAssertions;
using GameStore.BLL.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using GameStore.DAL.MappingProfiles;
using GameStore.DAL.Abstractions.Interfaces;
using GameStore.BLL.Interfaces;
using GameStore.DomainModels.Models.CreateModels;
using GameStore.DomainModels.Models;
using GameStore.DomainModels.Exceptions;
using GameStore.DomainModels.Models.EditModels;
using GameStore.BLL.MappingProfiles;
using GameStore.PL.MappingProfiles;

namespace GameStore.Tests.GameStoreBLL.Services
{
    public class GenreServiceTest
    {
        private readonly Mock<IGenreRepository> _genreRepositoryMock;
        private readonly Mock<IGoodsRepository> _gameRepositoryMock;
        private readonly IGenreService _genreService;
        private readonly IMapper _mapper;

        public GenreServiceTest()
        {
            _genreRepositoryMock = new Mock<IGenreRepository>();
            _gameRepositoryMock = new Mock<IGoodsRepository>();

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<EntityDomainMapperProfile>();
                cfg.AddProfile<BusinessMappingProfile>();
                cfg.AddProfile<PresentationLayerMapperProfile>();
            });
            _mapper = new Mapper(configuration);

            _genreService = new GenreService(_genreRepositoryMock.Object, _gameRepositoryMock.Object, _mapper);
        }

        [Fact]
        public async Task SaveGenreAsync_CalledCreateAsyncRepositoryMethod()
        {
            _genreRepositoryMock.Setup(x => x.IsGenreUnique(It.IsAny<Genre>()))
                .ReturnsAsync(true);

            await _genreService.CreateGenreAsync(new CreateGenreRequest());

            _genreRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Genre>()));
        }

        [Fact]
        public async Task FindGenreAsync_EnteredCorrectId_ReturnGenreWithSameId()
        {
            Guid id = Guid.NewGuid();
            Genre genre = new Genre()
            {
                Id = id
            };

            _genreRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
                .ReturnsAsync(genre);

            var actual = await _genreService.GetGenreByIdAsync(Guid.Empty);

            actual.Should().NotBeNull()
                .And.BeOfType<Genre>()
                .Which.Id.Should().Be(id);
        }

        [Fact]
        public void FindGenreAsync_EnteredInvalidId_ThrowNotFoundException()
        {
            _genreService.Invoking(x => x.GetGenreByIdAsync(Guid.Empty))
                .Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task GetGamesByGenreAsync_EnteredCorrectName_ReturnListOfGames()
        {
            _genreRepositoryMock.Setup(x => x.FindByNameAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(new Genre());

            _gameRepositoryMock.Setup(x => x.FindByGenreAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(new List<Goods>());

            var actual = await _genreService.GetGamesByGenreAsync("", "");

            actual.Should().NotBeNull()
                .And.BeOfType<List<Goods>>();
        }

        [Fact]
        public async Task GetAsync_ReturnsGenresList()
        {
            _genreRepositoryMock.Setup(x => x.GetAllAsync(It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(new List<Genre>() { new Genre() });

            var actual = await _genreService.GetAllGenresAsync();

            actual.Should().NotBeNull()
                .And.BeOfType<List<Genre>>();
        }

        [Fact]
        public async Task FindGenreByNameAsync_EnteredCorrectGenre_ReturnGenreWithSameId()
        {
            Guid id = Guid.NewGuid();
            Genre genre = new Genre()
            {
                Id = id
            };

            _genreRepositoryMock.Setup(x => x.FindByNameAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(genre);

            var actual = await _genreService.GetGenreByNameAsync("");

            actual.Should().NotBeNull()
                .And.BeOfType<Genre>()
                .Which.Id.Should().Be(id);
        }

        [Fact]
        public void FindGenreByNameAsync_EnteredInvalidName_ThrowNotFoundException()
        {
            _genreService.Invoking(x => x.GetGenreByNameAsync(""))
                .Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task EditGenreAsync_CalledUpdateAsync()
        {

            _genreRepositoryMock.Setup(x => x.IsGenreUnique(It.IsAny<Genre>()))
                .ReturnsAsync(true);

            await _genreService.EditGenreAsync(new EditGenreRequest());

            _genreRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Genre>()));
        }

        [Fact]
        public async Task SoftDeleteGenreAsync_EnteredCorrectId_CalledSoftDeleteAsync()
        {
            _genreRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
                .ReturnsAsync(new Genre());
            _genreRepositoryMock.Setup(x => x.GetChildGenresAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
                .ReturnsAsync(new List<Genre>() { new Genre() });
            _genreRepositoryMock.Setup(x => x.BatchUpdateParentsAsync(It.IsAny<List<Genre>>()));

            _genreRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
                .ReturnsAsync(new Genre());

            await _genreService.SoftDeleteGenreAsync(Guid.Empty);

            _genreRepositoryMock.Verify(x => x.SoftDeleteAsync(It.IsAny<Guid>()));
        }

        [Fact]
        public void SoftDeleteGenreAsync_EnteredInvalidId_ThrowNotFoundException()
        {
            _genreService.Invoking(x => x.SoftDeleteGenreAsync(Guid.Empty))
                .Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task HardDeleteGenreAsync_EnteredCorrectId_CalledDeleteAsync()
        {
            _genreRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
                .ReturnsAsync(new Genre());
            _genreRepositoryMock.Setup(x => x.GetChildGenresAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
                .ReturnsAsync(new List<Genre>() { new Genre() });
            _genreRepositoryMock.Setup(x => x.BatchUpdateParentsAsync(It.IsAny<List<Genre>>()));

            await _genreService.HardDeleteGenreAsync(Guid.Empty);

            _genreRepositoryMock.Verify(x => x.DeleteByIdAsync(It.IsAny<Guid>()));
        }

        [Fact]
        public void HardDeleteGenreAsync_EnteredInvalidId_ThrowNotFoundException()
        {
            _genreService.Invoking(x => x.HardDeleteGenreAsync(Guid.Empty))
                .Should().ThrowAsync<NotFoundException>();
        }
    }
}
