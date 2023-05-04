using GameStore.BLL.Interfaces;
using GameStore.BLL.Workflows.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace GameStore.BLL.Workflows
{
    public class OrderCancelingWorkflow : IOrderCancelingWorkflow
    {
        private static readonly int OrdersPerIteration = 100;
        private readonly IOrderService _orderService;

        public OrderCancelingWorkflow(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task CancelExpiredOrdersAsync(CancellationToken cancellationToken)
        {

            while (!cancellationToken.IsCancellationRequested)
            {
                bool isThereAnyProcessedOrders = await _orderService.CancelExpiredOrdersAsync(OrdersPerIteration);
                if (!isThereAnyProcessedOrders)
                {
                    break;
                }
            }
        }
    }
}
