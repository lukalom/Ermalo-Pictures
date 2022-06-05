using System.ComponentModel.DataAnnotations;

namespace EP.Application.Services.Cinema_Management.Cinema.DTO.Request
{
    public class DeleteCinemaRequestDto
    {
        [Required]
        public int Id { get; set; }

    }
}
