using EP.Shared.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EP.Shared.Extensions
{
    public static class SharedServiceExtensions
    {
        public static IServiceCollection ConfigurationServices(this IServiceCollection services,
            IConfiguration configuration, IWebHostEnvironment hostEnvironment)
        {
            services.Configure<JwtConfig>(configuration.GetSection(nameof(JwtConfig)));
            services.Configure<StripeConfig>(configuration.GetSection(nameof(StripeConfig)));
            services.Configure<SendGridConfig>(configuration.GetSection(nameof(SendGridConfig)));
            services.Configure<TwilioConfig>(configuration.GetSection(nameof(TwilioConfig)));
            services.Configure<FacebookConfig>(configuration.GetSection(nameof(FacebookConfig)));
            services.Configure<NbgConfig>(configuration.GetSection(nameof(NbgConfig)));
            services.Configure<EnvConfig>(x => x.WebRootPath = hostEnvironment.WebRootPath);
            services.Configure<QRCode>(configuration.GetSection(nameof(QRCode)));

            //shared
            services.AddSingleton<IEmailSender, EmailSender>();

            return services;
        }
    }
}
