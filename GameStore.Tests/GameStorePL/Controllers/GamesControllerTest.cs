using AutoMapper;
using FluentAssertions;
using GameStore.BLL.Interfaces;
using GameStore.DomainModels.Enums;
using GameStore.DomainModels.Models;
using GameStore.DomainModels.Models.CreateModels;
using GameStore.DomainModels.Models.EditModels;
using GameStore.PL.Controllers;
using GameStore.PL.DTOs;
using GameStore.PL.DTOs.CreateDTOs;
using GameStore.PL.DTOs.EditDTOs;
using GameStore.PL.Factories.Interfaces;
using GameStore.PL.MappingProfiles;
using GameStore.PL.ViewContexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace GameStore.Tests.GameStorePL.Controllers
{
    public class GamesControllerTest
    {
        private readonly Mock<IGoodsService> _gameServiceMock;
        private readonly IMapper _mapper;
        private readonly Mock<IGameContextFactory> _gameContextFactoryMock;
        private readonly GamesController _controller;
        private readonly Mock<ILogger<GamesController>> _logger;

        public GamesControllerTest()
        {
            _gameServiceMock = new Mock<IGoodsService>();
            _gameContextFactoryMock = new Mock<IGameContextFactory>();
            _logger = new Mock<ILogger<GamesController>>();

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<PresentationLayerMapperProfile>();
            });
            _mapper = new Mapper(configuration);

            _controller = new GamesController
            (
                _gameServiceMock.Object,
                _mapper,
                _logger.Object,
                _gameContextFactoryMock.Object
            );

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.Empty.ToString()),
                new Claim(ClaimTypes.Name, "test"),
                new Claim(ClaimTypes.Email, "test@mail.com"),
                new Claim(ClaimTypes.Role, UserRoles.Administrator.ToString())
            };

            var identity = new ClaimsIdentity(claims, "Test");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var controllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };

            _controller.ControllerContext = controllerContext;
        }

        [Fact]
        public async Task IndexAsync_ReturnsViewWithListOfGameDto()
        {
            _gameServiceMock.Setup(x => x.GetFilteredGoodsAsync(It.IsAny<GoodsSearchRequest>(), It.IsAny<string>()))
                .ReturnsAsync(new List<Goods>() { new Goods() });
            _gameContextFactoryMock.Setup(x => x.BuildGamesViewContextAsync(It.IsAny<List<GoodsDTO>>(), It.IsAny<string>()))
                .ReturnsAsync(new GamesViewsContext());

            var result = await _controller.IndexAsync(new GoodsSearchRequestDTO());

            var viewResult = result.Should().NotBeNull()
                .And.BeOfType<ViewResult>().Subject;

            viewResult.ViewName.Should().Be("Index");

            viewResult.Model.Should().NotBeNull()
                .And.BeOfType<GamesViewsContext>();
        }

        [Fact]
        public async Task GetCreatingViewAsync_ReturnsViewWithGameSavingViewContext()
        {
            GameCreationViewContext context = new GameCreationViewContext();

            _gameContextFactoryMock.Setup(obj => obj.BuildGameCreationContextAsync())
                .ReturnsAsync(context);

            var result = await _controller.GetCreationViewAsync();

            var viewResult = result.Should().NotBeNull()
                .And.BeOfType<ViewResult>().Subject;

            viewResult.ViewName.Should().Be("Create");

            viewResult.Model.Should().NotBeNull()
                .And.BeOfType<GameCreationViewContext>();
        }

        [Fact]
        public async Task CreateAsync_WithValidGame_RedirectToGameDetailsByKeyWithKey()
        {
            string key = "game";
            GameCreationViewContext context = new GameCreationViewContext()
            {
                GameCreateDTO = new CreateGoodsRequestDTO()
                {
                    Key = key
                }
            };

            _gameServiceMock.Setup(obj => obj.CreateGoodsAsync(It.IsAny<CreateGoodsRequest>()))
                .ReturnsAsync(key);

            var result = await _controller.CreateAsync(context);

            var redirectResult = result.Should().NotBeNull()
                .And.BeOfType<RedirectToActionResult>().Subject;

            redirectResult.ActionName.Should().Be("DetailsByKey");
            redirectResult.ControllerName.Should().Be("Game");
            redirectResult.RouteValues.Should().Contain("key", key);
        }

        [Fact]
        public async Task GetEditingViewAsync_WithExictingKey_ReturnsViewWithGameEditingViewContext()
        {
            string key = "game";
            Goods game = new Goods
            {
                Key = key
            };
            GameEditingViewContext gameEditingViewContext = new GameEditingViewContext
            {
                GameEditDTO = new EditGoodsRequestDTO()
                {
                    Key = key
                }
            };

            _gameServiceMock.Setup(obj => obj.GetGoodsByKeyIncludeAllLocalizationsAsync(key, It.IsAny<bool>()))
                .ReturnsAsync(game);
            _gameContextFactoryMock.Setup(obj => obj.BuildGameEditingContextAsync(It.IsAny<EditGoodsRequestDTO>()))
                .ReturnsAsync(gameEditingViewContext);

            var result = await _controller.GetEditingViewAsync(key);

            var viewResult = result.Should().NotBeNull()
                .And.BeOfType<ViewResult>().Subject;

            viewResult.ViewName.Should().Be("Edit");
            viewResult.Model.Should().NotBeNull()
                .And.BeOfType<GameEditingViewContext>()
                .Which.GameEditDTO.Key.Should().Be(key);
        }

        [Fact]
        public async Task EditAsync_WithValidGame_RedirectToIndex()
        {
            GameEditingViewContext context = new GameEditingViewContext()
            {
                GameEditDTO = new EditGoodsRequestDTO(),
                DistributorUserId = Guid.Empty
            };

            var result = await _controller.EditAsync(context);

            _gameServiceMock.Verify(obj => obj.EditGoodsAsync(It.IsAny<EditGoodsRequest>()));

            var redirectResult = result.Should().NotBeNull()
                .And.BeOfType<RedirectToActionResult>().Subject;

            redirectResult.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task GetDeletingViewAsync_WithExictingKey_ReturnsViewWithGameDto()
        {
            string key = "game";
            Goods game = new Goods
            {
                Key = key
            };

            _gameServiceMock.Setup(obj => obj.GetGoodsByKeyAsync(key, It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(game);

            var result = await _controller.GetDeletingViewAsync(key);

            var viewResult = result.Should().NotBeNull()
                .And.BeOfType<ViewResult>().Subject;

            viewResult.ViewName.Should().Be("Delete");
            viewResult.Model.Should().NotBeNull()
                .And.BeOfType<GoodsDTO>()
                .Which.Key.Should().Be(key);
        }

        [Fact]
        public async Task DeleteAsync_WithExictingId_RedirectToIndex()
        {
            Guid id = Guid.NewGuid();

            var result = await _controller.DeleteAsync(id.ToString());

            _gameServiceMock.Verify(obj => obj.SoftDeleteGoodsAsync(It.IsAny<string>()));

            var redirectResult = result.Should().NotBeNull()
                .And.BeOfType<RedirectToActionResult>().Subject;

            redirectResult.ActionName.Should().Be("Index");
        }
    }
}
