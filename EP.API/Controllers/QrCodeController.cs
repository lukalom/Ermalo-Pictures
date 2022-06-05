using EP.Application.Services.QRCode;
using Microsoft.AspNetCore.Mvc;

namespace EP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QrCodeController : ControllerBase
    {
        private readonly IQrCodeService _qrCodeService;

        public QrCodeController(IQrCodeService qrCodeService)
        {
            _qrCodeService = qrCodeService;
        }

        [HttpGet("{uniqueCode}")]
        public async Task<IActionResult> CheckQrCode(string uniqueCode)
        {
            if (string.IsNullOrEmpty(uniqueCode))
            {
                return BadRequest("Invalid QrCode");
            }

            var result = await _qrCodeService.CheckQrCode(uniqueCode);

            if (result.Content)
            {
                return Ok("Successful");
                return Redirect("https://localhost:5001/swagger/index.html");
            }

            return BadRequest("Something went wrong payment is not Valid");
        }
    }
}
