namespace EP.Application.Services.Show.DTO.Request
{
    public class CreateShowRequestDto
    {
        public DateTime StarTime { get; set; }
        public DateTime EndTime { get; set; }
        public int CinemaHallId { get; set; }
        public int MovieId { get; set; }
    }
}
