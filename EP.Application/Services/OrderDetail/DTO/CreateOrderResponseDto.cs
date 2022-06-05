using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Services.OrderDetail.DTO
{
    public class CreateOrderResponseDto
    {
        public CreateOrderResponseDto()
        {
            SeatIdList = new List<int>();
        }

        public decimal TotalPrice { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public string Movie { get; set; }
        public List<int> SeatIdList { get; set; }
        public string CreatedDate { get; set; } = DateTime.Now.ToString("F");
    }
}
