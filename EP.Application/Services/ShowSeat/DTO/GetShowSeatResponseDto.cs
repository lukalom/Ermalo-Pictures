using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EP.Infrastructure.Entities;

namespace EP.Application.Services.ShowSeat.DTO
{
    public class GetShowSeatResponseDto
    {
        public int SeatId { get; set; }
        public string SeatStatus { get; set; }
        public string Seat_Type { get; set; }
        public string Price { get; set; }
        public string Movie { get; set; }
        public string CinemaHall { get; set; }
        public int RowNumber { get; set; }
        public int ColumnNumber { get; set; }
    }
}
