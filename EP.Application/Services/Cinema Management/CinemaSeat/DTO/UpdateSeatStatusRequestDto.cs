using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EP.Infrastructure.Enums;

namespace EP.Application.Services.Cinema_Management.CinemaSeat.DTO
{
    public class UpdateSeatStatusRequestDto
    {
        [Required]
        public int CinemaHallId { get; set; }
        [Required]
        public int SeatId { get; set; }
        [Required]
        public SeatType SeatType { get; set; }
    }
}
