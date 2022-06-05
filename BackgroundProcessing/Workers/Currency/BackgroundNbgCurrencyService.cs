namespace BackgroundProcessing.Workers.Currency
{
    public class BackgroundNbgCurrencyService : BackgroundService
    {
        private readonly INbgCurrencyProcessor _nbgCurrencyProcessor;

        public BackgroundNbgCurrencyService(INbgCurrencyProcessor nbgCurrencyProcessor)
        {
            _nbgCurrencyProcessor = nbgCurrencyProcessor;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _nbgCurrencyProcessor.ProcessCurrencyAsync(stoppingToken);
        }
    }
}
