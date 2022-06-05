namespace EP.Application.Services.Cinema_Management.CinemaHall.DTO
{
    public class GetCinemaHallResponseDto
    {
        public int totalSeat { get; set; }
        public string hallName { get; set; }
        public int rows { get; set; }
        public int columns { get; set; }
    }
}
