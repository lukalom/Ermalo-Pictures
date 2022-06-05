using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Services.Cinema_Management.Cinema.DTO.Request
{
    public class RestoreCinemaRequestDto
    {
        [Required]
        public int Id { get; set; }
    }
}
