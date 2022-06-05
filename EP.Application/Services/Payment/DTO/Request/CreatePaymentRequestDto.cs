using System.ComponentModel.DataAnnotations;
using EP.Infrastructure.Enums;

namespace EP.Application.Services.Payment.DTO.Request
{
    public class CreatePaymentRequestDto
    {
        public CreatePaymentRequestDto() => OrderIdList = new List<int>();

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        public PaymentCurrency CurrencyCode { get; set; } = PaymentCurrency.GEL;

        public string? DiscountCoupon { get; set; }

        [Required]
        public List<int> OrderIdList { get; set; }
    }
}
