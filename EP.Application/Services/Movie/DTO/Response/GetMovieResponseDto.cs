using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Services.Movie.DTO.Response
{
    public class GetMovieResponseDto
    {
        public GetMovieResponseDto()
        {
            Genres = new List<string>();

        }

        public string Title { get; set; }

        public string Description { get; set; }

        public int DurationInMinutes { get; set; }

        public string Language { get; set; }

        public string ReleaseDate { get; set; }

        public string Country { get; set; }

        public string Director { get; set; }

        public List<string> Genres { get; set; }

        public string ImageUrl { get; set; }
    }

}
