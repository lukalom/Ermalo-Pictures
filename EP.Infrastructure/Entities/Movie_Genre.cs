using System.ComponentModel.DataAnnotations;
using EP.Infrastructure.IConfiguration;

namespace EP.Infrastructure.Entities
{
    public class Movie_Genre : BaseEntity<int>
    {
        [Required]
        public int GenreId { get; set; }
        public Genre Genre { get; set; }

        [Required]
        public int MovieId { get; set; }
        public Movie Movie { get; set; }

    }
}
