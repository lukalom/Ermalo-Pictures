using BackgroundProcessing.Jobs.Payment;
using EP.Shared.Configuration;
using Microsoft.Extensions.Options;

namespace BackgroundProcessing.Workers.Payment
{
    public class PaymentProcessor : IPaymentProcessor, IDisposable
    {
        private readonly ILogger<PaymentProcessor> _logger;
        public readonly IServiceProvider ServiceProvider;
        public readonly WorkerTimer WorkerTimer;

        public PaymentProcessor(
            ILogger<PaymentProcessor> logger,
            IServiceProvider serviceProvider,
            IOptionsMonitor<WorkerTimer> workerMonitor)
        {
            _logger = logger;
            ServiceProvider = serviceProvider;
            WorkerTimer = workerMonitor.CurrentValue;

        }

        public async Task ValidatePayment(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("Checked if Order Canceled and also Payment should cancel expired");
                using var scope = ServiceProvider.CreateScope();
                var orderDetailsService = scope.ServiceProvider.GetService<IPaymentProcessorService>();
                if (orderDetailsService != null) await orderDetailsService.CancelInvalidPayment();
                await Task.Delay(1000 * WorkerTimer.PaymentSyncTimer, cancellationToken);
            }
        }

        public void Dispose()
        {

        }
    }
}
