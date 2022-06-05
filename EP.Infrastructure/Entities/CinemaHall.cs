using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EP.Infrastructure.Enums;
using EP.Infrastructure.IConfiguration;

namespace EP.Infrastructure.Entities
{
    public class CinemaHall : BaseEntity<int>
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [Range(10, 40, ErrorMessage = "Max Hall Seats length 40")]
        public int TotalSeats { get; set; }

        [Required]
        public int Rows { get; set; }

        [Required]
        public int Columns { get; set; }

        [Required]
        public int CinemaId { get; set; }
        [Display(Name = "Cinema")]
        [ForeignKey("CinemaId")]
        public Cinema Cinema { get; set; }
    }
}
