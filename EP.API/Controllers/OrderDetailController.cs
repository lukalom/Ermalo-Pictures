using EP.Application.Services.OrderDetail;
using EP.Application.Services.OrderDetail.DTO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EP.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrderDetailController : ControllerBase
    {
        private readonly IOrderDetailService _orderDetailService;

        public OrderDetailController(IOrderDetailService orderDetailService)
        {
            _orderDetailService = orderDetailService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromQuery] CreateOrderRequestDto createOrderDto)
        {
            var result = await _orderDetailService.CreateOrder(createOrderDto);
            if (result.IsSuccess)
            {
                return Ok(result.Content);
            }

            return BadRequest(result.Error);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserApprovedOrders()
        {
            var result = await _orderDetailService.GetUserApprovedOrders();
            if (result.IsSuccess)
            {
                return Ok(result.Content);
            }

            return BadRequest(result.Error);
        }


    }
}
