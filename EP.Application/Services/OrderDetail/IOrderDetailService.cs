using EP.Application.DTO_General.Generic;
using EP.Application.Services.OrderDetail.DTO;

namespace EP.Application.Services.OrderDetail
{
    public interface IOrderDetailService
    {
        Task<Result<CreateOrderResponseDto>> CreateOrder(CreateOrderRequestDto createOrderDto);
        Task<Result<List<GetUserApprovedOrderResponseDto>>> GetUserApprovedOrders();
    }
}
