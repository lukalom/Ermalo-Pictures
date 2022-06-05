using EP.API.Security.Handlers;
using Microsoft.AspNetCore.Authorization;

namespace EP.API.Extensions
{
    public static class ApiServicesExtensions
    {
        public static IServiceCollection ApiServicesCollection(this IServiceCollection services)
        {
            //Auth Requirements
            services.AddSingleton<IAuthorizationHandler, UserBlockedStatusHandler>();
            services.AddSingleton<IAuthorizationHandler, ShowManagementHandler>();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed(origin => true)
                    .AllowCredentials()
                    .WithOrigins("http://localhost:4200/"));
            });

            return services;
        }
    }
}
