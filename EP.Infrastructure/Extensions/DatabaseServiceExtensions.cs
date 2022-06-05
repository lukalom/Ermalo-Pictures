using EP.Infrastructure.Data;
using EP.Infrastructure.Data.DB_Seed;
using EP.Infrastructure.IConfiguration;
using EP.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EP.Infrastructure.Extensions
{
    public static class DatabaseServiceExtensions
    {
        public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services,
            Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IDbInitializer, DbInitializer>();

            return services;
        }

        public static IServiceCollection AddGenericRepository(this IServiceCollection service)
        {
            //Infrastructure
            service.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));

            return service;
        }

    }
}