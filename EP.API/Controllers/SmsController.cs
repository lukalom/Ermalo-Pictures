using EP.Application.Services.SmsService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Twilio.AspNet.Core;

namespace EP.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,SuperAdmin")]
    public class SmsController : TwilioController
    {
        private readonly ISmsSender _smsSender;

        public SmsController(ISmsSender smsSender)
        {
            _smsSender = smsSender;
        }

        [HttpGet]
        public async Task<IActionResult> SendSms(string message)
        {
            var result = await _smsSender.SendSms(message);
            return Ok(result.Content);
        }
    }
}

