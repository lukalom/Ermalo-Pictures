using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Services.OrderDetail.DTO
{
    public class GetUserApprovedOrderResponseDto
    {
        public decimal TotalPrice { get; set; }
        public decimal TicketPrice { get; set; }
        public string Status { get; set; }
        public string Movie { get; set; }
        public string Seat { get; set; }
        public string CreatedDate { get; set; }
    }
}
