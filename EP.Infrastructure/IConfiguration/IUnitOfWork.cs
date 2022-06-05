using EP.Infrastructure.Entities;
using EP.Infrastructure.Repository;

namespace EP.Infrastructure.IConfiguration
{
    public interface IUnitOfWork
    {
        public IGenericRepository<Movie, int> Movie { get; }
        public IGenericRepository<Show, int> Show { get; }
        public IGenericRepository<CinemaHall, int> CinemaHall { get; }
        public IGenericRepository<Genre, int> Genre { get; }
        public IGenericRepository<Movie_Genre, int> MovieGenres { get; }
        public IGenericRepository<RefreshToken, int> RefreshTokens { get; }
        public IGenericRepository<CinemaSeat, int> CinemaSeat { get; }
        public IGenericRepository<Cinema, int> Cinema { get; }
        public IGenericRepository<ShowSeat, int> ShowSeat { get; }
        public IGenericRepository<OrderDetails, int> OrderDetail { get; }
        public IGenericRepository<Payments, int> Payment { get; }
        public IGenericRepository<DiscountCoupon, int> DiscountCoupon { get; }
        public IGenericRepository<NbgCurrency, int> NbgCurrency { get; }


        Task SaveAsync();
        public bool HasChanges();
    }
}
