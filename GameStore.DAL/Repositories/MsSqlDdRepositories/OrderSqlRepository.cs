using GameStore.DAL.Entities;
using GameStore.DAL.Repositories.MsSqlDdRepositories.Interfaces;
using GameStore.DomainModels.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore.DAL.Repositories.MsSqlDdRepositories
{
    public class OrderSqlRepository : SqlRepository<OrderEntity>, IOrderSqlRepository
    {
        public OrderSqlRepository(DbContext context, ILogger<OrderSqlRepository> logger) : base(context, logger) { }

        public Task<OrderEntity> FindByCustomerIdAsync(Guid id, string cultureCode = null, bool isIncludeDeleted = false)
        {
            return _dbSet
                .Include(o => o.OrderDetails.Where(od => !od.IsDeleted || isIncludeDeleted))
                    .ThenInclude(od => od.Product)
                        .ThenInclude(p => p.Localizations.Where(l => l.Localization.CultureCode == cultureCode))
                .Include(o => o.OrderDetails.Where(od => !od.IsDeleted || isIncludeDeleted))
                    .ThenInclude(od => od.Product)
                        .ThenInclude(od => od.Publisher)
                .FirstOrDefaultAsync(o => o.CustomerId == id
                    && o.Status != OrderStatus.Shipped
                    && o.Status != OrderStatus.Completed
                    && (!o.IsDeleted || isIncludeDeleted));
        }

        public override Task<OrderEntity> FindEntityByIdAsync(Guid id, bool includesDeletedRows = false)
        {
            return _dbSet
                .Include(o => o.OrderDetails.Where(od => !od.IsDeleted || includesDeletedRows))
                    .ThenInclude(od => od.Product)
                    .ThenInclude(od => od.Publisher)
                .FirstOrDefaultAsync(o => o.Id == id && (!o.IsDeleted || includesDeletedRows));
        }

        public Task<List<OrderEntity>> GetOrdersByOrderStatusAsync(OrderStatus status, bool isIncludeDeleted = false)
        {
            return _dbSet
                 .Include(o => o.OrderDetails.Where(od => !od.IsDeleted || isIncludeDeleted))
                    .ThenInclude(od => od.Product)
                .Where(o => o.Status == status && (!o.IsDeleted || isIncludeDeleted))
                .ToListAsync();
        }

        public Task<List<OrderEntity>> GetBetweenDatesAsync(DateTime minDate, DateTime maxDate, bool isIncludeDeleted = false)
        {
            return _dbSet
                .Where(o => o.OrderDate >= minDate && o.OrderDate <= maxDate && (!o.IsDeleted || isIncludeDeleted))
                .ToListAsync();
        }

        public Task<List<OrderEntity>> GetOrdersBeforeAsync(DateTime maxDate, bool isIncludeDeleted = false)
        {
            return _dbSet
                .Where(o => o.OrderDate <= maxDate && (!o.IsDeleted || isIncludeDeleted))
                .ToListAsync();
        }

        public Task<List<OrderEntity>> GetOrdersAfterDateAsync(DateTime minDate, bool isIncludeDeleted = false)
        {
            return _dbSet
                .Where(o => o.OrderDate >= minDate && (!o.IsDeleted || isIncludeDeleted))
                .ToListAsync();
        }

        public Task<List<OrderEntity>> GetExpiredOrdersAsync(int ordersAmount, bool isIncludeDeleted = false)
        {
            return _dbSet
                .Include(o => o.OrderDetails.Where(od => !od.IsDeleted || isIncludeDeleted))
                    .ThenInclude(od => od.Product)
                .Where(o => o.Status == OrderStatus.Processed
                    && o.ExpirationDateUtc <= DateTime.UtcNow
                    && (!o.IsDeleted || isIncludeDeleted))
                    .Take(ordersAmount).ToListAsync();
        }

        public Task ChangeOrderStatusAsync(OrderStatus status, Guid orderId)
        {
            FormattableString query = $@"UPDATE [Orders] SET [Status] = {status.ToString()} WHERE [Id] = {orderId};";
            return _context.Database.ExecuteSqlInterpolatedAsync(query);
        }

        public Task ProcessOrderAsync(DateTime expirationDate, Guid id)
        {
            FormattableString query = $@"UPDATE [Orders] SET [Status] = {OrderStatus.Processed.ToString()}, [ExpirationDateUtc] = {expirationDate.ToString("yyyy-MM-dd HH:mm:ss.fff")} WHERE [Id] = {id} ;";
            return _context.Database.ExecuteSqlInterpolatedAsync(query);
        }

        public Task ChangeCustomer(Guid orderId, Guid customerId)
        {
            FormattableString query = $@"UPDATE [Orders] SET [CustomerId] = {customerId} WHERE [Id] = {orderId};";
            return _context.Database.ExecuteSqlInterpolatedAsync(query);
        }

        public Task<OrderEntity> FindOrderByIdAsync(Guid id, string cultureCode = null, bool isIncludeDeleted = false)
        {
            return _dbSet
                .Include(o => o.OrderDetails.Where(od => !od.IsDeleted))
                    .ThenInclude(od => od.Product)
                        .ThenInclude(p => p.Localizations.Where(l => l.Localization.CultureCode == cultureCode))
                .Include(o => o.OrderDetails.Where(od => !od.IsDeleted))
                    .ThenInclude(od => od.Product)
                        .ThenInclude(od => od.Publisher)
                .FirstOrDefaultAsync(o => o.Id == id && (!o.IsDeleted || isIncludeDeleted));
        }
    }
}
