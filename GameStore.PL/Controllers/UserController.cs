using AutoMapper;
using GameStore.BLL.Interfaces;
using GameStore.DomainModels.Enums;
using GameStore.DomainModels.Exceptions;
using GameStore.DomainModels.Models;
using GameStore.PL.App_Code;
using GameStore.PL.Attributes;
using GameStore.PL.Configurations;
using GameStore.PL.DTOs;
using GameStore.PL.DTOs.CreateDTOs;
using GameStore.PL.DTOs.EditDTOs;
using GameStore.PL.Factories.Interfaces;
using GameStore.PL.Filters;
using GameStore.PL.Util.Authorization;
using GameStore.PL.ViewContexts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GameStore.PL.Controllers
{
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;

        private readonly IUserContextFactory _userFactory;

        private readonly GuestCookieSettings _coockieSettings;

        private readonly IMapper _mapper;

        public UserController(
            IUserService userService,
            IOrderService orderService,
            IUserContextFactory userFactory,
            GuestCookieSettings coockieSettings,
            IMapper mapper)
        {
            _userService = userService;
            _orderService = orderService;
            _userFactory = userFactory;
            _coockieSettings = coockieSettings;
            _mapper = mapper;
        }

        [PermissionLevel(UserRoles.Moderator)]
        [HttpGet("Ban/{username}")]
        public IActionResult GetBanView(string username)
        {
            UserDTO user = new UserDTO
            {
                Id = Guid.NewGuid(),
                Username = username
            };

            return View("Ban", user);
        }

        [PermissionLevel(UserRoles.Moderator)]
        [HttpPost("Ban")]
        public async Task<IActionResult> BanAsync(Guid userId, [Duration] uint hours)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            await _userService.BanAsync(userId, hours);

            return RedirectToAction("Index", "Games");
        }

        [PermissionLevel(UserRoles.Administrator)]
        [HttpGet()]
        public async Task<IActionResult> IndexAsync()
        {
            var users = await _userService.GetAllUsersAsync();

            var usersDtos = _mapper.Map<List<UserDTO>>(users);

            return View("Index", usersDtos);
        }

        [HttpGet("{username}")]
        [ArgumentDecoderFilter("username")]
        public async Task<IActionResult> DetailsAsync(string username)
        {
            var user = await _userService.GetUserByUsernameAsync(username);
            if (user is null)
            {
                return NotFound();
            }

            var userDto = _mapper.Map<UserDTO>(user);

            return View("Details", userDto);
        }

        [PermissionLevel(UserRoles.User)]
        [HttpGet("{username}/update")]
        [ArgumentDecoderFilter("username")]
        public async Task<IActionResult> GetEditingView(string username)
        {
            if (!HierarchicalValidator.IsUserAdminOrOwner(username, User))
            {
                return Forbid();
            }

            var user = await _userService.GetUserByUsernameAsync(username);

            var userDto = _mapper.Map<EditUserRequestDTO>(user);

            return View("Edit", userDto);
        }

        [PermissionLevel(UserRoles.User)]
        [HttpPost("update")]
        public async Task<IActionResult> EditAsync(EditUserRequestDTO userToEdit)
        {
            if (!HierarchicalValidator.IsUserAdminOrOwner(userToEdit.Id, User))
            {
                return Forbid();
            }

            var user = _mapper.Map<User>(userToEdit);
            try
            {
                await _userService.EditAsync(user);
            }
            catch (NotUniqueException ex)
            {
                ModelState.AddModelError("Unique constraint violation", ex.Message);
                return View("Edit");
            }

            if (HierarchicalValidator.IsCurrentUserOwner(userToEdit.Id, User) && IsUserChangedHisCredentials(userToEdit))
            {
                user = await _userService.GetUserByIdAsync(user.Id);
                await SignInAsync(user);
            }

            return RedirectToAction("Details", new { username = userToEdit.Username });
        }

        [PermissionLevel(UserRoles.Administrator)]
        [HttpGet("{username}/remove")]
        [ArgumentDecoderFilter("username")]
        public async Task<IActionResult> GetDeletingViewAsync(string username)
        {
            if (HierarchicalValidator.IsUserAdminAndOwner(username, User))
            {
                return Forbid();
            }

            var user = await _userService.GetUserByUsernameAsync(username);

            var userDto = _mapper.Map<UserDTO>(user);

            return View("Delete", userDto);
        }

        [PermissionLevel(UserRoles.Administrator)]
        [HttpPost("remove")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            if (HierarchicalValidator.IsUserAdminAndOwner(id, User))
            {
                return Forbid();
            }

            await _userService.SoftDeleteAsync(id);

            return RedirectToAction("Index");
        }

        [HttpGet("register")]
        public IActionResult GetRegistrationView()
        {
            return View("Register");
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(CreateUserRequestDTO userToCreate)
        {
            var user = _mapper.Map<User>(userToCreate);

            try
            {
                await RegisterUserAsync(user);
            }
            catch (NotUniqueException ex)
            {
                ModelState.AddModelError("Unique constraint violation", ex.Message);
                return View("Register");
            }

            user = await _userService.GetUserByEmailAsync(user.Email);
            await SignInAsync(user);

            return RedirectToAction("Details", new { username = user.Username });
        }

        [HttpGet("login")]
        public IActionResult GetLoginView()
        {
            return View("Login");
        }

        [HttpGet("access-denied")]
        public IActionResult GetAccesDeniedView()
        {
            ModelState.AddModelError("Role", "Access was denied");
            return View("Login");
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginUserRequestDTO request)
        {
            User user = await _userService.LoginAsync(request.Email, request.Password);

            await SignInAsync(user);

            return RedirectToAction("Details", new { username = user.Username });
        }

        [HttpGet("logout")]
        public async Task<IActionResult> LogoutAsync()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction("GetLoginView");
        }

        [PermissionLevel(UserRoles.Administrator)]
        [HttpPost]
        public async Task<IActionResult> OnPostEditRoleAsync(Guid userId, UserRoles role, UserRoles oldRole)
        {
            if (HierarchicalValidator.IsUserAdminAndOwner(userId, User))
            {
                return Forbid();
            }

            if (role == UserRoles.Publishers)
            {
                return RedirectToAction("GetUserDistributorConnectingView", new { userId = userId });
            }

            if (oldRole == UserRoles.Publishers)
            {
                await _userService.DisconnectUserFromDistributorAsync(userId);
            }

            await _userService.ChangeUserRoleAsync(userId, role);

            return RedirectToAction("Index");
        }

        [PermissionLevel(UserRoles.Administrator)]
        [HttpGet("{userId:guid}/distributor-connect")]
        public async Task<IActionResult> GetUserDistributorConnectingViewAsync(Guid userId)
        {
            var context = await _userFactory.BuildUserDistributorConnectingViewContextAsync(userId);
            return View("UserDistributorConnecting", context);
        }

        [PermissionLevel(UserRoles.Administrator)]
        [HttpPost("user-distributor-connect")]
        public async Task<IActionResult> ConnectUserToDistributorAsync(UserDistributorConnectingViewContext context)
        {
            await _userService.ChangeUserRoleAsync(context.User.Id, UserRoles.Publishers);
            await _userService.ConnectUserToDistributorAsync(context.User.Id, context.DistributorId);

            return RedirectToAction("Index");
        }

        private async Task SignInAsync(User user)
        {
            var guestIdString = HttpContext.Request.Cookies[_coockieSettings.GuestIdCookieName];
            if (guestIdString != null)
            {
                Response.Cookies.Delete(_coockieSettings.GuestIdCookieName);

                if (guestIdString != user.Id.ToString())
                {
                    await ConvertGuestToUserAsync(user, guestIdString);
                }
            }

            ClaimsPrincipal claimsPrincipal = CreateIdentityClaims(user);
            await HttpContext.SignInAsync("Cookies", claimsPrincipal);
        }

        private async Task ConvertGuestToUserAsync(User user, string guestIdString)
        {
            Guid guestId = Guid.Parse(guestIdString);
            await _orderService.MergeGuestOrderAsync(guestId, user.Id);
            await _userService.SoftDeleteAsync(guestId);
        }

        private Task RegisterUserAsync(User user)
        {
            var coockieGuestId = HttpContext.Request.Cookies[_coockieSettings.GuestIdCookieName];
            if (coockieGuestId == null)
            {
                return _userService.CreateAsync(user);
            }

            user.Role = UserRoles.User;
            user.Id = Guid.Parse(coockieGuestId);

            return _userService.EditAsync(user);
        }

        private static ClaimsPrincipal CreateIdentityClaims(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            ClaimsIdentity identity = new ClaimsIdentity(claims, "ApplicationCookie",
                ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            return new ClaimsPrincipal(identity);
        }

        private bool IsUserChangedHisCredentials(EditUserRequestDTO userToEdit)
        {
            return (User.Identity.Name != userToEdit.Username || ClaimsHelper.GetUserEmail(User.Claims) != userToEdit.Email);
        }
    }
}
