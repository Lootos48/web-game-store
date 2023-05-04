using GameStore.DAL.Entities;
using GameStore.DAL.Repositories.MsSqlDdRepositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.DAL.Repositories.MsSqlDdRepositories
{
    public class OrderDetailsSqlRepository : SqlRepository<OrderDetailsEntity>, IOrderDetailsSqlRepository
    {
        public OrderDetailsSqlRepository(DbContext context, ILogger<OrderDetailsSqlRepository> logger) : base(context, logger) { }

        public Task ChangeProductQuantity(Guid id, int quantity)
        {
            FormattableString query = $@"UPDATE [OrderDetails] SET [Quantity] = {quantity} WHERE [Id] = {id}";
            return _context.Database.ExecuteSqlInterpolatedAsync(query);
        }

        public Task SoftDeleteRangeAsync(List<Guid> excessDetailsIds)
        {
            if (excessDetailsIds.Count == 0)
            {
                return Task.CompletedTask;
            }

            FormattableString query = BuildSoftDeleteRangeQuery(excessDetailsIds);

            return _context.Database.ExecuteSqlInterpolatedAsync(query);
        }

        public Task ChangeOrder(List<Guid> detailsIds, Guid newOrderId)
        {
            if (detailsIds.Count == 0)
            {
                return Task.CompletedTask;
            }

            if (newOrderId == Guid.Empty)
            {
                throw new ArgumentException("Order id is empty");
            }

            FormattableString query = BuildChangeOrderQuery(detailsIds, newOrderId);

            return _context.Database.ExecuteSqlInterpolatedAsync(query);
        }

        public Task<List<OrderDetailsEntity>> FindByOrderIdAsync(Guid sqlOrderId, bool isIncludeDeleted = false)
        {
            return _dbSet
                .Include(x => x.Product)
                .Where(x => x.OrderId == sqlOrderId
                 && (!x.IsDeleted || isIncludeDeleted))
                .ToListAsync();
        }

        public Task BatchChangeProductQuantityAsync(List<OrderDetailsEntity> entities)
        {
            FormattableString query = BuildBatchChangeProductQuantityQuery(entities);
            return _context.Database.ExecuteSqlInterpolatedAsync(query);
        }

        private static FormattableString BuildSoftDeleteRangeQuery(List<Guid> excessDetailsIds)
        {
            StringBuilder query = new StringBuilder("UPDATE [OrderDetails] SET [IsDeleted] = 1 WHERE ");

            object[] arguments = new object[excessDetailsIds.Count];
            for (int i = 0; i < excessDetailsIds.Count; i++)
            {
                query.Append("[Id] = {" + i + "} OR");
                arguments[i] = excessDetailsIds[i];
            }

            query.Remove(query.Length - 2, 2);
            query.Append(";");

            return FormattableStringFactory.Create(query.ToString(), arguments);
        }

        private static FormattableString BuildChangeOrderQuery(List<Guid> detailsIds, Guid newOrderId)
        {
            object[] arguments = new object[detailsIds.Count + 1];
            arguments[detailsIds.Count] = newOrderId;

            StringBuilder query = new StringBuilder("UPDATE [OrderDetails] SET [OrderId] = {" + detailsIds.Count + "} WHERE ");
            for (int i = 0; i < detailsIds.Count; i++)
            {
                query.Append("[Id] = {" + i + "} OR");
                arguments[i] = detailsIds[i];
            }

            query.Remove(query.Length - 2, 2);
            query.Append(";");

            return FormattableStringFactory.Create(query.ToString(), arguments);
        }

        private static FormattableString BuildBatchChangeProductQuantityQuery(List<OrderDetailsEntity> entities)
        {
            StringBuilder query = new StringBuilder("UPDATE [OrderDetails] SET [Quantity] = CASE [Id]");
            StringBuilder queryPredicate = new StringBuilder("WHERE [Id] IN(");

            object[] arguments = new object[entities.Count * 2];
            for (int i = 0, argumentCounter = 0; i < entities.Count; i++, argumentCounter += 2)
            {
                query.Append(" WHEN {" + argumentCounter + "} THEN {" + (argumentCounter + 1) + "}");
                queryPredicate.Append("{" + argumentCounter + "}, ");

                arguments[argumentCounter] = entities[i].Id;
                arguments[argumentCounter + 1] = entities[i].Quantity;
            }

            query.Append(" ELSE [Quantity] END ");

            queryPredicate.Remove(queryPredicate.Length - 2, 2);
            queryPredicate.Append(");");

            query.Append(queryPredicate);

            return FormattableStringFactory.Create(query.ToString(), arguments);
        }
    }
}
