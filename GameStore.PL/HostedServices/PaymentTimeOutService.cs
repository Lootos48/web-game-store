using GameStore.BLL.Workflows.Interfaces;
using GameStore.DomainModels.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GameStore.PL.HostedServices
{
    [Singleton]
    public class PaymentTimeOutService : IHostedService
    {
        private readonly ILogger<PaymentTimeOutService> _logger;

        private CancellationTokenSource _cancellationTokenSource;
        private Task _worker;
        private Timer _timer;

        public PaymentTimeOutService(
            ILogger<PaymentTimeOutService> logger, 
            IServiceProvider services)
        {
            _logger = logger;
            Services = services;
        }

        private IServiceProvider Services { get; set; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource = new CancellationTokenSource();

            _timer = new Timer(
                RunCancellingExpiredOrders, 
                null, 
                TimeSpan.FromSeconds(0), 
                TimeSpan.FromHours(1));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource.Cancel();
            _timer?.Dispose();
            return _worker ?? Task.CompletedTask;
        }

        private void RunCancellingExpiredOrders(object nothing)
        {
            _worker = Task.Run(CancelExpiredOrderAsync, _cancellationTokenSource.Token);

            _logger.LogDebug("Expired orders canceling method was called");
        }

        private async Task CancelExpiredOrderAsync()
        {
            try
            {
                using (var scoped = Services.CreateScope())
                {
                    var workflow = scoped.ServiceProvider.GetRequiredService<IOrderCancelingWorkflow>();
                    await workflow.CancelExpiredOrdersAsync(_cancellationTokenSource.Token);
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(exp, "Error occured while trying to cancel expired orders");
            }
        }
    }
}
