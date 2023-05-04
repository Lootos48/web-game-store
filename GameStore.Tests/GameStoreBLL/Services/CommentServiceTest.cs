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
using GameStore.DomainModels.Models;
using GameStore.DomainModels.Exceptions;
using GameStore.DomainModels.Models.CreateModels;
using GameStore.DomainModels.Models.EditModels;
using GameStore.BLL.MappingProfiles;

namespace GameStore.Tests.GameStoreBLL.Services
{
    public class CommentServiceTest
    {
        private readonly Mock<ICommentRepository> _commentRepositoryMock;
        private readonly Mock<IGoodsRepository> _gameRepositoryMock;
        private readonly ICommentService _commentService;
        private readonly IMapper _mapper;

        public CommentServiceTest()
        {
            _commentRepositoryMock = new Mock<ICommentRepository>();
            _gameRepositoryMock = new Mock<IGoodsRepository>();

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<EntityDomainMapperProfile>();
                cfg.AddProfile<BusinessMappingProfile>();
            });
            _mapper = new Mapper(configuration);

            _commentService = new CommentService
            (
                _commentRepositoryMock.Object,
                _gameRepositoryMock.Object,
                _mapper
            );
        }

        [Fact]
        public async Task SaveCommentAsync_EnteredRightGameKey_CalledCreateAsync()
        {
            string key = "Sekiro";
            Guid guid = Guid.Empty;
            var entity = new Goods
            {
                Id = guid.ToString(),
                Key = key
            };

            _gameRepositoryMock.Setup(x => x.FindByKeyAsync(key, It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(entity);

            await _commentService.CreateCommentAsync(key, new CreateCommentRequest());

            _commentRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Comment>()));
        }

        [Fact]
        public void SaveCommentAsync_EnteredInvalidGameKey_ThrowNotFoundException()
        {
            _commentService.Invoking(x => x.CreateCommentAsync("", new CreateCommentRequest()))
                .Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task GetByGameKeyAsync_EnteredCorrectGameKey_ReturnsListOfCommentsWithComment()
        {
            Guid id = Guid.NewGuid();
            _gameRepositoryMock.Setup(x => x.FindByKeyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(new Goods() { Id = id.ToString() });
            _commentRepositoryMock.Setup(x => x.GetByGameIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
                .ReturnsAsync(new List<Comment>()
                {
                    new Comment()
                    {
                        Id = id
                    }
                });

            var actual = await _commentService.GetByGameKeyAsync("");

            actual.Should().NotBeNull()
                .And.BeOfType<List<Comment>>()
                .And.HaveCount(1);
        }

        [Fact]
        public void GetByGameKeyAsync_EnteredInvalidGameKey_ThrowNotFoundException()
        {
            _commentService.Invoking(x => x.GetByGameKeyAsync(""))
                .Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task FindCommentAsync_EnteredCorrectId_ReturnCommentWithSameId()
        {
            Guid id = Guid.NewGuid();
            Comment comment = new Comment()
            {
                Id = id,
            };
            _commentRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
                .ReturnsAsync(comment);

            var actual = await _commentService.GetCommentByIdAsync(id);

            actual.Should().NotBeNull()
                .And.BeOfType<Comment>()
                .Which.Id.Should().Be(id);
        }

        [Fact]
        public void FindCommentAsync_EnteredInvalidId_ThrowNotFoundException()
        {
            _commentService.Invoking(x => x.GetCommentByIdAsync(Guid.Empty))
                .Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task GetAsync_ReturnsCommentList()
        {
            _commentRepositoryMock.Setup(x => x.GetAllAsync(It.IsAny<bool>()))
                .ReturnsAsync(new List<Comment>());

            var actual = await _commentService.GetAllCommentsAsync();

            actual.Should().NotBeNull()
                .And.BeOfType<List<Comment>>();
        }

        [Fact]
        public async Task EditCommentAsync_UpdateAsyncWasCalled()
        {
            await _commentService.EditCommentAsync(new EditCommentRequest());

            _commentRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Comment>()));
        }

        [Fact]
        public async Task SoftDeleteCommentAsync_EnteredCorrectId_UpdateAsyncWasCalled()
        {
            _commentRepositoryMock.Setup(x => x.FindByIdAsync(Guid.Empty, It.IsAny<bool>())).ReturnsAsync(new Comment());

            await _commentService.SoftDeleteCommentAsync(Guid.Empty);

            _commentRepositoryMock.Verify(x => x.SoftDeleteAsync(It.IsAny<Guid>()));
        }

        [Fact]
        public void SoftDeleteCommentAsync_EnteredInvalidId_ThrowNotFoundException()
        {
            _commentService.Invoking(x => x.SoftDeleteCommentAsync(Guid.Empty))
                .Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task HardDeleteCommentAsync_EnteredCorrectId_DeleteAsyncWasCalled()
        {
            _commentRepositoryMock.Setup(x => x.FindByIdAsync(Guid.Empty, It.IsAny<bool>())).ReturnsAsync(new Comment());

            await _commentService.HardDeleteCommentAsync(Guid.Empty);

            _commentRepositoryMock.Verify(x => x.DeleteByIdAsync(It.IsAny<Guid>()));
        }

        [Fact]
        public void HardDeleteCommentAsync_EnteredInvalidId_ThrowNotFoundException()
        {
            _commentService.Invoking(x => x.HardDeleteCommentAsync(Guid.Empty))
                .Should().ThrowAsync<NotFoundException>();
        }
    }
}
