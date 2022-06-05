using System.ComponentModel.DataAnnotations;

namespace EP.Application.Services.Cinema_Management.CinemaHall.DTO
{
    public class CreateCinemaHallRequestDto
    {
        [Required]
        public int cinemaId { get; set; }
        [Required]
        public string hallName { get; set; }
        [Required]
        public int totalSeats { get; set; }

        [Required]
        public int Rows { get; set; }

        [Required]
        public int Columns { get; set; }
    }
}
