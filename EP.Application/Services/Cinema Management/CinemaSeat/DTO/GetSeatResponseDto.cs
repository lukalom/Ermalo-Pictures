using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Services.Cinema_Management.CinemaSeat.DTO
{
    public class GetSeatResponseDto
    {
        public string Type { get; set; }
        public string Hall { get; set; }
        public string Seat { get; set; }
    }
}
