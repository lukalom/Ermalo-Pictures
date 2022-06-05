using System.ComponentModel.DataAnnotations;

namespace EP.Application.Services.Cinema_Management.Cinema.DTO.Request
{
    public class AddCinemaRequestDto
    {
        [Required]
        public string Name { get; set; }
    }
}
