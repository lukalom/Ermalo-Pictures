using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EP.Infrastructure.Entities;
using EP.Infrastructure.Enums;
using EP.Infrastructure.IConfiguration;
using Microsoft.EntityFrameworkCore;

namespace EP.Infrastructure.Data.DB_Seed
{
    public class MovieSeed : ISeeder
    {
        private readonly IUnitOfWork _unitOfWork;

        public MovieSeed(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public int Index { get; set; } = 6;

        public async Task Seed()
        {
            if (await _unitOfWork.Movie.Query().CountAsync() == 0)
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

                await _unitOfWork.Movie.AddRangeAsync(movies);
                await _unitOfWork.MovieGenres.AddRangeAsync(movieGenres);
            }
        }
    }
}
