using EP.Infrastructure.Entities;

namespace EP.Application.Services.Movie.DTO.Response
{
    public class CreateMovieResponseDto
    {
        public CreateMovieResponseDto()
        {
            Genres = new List<string?>();
        }
        public string Title { get; set; }

        public string Description { get; set; }

        public int DurationInMinutes { get; set; }

        public string Language { get; set; }

        public string ReleaseDate { get; set; }

        public string Country { get; set; }

        public string Director { get; set; }

        public string Image { get; set; }

        public List<string?> Genres { get; set; }

    }
}
