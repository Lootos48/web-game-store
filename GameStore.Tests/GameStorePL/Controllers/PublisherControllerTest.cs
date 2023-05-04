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
using GameStore.PL.MappingProfiles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace GameStore.Tests.GameStorePL.Controllers
{
    public class PublisherControllerTest
    {
        private readonly Mock<IDistributorService> _publisherServiceMock;
        private readonly IMapper _mapper;
        private readonly PublisherController _controller;

        public PublisherControllerTest()
        {
            _publisherServiceMock = new Mock<IDistributorService>();

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<PresentationLayerMapperProfile>();
            });
            _mapper = new Mapper(configuration);

            _controller = new PublisherController(_publisherServiceMock.Object, _mapper);



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
        public async Task IndexAsync_ReturnsPublisherList()
        {
            Guid guid = Guid.NewGuid();
            List<Distributor> publisher = new List<Distributor>() { new Distributor { Id = guid.ToString() } };

            _publisherServiceMock.Setup(x => x.GetAllDistributorsAsync()).ReturnsAsync(publisher);

            var result = await _controller.IndexAsync();

            var viewResult = result.Should().NotBeNull()
                .And.BeOfType<ViewResult>().Subject;

            viewResult.ViewName.Should().Be("Index");

            viewResult.Model.Should().NotBeNull()
                .And.BeOfType<List<DistributorDTO>>();
        }

        [Fact]
        public async Task DetailsByIdAsync_EnteredExistsId_DetailsViewWithPublisherDTO()
        {
            Guid guid = Guid.NewGuid();
            Distributor publisher = new Distributor { Id = guid.ToString() };
            _publisherServiceMock.Setup(x => x.GetDistributorByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(publisher);

            var result = await _controller.DetailsAsync(Guid.Empty);

            var viewResult = result.Should().NotBeNull()
                .And.BeOfType<ViewResult>().Subject;

            viewResult.ViewName.Should().Be("Details");

            viewResult.Model.Should().NotBeNull()
                .And.BeOfType<DistributorDTO>().Which.Id.Should().Be(guid.ToString());
        }

        [Fact]
        public async Task DetailsByNameAsync_EnteredExistsName_DetailsViewWithPublisherDTO()
        {
            Guid guid = Guid.NewGuid();
            Distributor publisher = new Distributor { Id = guid.ToString() };
            _publisherServiceMock.Setup(x => x.GetDistributorByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(publisher);

            var result = await _controller.DetailsAsync("");

            var viewResult = result.Should().NotBeNull()
                .And.BeOfType<ViewResult>().Subject;

            viewResult.ViewName.Should().Be("Details");

            viewResult.Model.Should().NotBeNull()
                .And.BeOfType<DistributorDTO>().Which.Id.Should().Be(guid.ToString());
        }

        [Fact]
        public void GetCreationViewAsync_ReturnedCreateViewResult()
        {
            var result = _controller.GetCreationViewAsync();

            var viewResult = result.Should().NotBeNull()
                .And.BeOfType<ViewResult>().Subject;

            viewResult.ViewName.Should().Be("Create");
        }

        [Fact]
        public async Task CreateAsync_CalledCreatePublisherAsync_RedirectToPublisherDetails()
        {
            string companyName = "type";
            var platformTypeDTO = new CreateDistributorRequestDTO()
            {
                CompanyName = companyName
            };

            var result = await _controller.CreateAsync(platformTypeDTO);

            _publisherServiceMock.Verify(obj => obj.CreateDistributorAsync(It.IsAny<CreateDistributorRequest>()));

            var redirectResult = result.Should().NotBeNull()
                .And.BeOfType<RedirectToActionResult>().Subject;

            redirectResult.ActionName.Should().Be("Details");
            redirectResult.RouteValues.Should().Contain("name", companyName);
        }

        [Fact]
        public async Task GetEditingViewAsync_WithExictingType_ReturnsViewWithPlatformTypeDTO()
        {
            string companyName = "type";
            Distributor platformType = new Distributor()
            {
                CompanyName = companyName
            };

            _publisherServiceMock.Setup(obj => obj.GetDistributorByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(platformType);

            var result = await _controller.GetEditingViewAsync(companyName);

            var viewResult = result.Should().NotBeNull()
                .And.BeOfType<ViewResult>().Subject;

            viewResult.ViewName.Should().Be("Edit");
            viewResult.Model.Should().NotBeNull()
                .And.BeOfType<EditDistributorRequestDTO>()
                .Which.CompanyName.Should().Be(companyName);
        }

        [Fact]
        public async Task EditAsync_CalledEditPlatformAsync_RedirectToIndex()
        {
            string companyName = "type";
            var publisherDTO = new EditDistributorRequestDTO()
            {
                CompanyName = companyName
            };

            var result = await _controller.EditAsync(publisherDTO);

            _publisherServiceMock.Verify(obj => obj.EditDistributorAsync(It.IsAny<EditDistributorRequest>()));

            var redirectResult = result.Should().NotBeNull()
                .And.BeOfType<RedirectToActionResult>().Subject;

            redirectResult.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task GetDeletingViewAsync_WithExictingType_ReturnsViewWithPublisherDTO()
        {
            string companyName = "type";
            Distributor publisherDTO = new Distributor
            {
                CompanyName = companyName
            };

            _publisherServiceMock.Setup(obj => obj.GetDistributorByNameAsync(companyName))
                .ReturnsAsync(publisherDTO);

            var result = await _controller.GetDeletingViewAsync(companyName);

            var viewResult = result.Should().NotBeNull()
                .And.BeOfType<ViewResult>().Subject;

            viewResult.ViewName.Should().Be("Delete");
            viewResult.Model.Should().NotBeNull()
                .And.BeOfType<DistributorDTO>()
                .Which.CompanyName.Should().Be(companyName);
        }

        [Fact]
        public async Task DeleteAsync_WithExictingId_RedirectToIndex()
        {
            Guid id = Guid.NewGuid();

            var result = await _controller.DeleteAsync(id);

            _publisherServiceMock.Verify(obj => obj.SoftDeleteDistributorAsync(It.IsAny<Guid>()));

            var redirectResult = result.Should().NotBeNull()
                .And.BeOfType<RedirectToActionResult>().Subject;

            redirectResult.ActionName.Should().Be("Index");
        }
    }
}
