using AutoMapper;
using EP.Infrastructure.IConfiguration;
using EP.Infrastructure.Services.Currency;
using Microsoft.EntityFrameworkCore;

namespace BackgroundProcessing.Jobs.NbgCurrency
{
    public class NbgCurrencyProcessorService : INbgCurrencyProcessorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INbgCurrencyService _nbgCurrencyService;
        private readonly IMapper _mapper;

        public NbgCurrencyProcessorService(
            IUnitOfWork unitOfWork,
            INbgCurrencyService nbgCurrencyService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _nbgCurrencyService = nbgCurrencyService;
            _mapper = mapper;
        }

        public async Task<bool> UpdateCurrencyDatabase()
        {
            var nbgCurrencies = await _nbgCurrencyService.FetchCurrencies();
            if (await _unitOfWork.NbgCurrency.Query().CountAsync() == 0)
            {
                await _unitOfWork.NbgCurrency.AddRangeAsync(nbgCurrencies);
                await _unitOfWork.SaveAsync();
                return true;
            }

            if (nbgCurrencies.Any())
            {
                var currenciesFromDb = await _unitOfWork.NbgCurrency.Query().ToListAsync();

                foreach (var curCurrency in currenciesFromDb)
                {
                    foreach (var nbgCurrency in nbgCurrencies.Where(nbgCurrency => nbgCurrency.Code.ToLower() == curCurrency.Code.ToLower()))
                    {
                        _mapper.Map(nbgCurrency, curCurrency);
                    }
                }

                await _unitOfWork.SaveAsync();
                return true;
            }

            return false;
        }
    }
}
