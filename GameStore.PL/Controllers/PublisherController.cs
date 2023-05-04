using AutoMapper;
using GameStore.BLL.Interfaces;
using GameStore.DomainModels.Enums;
using GameStore.DomainModels.Exceptions;
using GameStore.DomainModels.Models;
using GameStore.DomainModels.Models.CreateModels;
using GameStore.DomainModels.Models.EditModels;
using GameStore.PL.DTOs;
using GameStore.PL.DTOs.CreateDTOs;
using GameStore.PL.DTOs.EditDTOs;
using GameStore.PL.Filters;
using GameStore.PL.Util.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.PL.Controllers
{
    [Route("[controller]")]
    public class PublisherController : Controller
    {
        private readonly IDistributorService _publisherService;
        private readonly IMapper _mapper;

        public PublisherController(
            IDistributorService publisherService, 
            IMapper mapper)
        {
            _publisherService = publisherService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> IndexAsync()
        {
            List<Distributor> publishers = await _publisherService.GetAllDistributorsAsync();
            List<DistributorDTO> publishersDTO = _mapper.Map<List<DistributorDTO>>(publishers);

            return View("Index", publishersDTO);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> DetailsAsync(Guid id)
        {
            Distributor foundedPublisher = await _publisherService.GetDistributorByIdAsync(id);
            DistributorDTO publisher = _mapper.Map<DistributorDTO>(foundedPublisher);

            return View("Details", publisher);
        }

        [HttpGet("{name}")]
        [ArgumentDecoderFilter("name")]
        public async Task<IActionResult> DetailsAsync(string name)
        {
            Distributor foundedPublisher = await _publisherService.GetDistributorByNameAsync(name);
            DistributorDTO publisher = _mapper.Map<DistributorDTO>(foundedPublisher);

            return View("Details", publisher);
        }

        [PermissionLevel(UserRoles.Manager)]
        [HttpGet("new")]
        public ActionResult GetCreationViewAsync()
        {
            return View("Create");
        }

        [PermissionLevel(UserRoles.Manager)]
        [HttpPost("new")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(CreateDistributorRequestDTO publisherToCreate)
        {
            CreateDistributorRequest createRequest = _mapper.Map<CreateDistributorRequest>(publisherToCreate);

            try
            {
                await _publisherService.CreateDistributorAsync(createRequest);
            }
            catch (NotUniqueException ex)
            {
                ModelState.AddModelError("Company name", ex.Message);
                return View("Create");
            }

            return RedirectToAction("Details", new { name = createRequest.CompanyName });
        }

        [PermissionLevel(UserRoles.Publishers)]
        [HttpGet("update/{name}")]
        [ArgumentDecoderFilter("name")]
        public async Task<IActionResult> GetEditingViewAsync(string name)
        {
            Distributor foundPublisher = await _publisherService.GetDistributorByNameAsync(name);

            if (!HierarchicalValidator.IsUserManagerOrOwner(foundPublisher.UserId, User))
            {
                return Forbid();
            }

            EditDistributorRequestDTO publiser = _mapper.Map<EditDistributorRequestDTO>(foundPublisher);

            return View("Edit", publiser);
        }

        [PermissionLevel(UserRoles.Publishers)]
        [HttpPost("update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync(EditDistributorRequestDTO editRequestDto)
        {
            if (!HierarchicalValidator.IsUserManagerOrOwner(editRequestDto.UserId, User))
            {
                return Forbid();
            }

            EditDistributorRequest editRequest = _mapper.Map<EditDistributorRequest>(editRequestDto);

            try
            {
                await _publisherService.EditDistributorAsync(editRequest);
            }
            catch (NotUniqueException ex)
            {
                ModelState.AddModelError("Company name", ex.Message);
                return View("Edit");
            }

            return RedirectToAction("Index");
        }

        [PermissionLevel(UserRoles.Manager)]
        [HttpGet("remove/{name}")]
        [ArgumentDecoderFilter("name")]
        public async Task<IActionResult> GetDeletingViewAsync(string name)
        {
            Distributor foundedPublisher = await _publisherService.GetDistributorByNameAsync(name);
            DistributorDTO publisherToDelete = _mapper.Map<DistributorDTO>(foundedPublisher);

            return View("Delete", publisherToDelete);
        }

        [PermissionLevel(UserRoles.Manager)]
        [HttpPost("remove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await _publisherService.SoftDeleteDistributorAsync(id);
            return RedirectToAction("Index");
        }
    }
}
