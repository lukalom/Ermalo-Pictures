using EP.Infrastructure.Entities;
using EP.Infrastructure.IConfiguration;
using EP.Infrastructure.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace EP.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ApplicationDbContext _db;
        private readonly IServiceProvider _serviceProvider;

        public UnitOfWork(ApplicationDbContext db,
            IServiceProvider serviceProvider)
        {
            _db = db;
            _serviceProvider = serviceProvider;
        }

        public IGenericRepository<Movie, int> Movie => _serviceProvider.GetRequiredService<IGenericRepository<Movie, int>>();

        public IGenericRepository<Show, int> Show => _serviceProvider.GetRequiredService<IGenericRepository<Show, int>>();

        public IGenericRepository<RefreshToken, int> RefreshTokens => _serviceProvider.GetRequiredService<IGenericRepository<RefreshToken, int>>();

        public IGenericRepository<CinemaHall, int> CinemaHall => _serviceProvider.GetRequiredService<IGenericRepository<CinemaHall, int>>();

        public IGenericRepository<Genre, int> Genre => _serviceProvider.GetRequiredService<IGenericRepository<Genre, int>>();

        public IGenericRepository<Movie_Genre, int> MovieGenres => _serviceProvider.GetRequiredService<IGenericRepository<Movie_Genre, int>>();

        public IGenericRepository<CinemaSeat, int> CinemaSeat => _serviceProvider.GetRequiredService<IGenericRepository<CinemaSeat, int>>();

        public IGenericRepository<Cinema, int> Cinema => _serviceProvider.GetRequiredService<IGenericRepository<Cinema, int>>();

        public IGenericRepository<ShowSeat, int> ShowSeat => _serviceProvider.GetRequiredService<IGenericRepository<ShowSeat, int>>();

        public IGenericRepository<OrderDetails, int> OrderDetail => _serviceProvider.GetRequiredService<IGenericRepository<OrderDetails, int>>();

        public IGenericRepository<Payments, int> Payment => _serviceProvider.GetRequiredService<IGenericRepository<Payments, int>>();

        public IGenericRepository<DiscountCoupon, int> DiscountCoupon => _serviceProvider.GetRequiredService<IGenericRepository<DiscountCoupon, int>>();

        public IGenericRepository<NbgCurrency, int> NbgCurrency => _serviceProvider.GetRequiredService<IGenericRepository<NbgCurrency, int>>();

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

        public bool HasChanges()
        {
            _db.ChangeTracker.DetectChanges();
            var changes = _db.ChangeTracker.HasChanges();

            return changes;
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
