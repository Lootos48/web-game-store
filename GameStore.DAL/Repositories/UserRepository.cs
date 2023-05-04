using AutoMapper;
using GameStore.DAL.Abstractions.Interfaces;
using GameStore.DAL.Entities;
using GameStore.DAL.Repositories.MsSqlDdRepositories;
using GameStore.DomainModels.Enums;
using GameStore.DomainModels.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore.DAL.Repositories
{
    public class UserRepository : SqlRepository<UserEntity>, IUserRepository
    {
        private readonly IMapper _mapper;

        public UserRepository(
            DbContext context,
            ILogger<UserRepository> logger,
            IMapper mapper) : base(context, logger)
        {
            _mapper = mapper;
        }

        public Task<bool> IsUsernameUniqueAsync(User user)
        {
            return _dbSet.AllAsync(u => u.Username != user.Username || (u.Id == user.Id && u.Username == user.Username));
        }

        public Task<bool> IsEmailUniqueAsync(User user)
        {
            return _dbSet.AllAsync(u => u.Email != user.Email || (u.Id == user.Id && u.Email == user.Email));
        }

        public async Task<List<Comment>> GetAllUserCommentsAsync(string username)
        {
            var user = await _dbSet.Include(x => x.Comments).FirstOrDefaultAsync(x => x.Username == username);

            var comments = user.Comments;

            return _mapper.Map<List<Comment>>(comments);
        }

        public async Task<List<Comment>> GetAllUserCommentsAsync(Guid id)
        {
            var user = await _dbSet.Include(x => x.Comments).FirstOrDefaultAsync(x => x.Id == id);

            var comments = user.Comments;

            return _mapper.Map<List<Comment>>(comments);
        }

        public async Task<List<Order>> GetAllUserOrdersAsync(string username)
        {
            var user = await _dbSet.Include(x => x.Orders).FirstOrDefaultAsync(x => x.Username == username);

            var orders = user.Orders;

            return _mapper.Map<List<Order>>(orders);
        }

        public async Task<List<Order>> GetAllUserOrdersAsync(Guid id)
        {
            var user = await _dbSet.Include(x => x.Orders).FirstOrDefaultAsync(x => x.Id == id);

            var orders = user.Orders;

            return _mapper.Map<List<Order>>(orders);
        }

        public async Task<List<User>> GetAllUsersAsync(bool includeDeleted = false)
        {
            List<UserEntity> entities = await _dbSet
                .Where(e => e.Role != UserRoles.Guest && !e.IsDeleted || includeDeleted)
                .ToListAsync();

            return _mapper.Map<List<User>>(entities);
        }

        public async Task<User> GetUserByIdAsync(Guid id, bool asNoTracking = false)
        {
            UserEntity entity = await _dbSet.FirstOrDefaultAsync(e =>
            e.Id == id &&
            e.Role != UserRoles.Guest);

            return _mapper.Map<User>(entity);
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            var user = await _dbSet.FirstOrDefaultAsync(x => x.Username == username);
            return _mapper.Map<User>(user);
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            var user = await _dbSet.FirstOrDefaultAsync(x => x.Email == email);
            return _mapper.Map<User>(user);
        }

        public Task CreateAsync(User user)
        {
            var entity = _mapper.Map<UserEntity>(user);
            return CreateEntityAsync(entity);
        }

        public Task EditAsync(User user)
        {
            var entity = _mapper.Map<UserEntity>(user);
            return UpdateEntityAsync(entity);
        }

        public Task ChangeUserRoleAsync(Guid userId, UserRoles role)
        {
            FormattableString query = $@"UPDATE [Users] SET Role = {role} WHERE Id = {userId};";
            return _context.Database.ExecuteSqlInterpolatedAsync(query);
        }

        public Task ChangeUserRoleAsync(string username, UserRoles role)
        {
            FormattableString query = $@"UPDATE [Users] SET Role = {role} WHERE Username = {username};";
            return _context.Database.ExecuteSqlInterpolatedAsync(query);
        }

        public Task DeleteAsync(Guid id)
        {
            return DeleteByIdAsync(id);
        }

        public async Task<User> GetUserByCredentialsAsync(string email, string password)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(
                x => x.Email == email &&
                x.Password == password &&
                x.Role != UserRoles.Guest);

            return _mapper.Map<User>(entity);
        }

        public async Task<User> GetUserByIdAsNoTrackingAsync(Guid id)
        {
            var entity = await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

            return _mapper.Map<User>(entity);
        }
    }
}
