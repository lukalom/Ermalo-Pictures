using EP.Infrastructure.Entities;

namespace EP.Application.Services.Cinema_Management.Cinema.DTO.Response
{
    public class GetCinemaResponseDto
    {
        public GetCinemaResponseDto()
        {
            Shows = new List<Infrastructure.Entities.Show>();
            CinemaHall = new List<Infrastructure.Entities.CinemaHall>();
        }

        public List<Infrastructure.Entities.Show> Shows { get; set; }
        public List<Infrastructure.Entities.CinemaHall> CinemaHall { get; set; }
    }
}
