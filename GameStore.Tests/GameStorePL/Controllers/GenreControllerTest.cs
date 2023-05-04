using AutoMapper;
using FluentAssertions;
using GameStore.BLL.Interfaces;
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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace GameStore.Tests.GameStorePL.Controllers
{
    public class GenreControllerTest
    {
        private readonly Mock<IGenreService> _genreServiceMock;
        private readonly IMapper _mapper;
        private readonly Mock<IGenreContextFactory> _genreContextFactoryMock;
        private readonly GenreController _controller;
        private readonly Mock<ILogger<GenreController>> _logger;

        public GenreControllerTest()
        {
            _genreServiceMock = new Mock<IGenreService>();
            _genreContextFactoryMock = new Mock<IGenreContextFactory>();
            _logger = new Mock<ILogger<GenreController>>();

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<PresentationLayerMapperProfile>();
            });
            _mapper = new Mapper(configuration);

            _controller = new GenreController
            (
                _genreServiceMock.Object,
                _mapper,
                _logger.Object,
                _genreContextFactoryMock.Object
            );
        }

        [Fact]
        public async Task IndexAsync_ReturnsViewWithListOfGenreDTO()
        {
            var result = await _controller.IndexAsync();

            var viewResult = result.Should().NotBeNull()
                .And.BeOfType<ViewResult>().Subject;

            viewResult.ViewName.Should().Be("Index");

            viewResult.Model.Should().NotBeNull()
                .And.BeOfType<List<GenreDTO>>();
        }

        [Fact]
        public async Task DetailsAsync_ReturnsViewWithGenreGamesDetailsViewContext()
        {
            string genre = "genre";
            GamesGenreDetailsViewContext context = new GamesGenreDetailsViewContext();

            _genreContextFactoryMock.Setup(obj => obj.BuildGamesGenreDetailsViewContextAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(context);

            var result = await _controller.DetailsAsync(genre);

            var viewResult = result.Should().NotBeNull()
                .And.BeOfType<ViewResult>().Subject;

            viewResult.ViewName.Should().Be("Details");

            viewResult.Model.Should().NotBeNull()
                .And.BeOfType<GamesGenreDetailsViewContext>();
        }

        [Fact]
        public async Task GetCreatingViewAsync_ReturnsViewWithGenreCreationViewContext()
        {
            GenreCreationViewContext context = new GenreCreationViewContext();

            _genreContextFactoryMock.Setup(obj => obj.BuildGenreCreationViewContextAsync())
                .ReturnsAsync(context);

            var result = await _controller.GetCreationViewAsync();

            var viewResult = result.Should().NotBeNull()
                .And.BeOfType<ViewResult>().Subject;

            viewResult.ViewName.Should().Be("Create");

            viewResult.Model.Should().NotBeNull()
                .And.BeOfType<GenreCreationViewContext>();
        }

        [Fact]
        public async Task CreateAsync_CalledSaveCenreAsync_RedirectToGenreDetails()
        {
            string genre = "genre";
            GenreCreationViewContext context = new GenreCreationViewContext()
            {
                GenreToCreate = new CreateGenreRequestDTO()
                {
                    Name = genre
                }
            };

            var result = await _controller.CreateAsync(context);

            _genreServiceMock.Verify(obj => obj.CreateGenreAsync(It.IsAny<CreateGenreRequest>()));

            var redirectResult = result.Should().NotBeNull()
                .And.BeOfType<RedirectToActionResult>().Subject;

            redirectResult.ActionName.Should().Be("Details");
            redirectResult.RouteValues.Should().Contain("genre", genre);
        }

        [Fact]
        public async Task GetEditingViewAsync_WithExictingKey_ReturnsViewWithGenreEditingViewContext()
        {
            string name = "genre";
            GenreEditingViewContext genreEditingViewContext = new GenreEditingViewContext
            {
                GenreToEdit = new EditGenreRequestDTO()
                {
                    Name = name
                }
            };

            _genreContextFactoryMock.Setup(obj => obj.BuildGenreEditingViewContextAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(genreEditingViewContext);

            var result = await _controller.GetEditingViewAsync(name);

            var viewResult = result.Should().NotBeNull()
                .And.BeOfType<ViewResult>().Subject;

            viewResult.ViewName.Should().Be("Edit");
            viewResult.Model.Should().NotBeNull()
                .And.BeOfType<GenreEditingViewContext>()
                .Which.GenreToEdit.Name.Should().Be(name);
        }

        [Fact]
        public async Task EditAsync_CalledEditGenreAsync_RedirectToIndex()
        {
            GenreEditingViewContext context = new GenreEditingViewContext();

            var result = await _controller.EditAsync(context);

            _genreServiceMock.Verify(obj => obj.EditGenreAsync(It.IsAny<EditGenreRequest>()));

            var redirectResult = result.Should().NotBeNull()
                .And.BeOfType<RedirectToActionResult>().Subject;

            redirectResult.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task GetDeletingViewAsync_WithExictingKey_ReturnsViewWithGenreDTO()
        {
            string name = "name";
            Genre genre = new Genre
            {
                Name = name
            };

            _genreServiceMock.Setup(obj => obj.GetGenreByNameAsync(name, It.IsAny<string>()))
                .ReturnsAsync(genre);

            var result = await _controller.GetDeletingViewAsync(name);

            var viewResult = result.Should().NotBeNull()
                .And.BeOfType<ViewResult>().Subject;

            viewResult.ViewName.Should().Be("Delete");
            viewResult.Model.Should().NotBeNull()
                .And.BeOfType<GenreDTO>()
                .Which.Name.Should().Be(name);
        }

        [Fact]
        public async Task DeleteAsync_WithExictingId_RedirectToIndex()
        {
            Guid id = Guid.NewGuid();

            var result = await _controller.DeleteAsync(id);

            _genreServiceMock.Verify(obj => obj.SoftDeleteGenreAsync(It.IsAny<Guid>()));

            var redirectResult = result.Should().NotBeNull()
                .And.BeOfType<RedirectToActionResult>().Subject;

            redirectResult.ActionName.Should().Be("Index");
        }
    }
}
