using EP.Application.Services.Payment;
using EP.Application.Services.Payment.DTO.Request;
using EP.Application.Services.SmsService;
using EP.Shared.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EP.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ISmsSender _smsSender;

        public PaymentController(
            IPaymentService paymentService,
            ISmsSender smsSender)
        {
            _paymentService = paymentService;
            _smsSender = smsSender;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromQuery] CreatePaymentRequestDto requestDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _paymentService.CreatePayment(requestDto);
                if (result.IsSuccess)
                {
                    //await _smsSender.SendSms("Payment Approved wish you a sweet session");
                    return Ok(result.Content);
                }

                return BadRequest(result.Error);
            }

            throw new AppException("Invalid Payload");
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> PaymentConfirmation(string uniqueCode)
        {
            if (string.IsNullOrEmpty(uniqueCode))
            {
                return BadRequest("Invalid Code");
            }

            var result = await _paymentService.ConfirmPayment(uniqueCode);

            if (result.Content) return Ok("Payment Confirmed");

            return BadRequest("Something went wrong payment is not Valid");
        }
    }
}
