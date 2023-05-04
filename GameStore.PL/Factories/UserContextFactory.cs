using AutoMapper;
using GameStore.BLL.Interfaces;
using GameStore.PL.DTOs;
using GameStore.PL.Factories.Interfaces;
using GameStore.PL.ViewContexts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.PL.Factories
{
    public class UserContextFactory : IUserContextFactory
    {
        private readonly IUserService _userService;
        private readonly IDistributorService _distributorService;

        private readonly IMapper _mapper;

        public UserContextFactory(
            IUserService userService,
            IDistributorService distributorService,
            IMapper mapper)
        {
            _userService=userService;
            _distributorService=distributorService;
            _mapper=mapper;
        }

        public async Task<UserDistributorConnectingViewContext> BuildUserDistributorConnectingViewContextAsync(Guid userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            var distributors = await _distributorService.GetAllDistributorsAsync();

            var context = new UserDistributorConnectingViewContext()
            {
                User = _mapper.Map<UserDTO>(user),
                Distributors = _mapper.Map<List<DistributorDTO>>(distributors)
            };

            return context;
        }
    }
}
