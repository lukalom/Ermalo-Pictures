using EP.Application.Services.Currency.DTO;
using EP.Infrastructure.Entities;

namespace EP.Infrastructure.Services.Currency
{
    public interface INbgCurrencyService
    {
        Task<List<NbgCurrency>> FetchCurrencies();
        Task<List<NbgCurrency>> GetCurrencies();
        Task<double> Convert(ConvertCurrencyDto requestDto);
    }
}
