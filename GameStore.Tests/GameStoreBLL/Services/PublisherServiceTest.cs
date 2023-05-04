using AutoMapper;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using GameStore.DAL.MappingProfiles;
using GameStore.DAL.Abstractions.Interfaces;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Services;
using GameStore.DomainModels.Models.CreateModels;
using GameStore.DomainModels.Models;
using GameStore.DomainModels.Exceptions;
using GameStore.DomainModels.Models.EditModels;
using GameStore.BLL.MappingProfiles;
using GameStore.PL.MappingProfiles;

namespace GameStore.Tests.GameStoreBLL.Services
{
    public class PublisherServiceTest
    {
        private readonly Mock<IDistributorRepository> _publisherRepository;
        private readonly IDistributorService _service;
        private readonly IMapper _mapper;

        public PublisherServiceTest()
        {
            _publisherRepository = new Mock<IDistributorRepository>();

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<EntityDomainMapperProfile>();
                cfg.AddProfile<BusinessMappingProfile>();
                cfg.AddProfile<PresentationLayerMapperProfile>();
            });
            _mapper = new Mapper(configuration);

            _service = new DistributorService(_publisherRepository.Object, _mapper);
        }

        [Fact]
        public async Task SavePublisherAsync_CalledCreateAsyncRepositoryMethod()
        {
            _publisherRepository.Setup(x => x.IsDistributorUniqueAsync(It.IsAny<Distributor>()))
                .ReturnsAsync(true);

            await _service.CreateDistributorAsync(new CreateDistributorRequest());

            _publisherRepository.Verify(x => x.CreateAsync(It.IsAny<Distributor>()));
        }

        [Fact]
        public async Task FindPublisherAsync_EnteredCorrectId_ReturnsPublisherWithSameId()
        {
            Guid id = Guid.NewGuid();
            Distributor publisher = new Distributor()
            {
                Id = id.ToString()
            };

            _publisherRepository.Setup(x => x.FindAsync(It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(publisher);

            var actual = await _service.GetDistributorByIdAsync(Guid.Empty);

            actual.Should().NotBeNull()
                .And.BeOfType<Distributor>()
                .Which.Id.Should().Be(id.ToString());
        }

        [Fact]
        public void FindPublisherAsync_EnteredInvalidId_ThrowNotFoundException()
        {
            _service.Invoking(x => x.GetDistributorByIdAsync(Guid.Empty))
                .Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task GetAllPublisherAsync_ReturnedPublisherList()
        {
            _publisherRepository.Setup(x => x.GetAllAsync(It.IsAny<bool>())).ReturnsAsync(new List<Distributor>() { new Distributor() });

            var actual = await _service.GetAllDistributorsAsync();

            actual.Should().NotBeNull()
                .And.BeOfType<List<Distributor>>();
        }

        [Fact]
        public async Task EditPublisherAsync_CalledEditAsyncRepositoryMethod()
        {
            _publisherRepository.Setup(x => x.IsDistributorUniqueAsync(It.IsAny<Distributor>()))
                .ReturnsAsync(true);

            await _service.EditDistributorAsync(new EditDistributorRequest());

            _publisherRepository.Verify(x => x.UpdateAsync(It.IsAny<Distributor>()));
        }

        [Fact]
        public async Task SoftDeletePublisherAsync_EnteredCorrectId_CalledUpdateAsync()
        {
            _publisherRepository.Setup(x => x.FindAsync(It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(new Distributor());

            await _service.SoftDeleteDistributorAsync(Guid.Parse("CC32CB4A-F03C-447E-95EB-356C5F8D28B3"));

            _publisherRepository.Verify(x => x.SoftDeleteAsync(It.IsAny<Guid>()));
        }

        [Fact]
        public void SoftDeletePublisherAsync_EnteredInvalidId_ThrowNotFoundException()
        {
            _service.Invoking(x => x.SoftDeleteDistributorAsync(Guid.Empty))
                .Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task HardDeletePublisherAsync_EnteredCorrectId_CalledDeleteAsync()
        {
            _publisherRepository.Setup(x => x.FindAsync(It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(new Distributor());

            await _service.HardDeleteDistributorAsync(Guid.Empty);

            _publisherRepository.Verify(x => x.DeleteAsync(It.IsAny<Guid>()));
        }

        [Fact]
        public void HardDeletePublisherAsync_EnteredInvalidId_ThrowNotFoundException()
        {
            _service.Invoking(x => x.HardDeleteDistributorAsync(Guid.Empty))
                .Should().ThrowAsync<NotFoundException>();
        }
        [Fact]
        public async Task FindPublisherByNameAsync_EnteredCorrectName_ReturnsPublisherWithSameId()
        {
            Guid id = Guid.NewGuid();
            Distributor publisher = new Distributor()
            {
                Id = id.ToString()
            };

            _publisherRepository.Setup(x => x.FindByNameAsync(It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(publisher);

            var actual = await _service.GetDistributorByNameAsync("");

            actual.Should().NotBeNull()
                .And.BeOfType<Distributor>()
                .Which.Id.Should().Be(id.ToString());
        }

        [Fact]
        public void FindPublisherByNameAsync_EnteredInvalidName_ThrowNotFoundException()
        {
            _service.Invoking(x => x.HardDeleteDistributorAsync(Guid.Empty))
                .Should().ThrowAsync<NotFoundException>();
        }
    }
}
