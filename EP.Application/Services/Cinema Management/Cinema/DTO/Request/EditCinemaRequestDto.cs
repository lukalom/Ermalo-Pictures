using System.ComponentModel.DataAnnotations;

namespace EP.Application.Services.Cinema_Management.Cinema.DTO.Response
{
    public class EditCinemaRequestDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
