using EP.Infrastructure.Entities;
using Newtonsoft.Json;

namespace EP.Application.Services.Currency.DTO
{
    public record NbgCurrencyJsonDto
    {
        [JsonProperty("date")]
        public DateTimeOffset Date { get; set; }

        [JsonProperty("currencies")]
        public List<NbgCurrency> Currencies { get; set; }
    }
}
