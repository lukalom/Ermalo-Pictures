using EP.Infrastructure.IConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EP.Infrastructure.Extensions
{
    public static class SeederExtensions
    {

        public static void AddSeederService(this IServiceCollection services)
        {
            services.Scan(scan => scan
                .FromAssembliesOf(typeof(ISeeder))
                .AddClasses(classes => classes.AssignableTo<ISeeder>())
                .AsImplementedInterfaces()
                .WithScopedLifetime());
        }

        public static async Task Seed(this IHost host, IServiceProvider serviceProvider)
        {
            var dbInitializer = serviceProvider.GetRequiredService<IDbInitializer>();
            await dbInitializer.Initialize();
        }
    }
}
