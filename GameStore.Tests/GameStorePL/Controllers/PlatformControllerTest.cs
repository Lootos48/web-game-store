using AutoMapper;
using FluentAssertions;
using GameStore.BLL.Interfaces;
using GameStore.DomainModels.Models;
using GameStore.DomainModels.Models.CreateModels;
using GameStore.DomainModels.Models.EditModels;
using GameStore.PL.Controllers;
using GameStore.PL.DTOs;
using GameStore.PL.DTOs.EditDTOs;
using GameStore.PL.Factories.Interfaces;
using GameStore.PL.MappingProfiles;
using GameStore.PL.ViewContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace GameStore.Tests.GameStorePL.Controllers
{
    public class PlatformControllerTest
    {
        private readonly Mock<IPlatformTypeService> _platformServiceMock;
        private readonly IMapper _mapper;
        private readonly Mock<IPlatformContextFactory> _platformContextFactoryMock;
        private readonly PlatformController _controller;
        private readonly Mock<ILogger<PlatformController>> _logger;

        public PlatformControllerTest()
        {
            _platformServiceMock = new Mock<IPlatformTypeService>();
            _platformContextFactoryMock = new Mock<IPlatformContextFactory>();
            _logger = new Mock<ILogger<PlatformController>>();

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<PresentationLayerMapperProfile>();
            });
            _mapper = new Mapper(configuration);

            _controller = new PlatformController
            (
                _platformServiceMock.Object,
                _mapper,
                _logger.Object,
                _platformContextFactoryMock.Object
            );
        }

        [Fact]
        public async Task IndexAsync_ReturnsViewWithListOfPlatformTypeDTO()
        {
            var result = await _controller.IndexAsync();

            var viewResult = result.Should().NotBeNull()
                .And.BeOfType<ViewResult>().Subject;

            viewResult.ViewName.Should().Be("Index");

            viewResult.Model.Should().NotBeNull()
                .And.BeOfType<List<PlatformTypeDTO>>();
        }

        [Fact]
        public async Task DetailsAsync_ReturnsViewWithPlatformGamesDetailsViewContext()
        {
            string type = "type";
            var context = new GamesPlatformDetailsViewContext();

            _platformContextFactoryMock.Setup(obj => obj.BuildGamesPlatformDetailsViewContextAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(context);

            var result = await _controller.DetailsAsync(type);

            var viewResult = result.Should().NotBeNull()
                .And.BeOfType<ViewResult>().Subject;

            viewResult.ViewName.Should().Be("Details");

            viewResult.Model.Should().NotBeNull()
                .And.BeOfType<GamesPlatformDetailsViewContext>();
        }

        [Fact]
        public void GetCreatingView_ReturnsView()
        {
            var result = _controller.GetCreationViewAsync();

            var viewResult = result.Should().NotBeNull()
                .And.BeOfType<ViewResult>().Subject;

            viewResult.ViewName.Should().Be("Create");
        }

        [Fact]
        public async Task CreateAsync_CalledSavePlatformAsync_RedirectToPlatformTypeDetails()
        {
            string type = "type";
            PlatformTypeDTO platformTypeDTO = new PlatformTypeDTO()
            {
                Type = type
            };

            var result = await _controller.CreateAsync(platformTypeDTO);

            _platformServiceMock.Verify(obj => obj.CreatePlatformAsync(It.IsAny<CreatePlatformTypeRequest>()));

            var redirectResult = result.Should().NotBeNull()
                .And.BeOfType<RedirectToActionResult>().Subject;

            redirectResult.ActionName.Should().Be("Details");
            redirectResult.RouteValues.Should().Contain("type", type);
        }

        [Fact]
        public async Task GetEditingViewAsync_WithExictingType_ReturnsViewWithPlatformTypeDTO()
        {
            string type = "type";

            PlatformTypeEditingViewContext context = new PlatformTypeEditingViewContext()
            {
                PlatformType = new EditPlatformTypeRequestDTO()
                {
                    Type = type
                }
            };

            _platformContextFactoryMock.Setup(obj => obj.BuildPlatformTypeEditingViewContextAsync(It.IsAny<string>()))
                .ReturnsAsync(context);

            var result = await _controller.GetEditingViewAsync(type);

            var viewResult = result.Should().NotBeNull()
                .And.BeOfType<ViewResult>().Subject;

            viewResult.ViewName.Should().Be("Edit");
            viewResult.Model.Should().NotBeNull()
                .And.BeOfType<PlatformTypeEditingViewContext>()
                .Which.PlatformType.Type.Should().Be(type);
        }

        [Fact]
        public async Task EditAsync_CalledEditPlatformAsync_RedirectToIndex()
        {
            string type = "type";
            PlatformTypeEditingViewContext context = new PlatformTypeEditingViewContext()
            {
                PlatformType = new EditPlatformTypeRequestDTO
                {
                    Type = type
                }
            };

            _platformContextFactoryMock.Setup(obj => obj.BuildPlatformTypeEditingViewContextAsync(type))
                .ReturnsAsync(context);

            var result = await _controller.EditAsync(context);

            _platformServiceMock.Verify(obj => obj.EditPlatformAsync(It.IsAny<EditPlatformRequest>()));

            var redirectResult = result.Should().NotBeNull()
                .And.BeOfType<RedirectToActionResult>().Subject;

            redirectResult.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task GetDeletingViewAsync_WithExictingType_ReturnsViewWithPlatformTypeDTO()
        {
            string type = "type";
            PlatformType platform = new PlatformType
            {
                Type = type
            };

            _platformServiceMock.Setup(obj => obj.GetPlatformByTypeAsync(type, It.IsAny<string>()))
                .ReturnsAsync(platform);

            var result = await _controller.GetDeletingViewAsync(type);

            var viewResult = result.Should().NotBeNull()
                .And.BeOfType<ViewResult>().Subject;

            viewResult.ViewName.Should().Be("Delete");
            viewResult.Model.Should().NotBeNull()
                .And.BeOfType<PlatformTypeDTO>()
                .Which.Type.Should().Be(type);
        }

        [Fact]
        public async Task DeleteAsync_WithExictingId_RedirectToIndex()
        {
            Guid id = Guid.NewGuid();

            var result = await _controller.DeleteAsync(id);

            _platformServiceMock.Verify(obj => obj.SoftDeletePlatformAsync(It.IsAny<Guid>()));

            var redirectResult = result.Should().NotBeNull()
                .And.BeOfType<RedirectToActionResult>().Subject;

            redirectResult.ActionName.Should().Be("Index");
        }
    }
}
