using AutoMapper;
using FluentAssertions;
using GameStore.BLL.Interfaces;
using GameStore.DomainModels.Enums;
using GameStore.DomainModels.Models;
using GameStore.DomainModels.Models.CreateModels;
using GameStore.PL.Configurations;
using GameStore.PL.Controllers;
using GameStore.PL.DTOs;
using GameStore.PL.DTOs.CreateDTOs;
using GameStore.PL.Factories.Interfaces;
using GameStore.PL.MappingProfiles;
using GameStore.PL.ViewContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace GameStore.Tests.GameStorePL.Controllers
{
    public class GameControllerTest
    {
        private readonly Mock<IGoodsService> _gameServiceMock;
        private readonly Mock<ICommentService> _commentServiceMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<ICommentContextFactory> _commentContextFactoryMock;
        private readonly Mock<ILogger<GameController>> _loggerMock;
        private readonly IMapper _mapper;
        private readonly Mock<GuestCookieSettings> _configMock;
        private readonly GameController _controller;

        public GameControllerTest()
        {
            _gameServiceMock = new Mock<IGoodsService>();
            _commentServiceMock = new Mock<ICommentService>();
            _commentContextFactoryMock = new Mock<ICommentContextFactory>();
            _userServiceMock = new Mock<IUserService>();
            _configMock = new Mock<GuestCookieSettings>();
            _loggerMock = new Mock<ILogger<GameController>>();

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<PresentationLayerMapperProfile>();
            });
            _mapper = new Mapper(configuration);

            _controller = new GameController
            (
                _gameServiceMock.Object,
                _commentServiceMock.Object,
                _commentContextFactoryMock.Object,
                _userServiceMock.Object,
                _mapper,
                _loggerMock.Object,
                _configMock.Object
            );
        }

        [Fact]
        public async Task DetailsByKeyAsync_WithExistingKey_ReturnsViewWithGameDto()
        {
            string key = "game";
            Goods game = new Goods() { Key = key };

            _gameServiceMock.Setup(obj => obj.GetGoodsByKeyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(game);

            var result = await _controller.DetailsByKeyAsync(key);

            var viewResult = result.Should().NotBeNull()
                .And.BeOfType<ViewResult>().Subject;

            viewResult.ViewName.Should().Be("Details");

            viewResult.Model.Should().NotBeNull()
                .And.BeOfType<GoodsDTO>()
                .Which.Key.Should().Be(key);
        }

        [Fact]
        public async Task DetailsByIdAsync_WithExistingId_ReturnsViewWithGameDto()
        {
            Guid id = Guid.NewGuid();
            Goods game = new Goods() { Id = id.ToString() };

            _gameServiceMock.Setup(obj => obj.GetGoodsByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(game);

            var result = await _controller.DetailsByIdAsync(id);

            var viewResult = result.Should().NotBeNull()
                .And.BeOfType<ViewResult>().Subject;

            viewResult.ViewName.Should().Be("Details");

            viewResult.Model.Should().NotBeNull()
                .And.BeOfType<GoodsDTO>()
                .Which.Id.Should().Be(id.ToString());
        }

        [Fact]
        public async Task GetCommentsAsync_WithExistingKey_ReturnsViewWithGameCommentsViewContext()
        {
            string key = "game";

            GameCommentsViewContext context = new GameCommentsViewContext()
            {
                GameKey = key
            };

            _commentContextFactoryMock.Setup(obj => obj.BuildGameCommentsContextViewAsync(
                key,
                It.IsAny<Guid?>(),
                It.IsAny<CommentType>()
                )).ReturnsAsync(context);

            var result = await _controller.GetCommentsAsync(key);

            var viewResult = result.Should().NotBeNull()
                .And.BeOfType<ViewResult>().Subject;

            viewResult.ViewName.Should().Be("GetComments");

            viewResult.Model.Should().NotBeNull()
                .And.BeOfType<GameCommentsViewContext>()
                .Which.GameKey.Should().Be(key);
        }

        [Fact]
        public async Task CreateCommentAsync_WithExistingKey_RedirectToGetCommentsWithKey()
        {
            string key = "game";
            GameCommentsViewContext context = new GameCommentsViewContext()
            {
                GameKey = key,
                CommentToCreate = new CreateCommentRequestDTO()
            };

            var result = await _controller.CreateCommentAsync(key, context);

            _commentServiceMock.Verify(obj => obj.CreateCommentAsync(It.IsAny<string>(), It.IsAny<CreateCommentRequest>()));

            var redirectResult = result.Should().NotBeNull()
                .And.BeOfType<RedirectToActionResult>().Subject;

            redirectResult.ActionName.Should().Be("GetComments");

            redirectResult.RouteValues.Should().NotBeNull()
                .And.Contain("key", key);
        }

        [Fact]
        public async Task CommentReplyAsync_WithExistingKey_ReturnsViewWithGameCommentsViewContext()
        {
            string key = "game";
            Guid parentId = Guid.NewGuid();
            GameCommentsViewContext context = new GameCommentsViewContext()
            {
                GameKey = key,
                CommentToCreate = new CreateCommentRequestDTO
                {
                    ParentId = parentId
                }
            };

            _commentContextFactoryMock.Setup(obj => obj.BuildGameCommentsContextViewAsync(
                It.IsAny<string>(),
                It.IsAny<Guid>(),
                It.IsAny<CommentType>()
                )).ReturnsAsync(context);

            var result = await _controller.CommentReplyAsync(key, parentId);

            var viewResult = result.Should().NotBeNull()
                .And.BeOfType<ViewResult>().Subject;

            viewResult.ViewName.Should().Be("GetComments");

            viewResult.Model.Should().NotBeNull()
                .And.BeOfType<GameCommentsViewContext>()
                .Which.CommentToCreate.ParentId.Should().Be(parentId);
        }

        [Fact]
        public void Download_WithExistingKey_ReturnsViewWithGameDto()
        {
            string key = "game";

            var result = _controller.Download(key);

            var viewResult = result.Should().NotBeNull()
                .And.BeOfType<ViewResult>().Subject;

            viewResult.ViewName.Should().Be("Download");

            viewResult.Model.Should().NotBeNull()
                .And.BeOfType<GoodsDTO>()
                .Which.Key.Should().Be(key);
        }

        [Fact]
        public async Task DownloadFileAsync_WithExistingKey_ReturnsFile()
        {
            string key = "game";
            byte[] GameIco = new byte[] { 0 };

            _gameServiceMock.Setup(obj => obj.DownloadAsync(It.IsAny<string>()))
                .ReturnsAsync(GameIco);

            var result = await _controller.DownloadFileAsync(key);

            var fileResult = result.Should().NotBeNull()
                .And.BeOfType<FileContentResult>().Subject;

            fileResult.ContentType.Should().Be("text/plain");
            fileResult.FileDownloadName.Should().Be($"{key}.bin");
            fileResult.FileContents.Should().BeEquivalentTo(GameIco);
        }
    }
}
