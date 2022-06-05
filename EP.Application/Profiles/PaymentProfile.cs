using AutoMapper;
using EP.Application.Services.Payment.DTO.Response;
using EP.Infrastructure.Services.Stripe.DTO;

namespace EP.Application.Profiles
{
    public class PaymentProfile : Profile
    {
        public PaymentProfile()
        {
            CreateMap<CreatePaymentResponseDto, StripeSessionDto>();
        }
    }
}
