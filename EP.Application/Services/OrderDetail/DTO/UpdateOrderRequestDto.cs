using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EP.Infrastructure.Enums;

namespace EP.Application.Services.OrderDetail.DTO
{
    public class UpdateOrderRequestDto
    {
        public string UserEmail { get; set; }
        public OrderStatus Status { get; set; }
    }
}
