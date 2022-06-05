using System.ComponentModel.DataAnnotations;

namespace EP.Application.Services.Show.DTO.Response
{
    public class GetShowResponseDto
    {
        public string Date { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public string HallName { get; set; }

        public string Movie { get; set; }

    }
}
