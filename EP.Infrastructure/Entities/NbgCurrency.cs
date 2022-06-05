using EP.Infrastructure.IConfiguration;
using Newtonsoft.Json;

namespace EP.Infrastructure.Entities
{
    public class NbgCurrency : BaseEntity<int>
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("quantity")]
        public long Quantity { get; set; }

        [JsonProperty("rateFormated")]
        public string RateFormated { get; set; }

        [JsonProperty("diffFormated")]
        public string DiffFormated { get; set; }

        [JsonProperty("rate")]
        public double Rate { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("diff")]
        public double Diff { get; set; }

        [JsonProperty("date")]
        public DateTimeOffset Date { get; set; }

        [JsonProperty("validFromDate")]
        public DateTimeOffset ValidFromDate { get; set; }
    }
}
