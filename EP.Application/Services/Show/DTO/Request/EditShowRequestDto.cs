using System.ComponentModel.DataAnnotations;

namespace EP.Application.Services.Show.DTO.Request
{
    public class EditShowRequestDto
    {
        [Required]
        public int ShowId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        public int CinemaHallId { get; set; }

        [Required]
        public int MovieId { get; set; }
    }
}
