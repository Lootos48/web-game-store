using GameStore.PL.ViewContexts;
using System;
using System.Threading.Tasks;

namespace GameStore.PL.Factories.Interfaces
{
    public interface IUserContextFactory
    {
        Task<UserDistributorConnectingViewContext> BuildUserDistributorConnectingViewContextAsync(Guid userId);
    }
}
