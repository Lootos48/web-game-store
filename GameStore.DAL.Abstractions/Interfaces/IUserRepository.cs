using GameStore.DomainModels.Enums;
using GameStore.DomainModels.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.DAL.Abstractions.Interfaces
{
    public interface IUserRepository
    {
        public Task<User> GetUserByIdAsync(Guid id, bool asNoTracking = false);

        public Task<List<User>> GetAllUsersAsync(bool includeDeleted = false);

        public Task<User> GetUserByUsernameAsync(string username);

        public Task<List<Comment>> GetAllUserCommentsAsync(string username);

        public Task<List<Comment>> GetAllUserCommentsAsync(Guid id);

        public Task<List<Order>> GetAllUserOrdersAsync(string username);

        public Task<List<Order>> GetAllUserOrdersAsync(Guid id);

        public Task ChangeUserRoleAsync(string username, UserRoles role);

        Task ChangeUserRoleAsync(Guid userId, UserRoles role);

        public Task CreateAsync(User user);

        public Task EditAsync(User user);

        public Task DeleteAsync(Guid id);

        public Task SoftDeleteAsync(Guid id);

        public Task RecoverAsync(Guid id);

        Task<User> GetUserByCredentialsAsync(string email, string password);

        Task<User> FindByEmailAsync(string email);

        Task<User> GetUserByIdAsNoTrackingAsync(Guid id);

        Task<bool> IsUsernameUniqueAsync(User user);

        Task<bool> IsEmailUniqueAsync(User user);
    }
}
