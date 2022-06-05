using BackgroundProcessing.Jobs.NbgCurrency;
using EP.Shared.Configuration;
using Microsoft.Extensions.Options;

namespace BackgroundProcessing.Workers.Currency
{
    public class NbgCurrencyProcessor : INbgCurrencyProcessor, IDisposable
    {
        private readonly ILogger<NbgCurrencyProcessor> _logger;
        public readonly IServiceProvider ServiceProvider;
        private readonly NbgConfig _config;

        public NbgCurrencyProcessor(
            ILogger<NbgCurrencyProcessor> logger,
            IServiceProvider serviceProvider, IOptionsMonitor<NbgConfig> config)
        {
            _logger = logger;
            ServiceProvider = serviceProvider;
            _config = config.CurrentValue;
        }

        public async Task ProcessCurrencyAsync(CancellationToken cancellationToken)
        {

            while (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("Checked if Orders processing 5 minute expired");
                using var scope = ServiceProvider.CreateScope();
                var nbgCurrencyService = scope.ServiceProvider.GetService<INbgCurrencyProcessorService>();
                if (nbgCurrencyService != null && await nbgCurrencyService.UpdateCurrencyDatabase())
                {
                    _logger.LogInformation($"NBG Currency updated successfully");
                }
                await Task.Delay(1000 * _config.SyncTimer, cancellationToken);
            }
        }

        public void Dispose()
        {

        }
    }
}
