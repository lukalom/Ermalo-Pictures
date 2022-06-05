using EP.Application.Services.Account.Auth;
using EP.Application.Services.Account.Claims;
using EP.Application.Services.Account.Profile;
using EP.Application.Services.Account.Role;
using EP.Application.Services.Cinema_Management.Cinema;
using EP.Application.Services.Cinema_Management.CinemaHall;
using EP.Application.Services.Cinema_Management.CinemaSeat;
using EP.Application.Services.DiscountCoupon;
using EP.Application.Services.Movie;
using EP.Application.Services.OrderDetail;
using EP.Application.Services.Payment;
using EP.Application.Services.QRCode;
using EP.Application.Services.Show;
using EP.Application.Services.ShowSeat;
using EP.Application.Services.SmsService;
using EP.Application.Services.UserContext;
using EP.Application.Token;
using EP.Infrastructure.Data;
using EP.Infrastructure.IConfiguration;
using EP.Infrastructure.Services.Currency;
using EP.Infrastructure.Services.Stripe;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace EP.Application.Extensions
{
    public static class AppServiceExtensions
    {
        public static IServiceCollection ApplicationServiceExtensions(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IMovieService, MovieService>();
            services.AddScoped<ICinemaHallService, CinemaHallService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IClaimsService, ClaimsService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped(typeof(ITokenManager), typeof(TokenManager));
            services.AddScoped<ICinemaService, CinemaService>();
            services.AddScoped<ICinemaSeatService, CinemaSeatService>();
            services.AddScoped<IShowService, ShowService>();
            services.AddScoped<IShowSeatService, ShowSeatService>();
            services.AddScoped<IOrderDetailService, OrderDetailService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<ISmsSender, SmsSender>();
            services.AddScoped<IDiscountCouponService, DiscountCouponService>();
            services.AddHttpClient();
            services.AddScoped<INbgCurrencyService, NbgCurrencyService>();
            services.AddScoped<IUserContext, UserContext>();
            services.AddScoped<IQrCodeService, QrCodeService>();
            services.AddScoped<IStripeService, StripeService>();

            return services;
        }
    }

}
