using BackgroundProcessing.Jobs.Show;
using EP.Shared.Configuration;
using Microsoft.Extensions.Options;

namespace BackgroundProcessing.Workers.Show
{
    public class ShowProcessor : IShowProcessor, IDisposable
    {
        public readonly ILogger<ShowProcessor> Logger;
        public readonly IServiceProvider ServiceProvider;
        public readonly WorkerTimer WorkerTimer;

        public ShowProcessor(
            ILogger<ShowProcessor> logger,
            IServiceProvider serviceProvider,
            IOptionsMonitor<WorkerTimer> workerMonitor)

        {
            Logger = logger;
            ServiceProvider = serviceProvider;
            WorkerTimer = workerMonitor.CurrentValue;
        }

        public async Task DeleteShows(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                Logger.LogInformation("Deleted Shows");
                using var scope = ServiceProvider.CreateScope();
                var showService = scope.ServiceProvider.GetService<IShowProcessorService>();
                if (showService != null) await showService.DeleteEndTimeShows();
                await Task.Delay(1000 * WorkerTimer.ShowSyncTimer, cancellationToken);
            }
        }

        public void Dispose()
        {

        }
    }
}
