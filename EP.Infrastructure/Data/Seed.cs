using System.Security.Claims;
using EP.Infrastructure.Entities;
using EP.Infrastructure.Enums;
using EP.Infrastructure.IConfiguration;
using Microsoft.AspNetCore.Identity;

namespace EP.Infrastructure.Data
{
    public static class Seed
    {
        public static async Task SeedUserAsync(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            //create roles if they are not created
            if (!roleManager.RoleExistsAsync(Role.AppUser.ToString()).GetAwaiter().GetResult())
            {
                roleManager.CreateAsync(new IdentityRole(Role.Admin.ToString())).GetAwaiter().GetResult();
                roleManager.CreateAsync(new IdentityRole(Role.SuperAdmin.ToString())).GetAwaiter().GetResult();
                roleManager.CreateAsync(new IdentityRole(Role.AppUser.ToString())).GetAwaiter().GetResult();

                //if roles are not created, then we will create admin user as well
                userManager.CreateAsync(new ApplicationUser()
                {
                    UserName = "LukaLom",
                    Email = "lukalomiashvili@gmail.com",
                    PhoneNumber = "111111123",
                    EmailConfirmed = true
                }, "String123!").GetAwaiter().GetResult();

                var user = await userManager.FindByEmailAsync("lukalomiashvili@gmail.com");
                userManager.AddToRoleAsync(user, Role.SuperAdmin.ToString()).GetAwaiter().GetResult();
                userManager.AddToRoleAsync(user, Role.AppUser.ToString()).GetAwaiter().GetResult();
                userManager.AddToRoleAsync(user, Role.Admin.ToString()).GetAwaiter().GetResult();

                var claimList = new List<Claim>()
                {
                    new("Role Management", "Role Management"),
                    new("Show Management", "Show Management")
                };
                await userManager.AddClaimsAsync(user, claimList);
            }
        }

        public static async Task SeedCinemaAsync(IUnitOfWork unitOfWork)
        {
            var cinema = new Cinema() { Name = "Cavea" };
            await unitOfWork.Cinema.AddAsync(cinema);
        }

        public static async Task SeedGenresAsync(IUnitOfWork unitOfWork)
        {
            var genres = new List<Genre>()
            {
                new (){Name = "Action"},
                new (){Name = "Drama"},
                new (){Name = "Comedy"},
                new (){Name = "Fantasy"},
                new (){Name = "Western"},
                new (){Name = "Georgian"},
                new (){Name = "Sci-Fi"},
                new (){Name = "Romantic"},

            };
            await unitOfWork.Genre.AddRangeAsync(genres);
        }

        public static async Task SeedMovieAsync(IUnitOfWork unitOfWork)
        {
            var movies = new List<Movie>
            {
                new(){
                    Title = "Gone Girl",
                    Description = "რეჟისორ დევიდ ფინჩერის 2014 წლის თრილერი. დაფუძნებულია ჯილიან ფლინის ამავე სახელწოდების რომანზე.",
                    DurationInMinutes = 149,
                    Director = "David Fincher",
                    ReleaseDate = new DateTime(2014, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                    Language = Language.Eng,
                    Country = "USA",
                    ImageUrl = "4c994d5a-687d-49c4-a195-4361121f3ff0.jpg"
                },
                new(){
                    Title = "The Batman",
                    Description = "A reclusive billionaire who obsessively protects Gotham City as a masked vigilante to cope with his traumatic past",
                    DurationInMinutes = 176,
                    Director = "Matt Reeves",
                    ReleaseDate = new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                    Language = Language.Eng,
                    Country = "USA",
                    ImageUrl = "234396bb-413c-4e11-902d-e95bf65a4441.jpg"
                },
            };

            var movieGenres = new List<Movie_Genre>
            {
                new(){MovieId = 1, GenreId = 1},
                new(){MovieId = 1, GenreId = 4},
                new(){MovieId = 2, GenreId = 1},
                new(){MovieId = 2, GenreId = 2},
                new(){MovieId = 2, GenreId = 3}
            };

            await unitOfWork.Movie.AddRangeAsync(movies);
            await unitOfWork.MovieGenres.AddRangeAsync(movieGenres);
        }

        public static async Task SeedCinemaHallAsync(IUnitOfWork unitOfWork)
        {
            var cinemaHall = new CinemaHall
            {
                CinemaId = 1,
                TotalSeats = 30,
                Name = "Hall 1",
                Columns = 10,
                Rows = 3
            };

            await unitOfWork.CinemaHall.AddAsync(cinemaHall);
        }

        public static async Task SeedCinemaSeatAsync(IUnitOfWork unitOfWork)
        {
            var cinemaSeats = new List<CinemaSeat>();
            for (int i = 1; i < 4; i++)
            {
                for (int j = 1; j < 11; j++)
                {
                    cinemaSeats.Add(new CinemaSeat()
                    {
                        CinemaHallId = 1,
                        ColumnNumber = j,
                        RowNumber = i,
                        SeatType = j % 2 == 0 ? SeatType.Medium : SeatType.Vip
                    });
                }
            }

            await unitOfWork.CinemaSeat.AddRangeAsync(cinemaSeats);
        }

        public static async Task SeedShowAsync(IUnitOfWork unitOfWork)
        {
            var show = new Show
            {
                Date = DateTime.Now,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(2),
                MovieId = 1,
                CinemaHallId = 1
            };
            await unitOfWork.Show.AddAsync(show);
            await unitOfWork.SaveAsync();
        }

        public static async Task SeedShowSeatAsync(IUnitOfWork unitOfWork)
        {
            var showSeatList = new List<ShowSeat>();
            //for (int i = 1; i < 1; i++)
            //{
            showSeatList.Add(new ShowSeat()
            {
                //Price = i % 2 == 0 ? 10 : 15,
                Price = 3.0m,
                //Status = ShowSeatStatus.Available,
                CinemaSeatId = 1,
                ShowId = 1
            });
            //}

            await unitOfWork.ShowSeat.AddRangeAsync(showSeatList);
        }

        public static async Task SeedDiscountCouponAsync(IUnitOfWork unitOfWork)
        {
            var discountCoupon = new DiscountCoupon { Code = "forstud", Uses = 10, Discount = 50 };
            await unitOfWork.DiscountCoupon.AddAsync(discountCoupon);
        }
    }
}
