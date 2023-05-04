using GameStore.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.DAL.Repositories.MsSqlDdRepositories.Interfaces
{
    public interface IOrderDetailsSqlRepository : ISqlRepository<OrderDetailsEntity>
    {
        Task BatchChangeProductQuantityAsync(List<OrderDetailsEntity> entities);

        Task ChangeOrder(List<Guid> detailsIds, Guid newOrderId);

        Task ChangeProductQuantity(Guid id, int quantity);

        Task<List<OrderDetailsEntity>> FindByOrderIdAsync(Guid sqlOrderId, bool isIncludeDeleted = false);

        Task SoftDeleteRangeAsync(List<Guid> excessDetailsIds);
    }
}