namespace EP.Infrastructure.Services.Stripe.DTO
{
    public class StripePaymentRequestDto
    {
        public List<StripeSessionDto> SessionData { get; set; }
        public string Currency { get; set; }
    }
}
