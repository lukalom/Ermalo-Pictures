using System.ComponentModel.DataAnnotations;

namespace EP.Application.Services.Currency.DTO
{
    public record ConvertCurrencyDto([Required] string Code, double AmountToGel);
}
