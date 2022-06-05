using EP.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EP.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Movie_Genre>()
                .HasOne(m => m.Movie)
                .WithMany(mg => mg.Movie_Genres)
                .HasForeignKey(mi => mi.MovieId);

            modelBuilder.Entity<Movie_Genre>()
                .HasOne(g => g.Genre)
                .WithMany(mg => mg.Movie_Genres)
                .HasForeignKey(gi => gi.GenreId);
        }

        public virtual DbSet<Movie> Movies { get; set; }
        public virtual DbSet<Genre> Genres { get; set; }
        public DbSet<Movie_Genre> Movies_Genres { get; set; }
        public virtual DbSet<Show> Shows { get; set; }
        public DbSet<CinemaHall> CinemaHalls { get; set; }
        public DbSet<CinemaSeat> CinemaSeats { get; set; }
        public DbSet<ShowSeat> ShowSeats { get; set; }
        public DbSet<DiscountCoupon> DiscountCoupons { get; set; }
        public virtual DbSet<OrderDetails> OrderDetails { get; set; }
        public virtual DbSet<Payments> Payments { get; set; }
        public virtual DbSet<RefreshToken> RefreshToken { get; set; }
        public DbSet<NbgCurrency> NbgCurrencies { get; set; }
    }

}
