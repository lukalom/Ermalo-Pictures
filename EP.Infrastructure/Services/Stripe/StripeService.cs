using EP.Infrastructure.Services.Stripe.DTO;
using EP.Shared.Configuration;
using Microsoft.Extensions.Options;
using Stripe.Checkout;

namespace EP.Infrastructure.Services.Stripe
{
    public class StripeService : IStripeService
    {
        private readonly StripeConfig _stripeConfig;

        public StripeService(IOptionsMonitor<StripeConfig> stripeOptionsMonitor)
        {
            _stripeConfig = stripeOptionsMonitor.CurrentValue;
        }

        public async Task<StripePaymentDto> Pay(StripePaymentRequestDto requestDto)
        {
            var appUrl = _stripeConfig.PaymentConfirmation;
            var confirmUrl = string.Format(appUrl, requestDto.SessionData.First().TransactionUniqueCode);
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = confirmUrl,
                CancelUrl = string.Format(appUrl, "Failed"),
            };

            foreach (var data in requestDto.SessionData)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(data.Price * 100),
                        Currency = requestDto.Currency,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = data.Movie + " Ticket",
                            Description = $"Seat row {data.RowNumber} column {data.ColumnNumber}"
                        }
                    },
                    Quantity = 1
                };
                options.LineItems.Add(sessionLineItem);
            }

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            var stripePaymentDto = new StripePaymentDto()
            {
                SessionId = session.Id,
                PaymentUrl = session.Url
            };
            return stripePaymentDto;
        }
    }
}
