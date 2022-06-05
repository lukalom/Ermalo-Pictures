namespace BackgroundProcessing.Jobs.NbgCurrency
{
    public interface INbgCurrencyProcessorService
    {
        Task<bool> UpdateCurrencyDatabase();
    }
}
