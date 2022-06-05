namespace BackgroundProcessing.Workers.Currency
{
    public interface INbgCurrencyProcessor
    {
        Task ProcessCurrencyAsync(CancellationToken cancellationToken);
    }
}
