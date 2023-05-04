using AutoMapper;
using FluentAssertions;
using GameStore.BLL.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using GameStore.DAL.MappingProfiles;
using GameStore.DomainModels.Models;
using GameStore.DAL.Abstractions.Interfaces;
using GameStore.BLL.Interfaces;
using GameStore.DomainModels.Models.CreateModels;
using GameStore.DomainModels.Exceptions;
using GameStore.DomainModels.Models.EditModels;
using GameStore.BLL.MappingProfiles;
using GameStore.PL.MappingProfiles;

namespace GameStore.Tests.GameStoreBLL.Services
{
    public class PlatformTypeServiceTest
    {

        private readonly Mock<IPlatformTypeRepository> _platformRepositoryMock;
        private readonly Mock<IGoodsRepository> _gameRepositoryMock;
        private readonly IPlatformTypeService _service;
        private readonly IMapper _mapper;

        public PlatformTypeServiceTest()
        {
            _platformRepositoryMock = new Mock<IPlatformTypeRepository>();
            _gameRepositoryMock = new Mock<IGoodsRepository>();

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<EntityDomainMapperProfile>();
                cfg.AddProfile<BusinessMappingProfile>();
                cfg.AddProfile<PresentationLayerMapperProfile>();
            });

            _mapper = new Mapper(configuration);
            _service = new PlatformTypeService(_platformRepositoryMock.Object, _gameRepositoryMock.Object, _mapper);
        }

        [Fact]
        public async Task SavePlatformAsync_CalledCreateAsyncRepositoryMethod()
        {
            _platformRepositoryMock.Setup(x => x.IsPlatformTypeUnique(It.IsAny<PlatformType>()))
                .ReturnsAsync(true);

            await _service.CreatePlatformAsync(new CreatePlatformTypeRequest());

            _platformRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<PlatformType>()));
        }

        [Fact]
        public async Task FindPlatformAsync_EnteredCorrectId_ReturnPlatformTypeWithSameId()
        {
            Guid id = Guid.NewGuid();
            PlatformType platformType = new PlatformType()
            {
                Id = id
            };

            _platformRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
                .ReturnsAsync(platformType);

            var actual = await _service.GetPlatformByIdAsync(Guid.Empty);

            actual.Should().NotBeNull()
                .And.BeOfType<PlatformType>()
                .Which.Id.Should().Be(id);
        }

        [Fact]
        public void FindPlatformAsync_EnteredInvalidId_ThrowNotFoundException()
        {
            _service.Invoking(x => x.GetPlatformByIdAsync(Guid.Empty))
                .Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task FindPlatformByTypeAsync_EnteredCorrectName_ReturnPlatformTypeWithSameId()
        {
            Guid id = Guid.NewGuid();
            PlatformType platformType = new PlatformType()
            {
                Id = id
            };

            _platformRepositoryMock.Setup(x => x.FindByTypeAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(platformType);

            var actual = await _service.GetPlatformByTypeAsync("");

            actual.Should().NotBeNull()
                .And.BeOfType<PlatformType>()
                .Which.Id.Should().Be(id);
        }

        [Fact]
        public async Task GetAsync_ReturnsPlatformsList()
        {
            _platformRepositoryMock.Setup(x => x.GetAllAsync(It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(new List<PlatformType>() { new PlatformType() });

            var actual = await _service.GetAllPlatformsAsync();

            actual.Should().NotBeNull()
                .And.BeOfType<List<PlatformType>>();
        }

        [Fact]
        public void FindPlatformByTypeAsync_EnteredInvalidName_ThrowNotFoundException()
        {
            _service.Invoking(x => x.GetPlatformByTypeAsync(""))
                .Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task EditPlatformAsync_CalledUpdateAsync()
        {
            _platformRepositoryMock.Setup(x => x.IsPlatformTypeUnique(It.IsAny<PlatformType>()))
                .ReturnsAsync(true);

            await _service.EditPlatformAsync(new EditPlatformRequest());

            _platformRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<PlatformType>()));
        }

        [Fact]
        public async Task SoftDeletePlatformAsync_EnteredCorrectId_CalledUpdateAsync()
        {
            _platformRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
                .ReturnsAsync(new PlatformType());

            await _service.SoftDeletePlatformAsync(Guid.Empty);

            _platformRepositoryMock.Verify(x => x.SoftDeleteAsync(It.IsAny<Guid>()));
        }

        [Fact]
        public void SoftDeletePlatformAsync_EnteredInvalidId_ThrowNotFoundException()
        {
            _service.Invoking(x => x.SoftDeletePlatformAsync(Guid.Empty))
                .Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task HardDeletePlatformAsync_EnteredCorrectId_CalledDeleteAsync()
        {
            _platformRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
                .ReturnsAsync(new PlatformType());

            await _service.HardDeletePlatformAsync(Guid.Empty);

            _platformRepositoryMock.Verify(x => x.DeleteByIdAsync(It.IsAny<Guid>()));
        }

        [Fact]
        public void HardDeletePlatformAsync_EnteredInvalidId_ThrowNotFoundException()
        {
            _service.Invoking(x => x.HardDeletePlatformAsync(Guid.Empty))
                .Should().ThrowAsync<NotFoundException>();
        }
    }
}
