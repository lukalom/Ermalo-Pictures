using EP.Application.Services.Currency.DTO;
using EP.Infrastructure.Services.Currency;
using Microsoft.AspNetCore.Mvc;

namespace EP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly INbgCurrencyService _currencyService;

        public CurrencyController(INbgCurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrencies() => Ok(await _currencyService.GetCurrencies());

        [HttpGet("Convert")]
        public async Task<IActionResult> Convert([FromQuery] ConvertCurrencyDto requestDto) =>
            Ok(await _currencyService.Convert(requestDto));
    }
}
