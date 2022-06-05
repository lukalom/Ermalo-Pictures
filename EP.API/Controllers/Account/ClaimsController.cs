using EP.Application.Services.Account.Claims;
using EP.Application.Services.Account.Claims.DTO.Request;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EP.API.Controllers.Account
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,SuperAdmin")]
    public class ClaimsController : ControllerBase
    {
        private readonly IClaimsService _claimsService;

        public ClaimsController(IClaimsService claimsService)
        {
            _claimsService = claimsService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllClaims(string email)
        {
            var result = await _claimsService.GetAllClaims(email);

            if (result.IsSuccess)
            {
                return Ok(result.Content);
            }

            return BadRequest(result.Error);
        }

        [HttpPost("AddClaimsToUser")]
        public async Task<IActionResult> AddClaimsToUser(AddClaimsToUserRequestDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestModelState();
            }

            var result = await _claimsService.AddClaimsToUser(requestDto);

            if (result.IsSuccess)
            {
                return Ok(result.Content);
            }

            return BadRequest(result.Error);
        }

        private IActionResult BadRequestModelState()
        {
            var errorMessages = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));

            return BadRequest(new { error = errorMessages });
        }
    }
}
