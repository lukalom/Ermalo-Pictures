using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EP.Infrastructure.Enums;
using EP.Infrastructure.IConfiguration;


namespace EP.Infrastructure.Entities
{
    public class CinemaSeat : BaseEntity<int>
    {
        [Required]
        public SeatType SeatType { get; set; }

        public int RowNumber { get; set; }

        public int ColumnNumber { get; set; }

        [Required]
        public int CinemaHallId { get; set; }
        [Display(Name = "Cinema Hall")]
        [ForeignKey("CinemaHallId")]
        public CinemaHall CinemaHall { get; set; }
    }
}
