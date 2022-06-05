using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EP.Infrastructure.Entities;

namespace EP.Application.Services.ShowSeat.DTO
{
    public class CreateShowSeatRequestDto
    {
        public CreateShowSeatRequestDto()
        {
            CinemaSeatIdList = new List<int>();
        }

        public List<int> CinemaSeatIdList { get; set; }

        public int ShowId { get; set; }

        public decimal VipSeatPrice { get; set; }

        public decimal NormalSeatPrice { get; set; }
    }
}
