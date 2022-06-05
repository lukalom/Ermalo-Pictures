using BackgroundProcessing.Jobs.NbgCurrency;
using BackgroundProcessing.Jobs.OrderDetails;
using BackgroundProcessing.Jobs.Payment;
using BackgroundProcessing.Jobs.Show;
using BackgroundProcessing.Workers.Currency;
using BackgroundProcessing.Workers.OrderDetails;
using BackgroundProcessing.Workers.Payment;
using BackgroundProcessing.Workers.Show;
using EP.Infrastructure.Data;
using EP.Infrastructure.Entities;
using EP.Infrastructure.IConfiguration;
using EP.Infrastructure.Services.Currency;
using EP.Shared.Configuration;
using Microsoft.AspNetCore.Identity;

namespace BackgroundProcessing.Extensions
{
    public static class BackgroundServiceExtensions
    {
        public static IServiceCollection BackgroundProcessServiceExtensions(this IServiceCollection services)
        {
            //Service to work
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddHttpClient();
            services.AddScoped<IPaymentProcessorService, PaymentProcessorService>();
            services.AddScoped<IOrderDetailsProcessorService, OrderDetailsProcessorService>();
            services.AddScoped<IShowProcessorService, ShowProcessorService>();
            services.AddScoped<INbgCurrencyProcessorService, NbgCurrencyProcessorService>();
            services.AddScoped<INbgCurrencyService, NbgCurrencyService>();

            //Background Processor
            services.AddHostedService<BackgroundShowDeleterService>();
            services.AddHostedService<BackgroundOrderDetailsService>();
            services.AddHostedService<BackgroundPaymentService>();
            services.AddHostedService<BackgroundNbgCurrencyService>();

            //Workers
            services.AddSingleton<IShowProcessor, ShowProcessor>();
            services.AddSingleton<IOrderDetailsProcessor, OrderDetailsProcessor>();
            services.AddSingleton<IPaymentProcessor, PaymentProcessor>();
            services.AddSingleton<INbgCurrencyProcessor, NbgCurrencyProcessor>();

            //Auth
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                    options.SignIn.RequireConfirmedAccount = true)
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            return services;
        }

        public static IServiceCollection ConfigurationServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<NbgConfig>(configuration.GetSection(nameof(NbgConfig)));
            services.Configure<WorkerTimer>(configuration.GetSection(nameof(WorkerTimer)));

            return services;
        }

    }
}
