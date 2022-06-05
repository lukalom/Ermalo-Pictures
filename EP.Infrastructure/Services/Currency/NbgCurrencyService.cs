using EP.Application.Services.Currency.DTO;
using EP.Infrastructure.Entities;
using EP.Infrastructure.IConfiguration;
using EP.Shared.Configuration;
using EP.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace EP.Infrastructure.Services.Currency
{
    public class NbgCurrencyService : INbgCurrencyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<NbgCurrencyService> _logger;
        private readonly NbgConfig _nbgConfig;

        public NbgCurrencyService(
            IUnitOfWork unitOfWork,
            IOptionsMonitor<NbgConfig> options,
            IHttpClientFactory httpClientFactory,
            ILogger<NbgCurrencyService> logger)
        {
            _unitOfWork = unitOfWork;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _nbgConfig = options.CurrentValue;
        }

        public async Task<List<NbgCurrency>> FetchCurrencies()
        {
            try
            {
                var result = await _httpClientFactory.CreateClient().GetAsync(_nbgConfig.CurrencyUrl);
                result.EnsureSuccessStatusCode();
                var responseAsString = await result.Content.ReadAsStringAsync();
                NbgCurrencyJsonDto? json = JsonConvert.DeserializeObject<List<NbgCurrencyJsonDto>>(responseAsString)?[0];
                if (json == null) throw new AppException("Json is null here");

                return json.Currencies;
            }
            catch (Exception e)
            {
                throw new AppException(e.Message);
            }

        }

        public async Task<List<NbgCurrency>> GetCurrencies()
        {
            var currencies = await FetchCurrencies();
            if (currencies == null) return await _unitOfWork.NbgCurrency.Query().ToListAsync();
            return currencies;
        }

        public async Task<double> Convert(ConvertCurrencyDto requestDto)
        {
            var currency = await _unitOfWork.NbgCurrency
                .FindByCondition(x => x.Code.ToLower() == requestDto.Code.ToLower())
                .FirstOrDefaultAsync();

            if (currency == null)
            {
                _logger.LogError($"We don't have currency with this code = {requestDto.Code}");
                throw new AppException($"We don't have currency with this code = {requestDto.Code}");
            }

            if (requestDto.AmountToGel <= 1)
            {
                _logger.LogError("We are not converting under 1 Gel");
                throw new AppException("We are not converting under 1 Gel");
            }

            var convertedValue = requestDto.AmountToGel / currency.Rate;
            return convertedValue;
        }

    }
}
