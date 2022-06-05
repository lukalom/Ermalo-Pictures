using BackgroundProcessing.Jobs.OrderDetails;
using EP.Shared.Configuration;
using Microsoft.Extensions.Options;

namespace BackgroundProcessing.Workers.OrderDetails
{
    public class OrderDetailsProcessor : IOrderDetailsProcessor, IDisposable
    {
        private readonly ILogger<OrderDetailsProcessor> _logger;
        public readonly IServiceProvider ServiceProvider;
        public readonly WorkerTimer WorkerTimer;

        public OrderDetailsProcessor(
            ILogger<OrderDetailsProcessor> logger,
            IServiceProvider serviceProvider,
            IOptionsMonitor<WorkerTimer> workerMonitor)
        {
            _logger = logger;
            ServiceProvider = serviceProvider;
            WorkerTimer = workerMonitor.CurrentValue;
        }

        public async Task ValidateOrders(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("Checked if Orders processing 5 minute expired");
                using var scope = ServiceProvider.CreateScope();
                var orderDetailsService = scope.ServiceProvider.GetService<IOrderDetailsProcessorService>();
                if (orderDetailsService != null) await orderDetailsService.CancelInvalidOrders();
                await Task.Delay(1000 * WorkerTimer.OrderSyncTimer, cancellationToken);
            }
        }

        public void Dispose()
        {

        }
    }
}
