using GameStore.BLL.Interfaces;
using GameStore.DAL.Abstractions.Interfaces;
using GameStore.DomainModels.Enums;
using GameStore.DomainModels.Exceptions;
using GameStore.DomainModels.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IDistributorRepository _distributorRepository;

        public UserService(
            IUserRepository userRepository,
            IDistributorRepository distributorRepository)
        {
            _userRepository=userRepository;
            _distributorRepository=distributorRepository;
        }

        public Task<List<Comment>> GetAllUserCommentsAsync(string username)
        {
            return _userRepository.GetAllUserCommentsAsync(username);
        }

        public Task<List<Comment>> GetAllUserCommentsAsync(Guid id)
        {
            return _userRepository.GetAllUserCommentsAsync(id);
        }

        public Task<List<Order>> GetAllUserOrdersAsync(string username)
        {
            return _userRepository.GetAllUserOrdersAsync(username);
        }

        public Task<List<Order>> GetAllUserOrdersAsync(Guid id)
        {
            return _userRepository.GetAllUserOrdersAsync(id);
        }

        public Task<List<User>> GetAllUsersAsync()
        {
            return _userRepository.GetAllUsersAsync();
        }

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user is null)
            {
                throw new NotFoundException("User with that id wasn't found");
            }

            return user;
        }

        public Task<User> GetUserByUsernameAsync(string username)
        {
            var user = _userRepository.GetUserByUsernameAsync(username);
            if (user is null)
            {
                throw new NotFoundException("User with that username wasn't found");
            }

            return user;
        }

        public Task BanAsync(Guid userId, uint duration)
        {
            DateTime banExpirationDate = DateTime.UtcNow.AddHours(duration);
            return Task.CompletedTask;
        }

        public async Task ChangeUserRoleAsync(Guid userId, UserRoles role)
        {
            await _userRepository.ChangeUserRoleAsync(userId, role);
        }

        public async Task ChangeUserRoleAsync(string username, UserRoles role)
        {
            await _userRepository.ChangeUserRoleAsync(username, role);
        }

        public async Task CreateAsync(User user)
        {
            if (!string.IsNullOrEmpty(user.Username) && !string.IsNullOrEmpty(user.Email))
            {
                await CheckUserCredentialsIsUnique(user);
            }

            if (user.Password != null)
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            }

            await _userRepository.CreateAsync(user);
        }

        public Task DeleteAsync(Guid id)
        {
            return _userRepository.DeleteAsync(id);
        }

        public async Task EditAsync(User user)
        {
            await CheckUserCredentialsIsUnique(user);

            await SecurePasswordAndRole(user);

            await _userRepository.EditAsync(user);
        }

        public Task RecoverAsync(Guid id)
        {
            return _userRepository.RecoverAsync(id);
        }

        public Task SoftDeleteAsync(Guid id)
        {
            return _userRepository.SoftDeleteAsync(id);
        }

        public Task ConnectUserToDistributorAsync(Guid userId, string distributorId)
        {
            return _distributorRepository.ConnectUserToDistributorAsync(userId, distributorId);
        }

        public Task DisconnectUserFromDistributorAsync(Guid userId)
        {
            return _distributorRepository.DisconnectUserFromDistributorAsync(userId);
        }

        public Task<User> GetUserByEmailAsync(string email)
        {
            return _userRepository.FindByEmailAsync(email);
        }

        public async Task<User> LoginAsync(string email, string password)
        {
            User user = await _userRepository.FindByEmailAsync(email);

            if (user is null)
            {
                throw new NotFoundException($"User with email {email} not found");
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                throw new InvalidPasswordException($"Password was wrong");
            }

            return user;
        }

        private async Task CheckUserCredentialsIsUnique(User user)
        {
            bool isUsernameUnique = await _userRepository.IsUsernameUniqueAsync(user);
            if (!isUsernameUnique)
            {
                throw new NotUniqueException("User with that username is already exist");
            }

            bool isEmailUnique = await _userRepository.IsEmailUniqueAsync(user);
            if (!isEmailUnique)
            {
                throw new NotUniqueException("User with that email is already exist");
            }
        }

        private async Task SecurePasswordAndRole(User user)
        {
            var oldUser = await _userRepository.GetUserByIdAsNoTrackingAsync(user.Id);
            if (oldUser == null || string.IsNullOrEmpty(oldUser.Email) || string.IsNullOrEmpty(oldUser.Username))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            }
            else
            {
                user.Role = oldUser.Role;
                user.Password = oldUser.Password;
            }
        }
    }
}
