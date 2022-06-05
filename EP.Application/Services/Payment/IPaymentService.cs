using EP.Application.DTO_General.Generic;
using EP.Application.Services.Payment.DTO.Request;
using EP.Application.Services.Payment.DTO.Response;

namespace EP.Application.Services.Payment
{
    public interface IPaymentService
    {
        Task<Result<List<CreatePaymentResponseDto>>> CreatePayment(CreatePaymentRequestDto requestDto);
        Task<Result<bool>> ConfirmPayment(string uniqueCode);
    }
}
