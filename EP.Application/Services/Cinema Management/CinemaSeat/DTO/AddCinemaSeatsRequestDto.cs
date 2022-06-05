using System.ComponentModel.DataAnnotations;
using EP.Infrastructure.Enums;

namespace EP.Application.Services.Cinema_Management.CinemaSeat.DTO
{
    public class AddCinemaSeatsRequestDto
    {
        public AddCinemaSeatsRequestDto()
        {
            SeatList = new List<AddSeatRequestDto>();
        }

        [Required]
        public int CinemaHallId { get; set; }

        [Required]
        public List<AddSeatRequestDto> SeatList { get; set; }

    }
}
