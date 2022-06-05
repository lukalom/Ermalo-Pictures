using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EP.Infrastructure.IConfiguration;

namespace EP.Infrastructure.Entities
{
    public class Show : BaseEntity<int>
    {
        [Required]
        public DateTime Date { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        [Display(Name = "Cinema Hall")]
        public int CinemaHallId { get; set; }
        [ForeignKey("CinemaHallId")]
        public CinemaHall CinemaHall { get; set; }

        [Required]
        [Display(Name = "Movie")]
        public int MovieId { get; set; }
        [ForeignKey("MovieId")]
        public Movie Movie { get; set; }
    }
}
