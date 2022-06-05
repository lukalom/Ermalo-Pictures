namespace EP.Infrastructure.Services.Stripe.DTO
{
    public class StripePaymentDto
    {
        public string PaymentUrl { get; set; }
        public string SessionId { get; set; }
    }
}
