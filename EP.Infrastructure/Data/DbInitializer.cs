using EP.Infrastructure.IConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EP.Infrastructure.Data
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<DbInitializer> _logger;
        private readonly IServiceProvider _serviceProvider;

        public DbInitializer(
            ApplicationDbContext db,
            ILogger<DbInitializer> logger,
            IServiceProvider serviceProvider)
        {
            _db = db;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task Initialize()
        {
            try
            {   //apply migration if they are not applied already
                if ((await _db.Database.GetPendingMigrationsAsync()).Any())
                {
                    await _db.Database.MigrateAsync();

                    var seeders = _serviceProvider.GetServices(typeof(ISeeder)).ToList()
                        .Where(x => x != null).Select(x => x as ISeeder).OrderBy(x => x?.Index);
                    if (seeders.Any())
                    {
                        foreach (var item in seeders)
                        {
                            if (item is ISeeder seeder) await seeder.Seed();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw new Exception(e.Message);
            }
        }
    }
}
