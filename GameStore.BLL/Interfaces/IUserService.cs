using GameStore.DomainModels.Enums;
using GameStore.DomainModels.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.BLL.Interfaces
{
    public interface IUserService
    {
        Task BanAsync(Guid userId, uint duration);

        public Task<User> GetUserByIdAsync(Guid id);

        public Task<List<User>> GetAllUsersAsync();

        public Task<User> GetUserByUsernameAsync(string username);

        public Task<List<Comment>> GetAllUserCommentsAsync(string username);

        public Task<List<Comment>> GetAllUserCommentsAsync(Guid id);

        public Task<List<Order>> GetAllUserOrdersAsync(string username);

        public Task<List<Order>> GetAllUserOrdersAsync(Guid id);

        public Task ChangeUserRoleAsync(string username, UserRoles role);

        public Task CreateAsync(User user);

        public Task EditAsync(User user);

        public Task DeleteAsync(Guid id);

        public Task SoftDeleteAsync(Guid id);

        public Task RecoverAsync(Guid id);

        Task<User> LoginAsync(string email, string password);
        Task ConnectUserToDistributorAsync(Guid userId, string distributorId);
        Task<User> GetUserByEmailAsync(string email);
        Task ChangeUserRoleAsync(Guid userId, UserRoles role);
        Task DisconnectUserFromDistributorAsync(Guid userId);
    }
}
