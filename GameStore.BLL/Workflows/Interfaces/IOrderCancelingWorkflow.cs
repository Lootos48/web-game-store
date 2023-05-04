using System.Threading;
using System.Threading.Tasks;

namespace GameStore.BLL.Workflows.Interfaces
{
    public interface IOrderCancelingWorkflow
    {
        Task CancelExpiredOrdersAsync(CancellationToken cancellationToken);
    }
}
