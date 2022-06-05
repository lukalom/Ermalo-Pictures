using EP.Infrastructure.Services.Stripe.DTO;

namespace EP.Infrastructure.Services.Stripe
{
    public interface IStripeService
    {
        Task<StripePaymentDto> Pay(StripePaymentRequestDto requestDto);
    }
}
