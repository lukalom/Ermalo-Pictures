using EP.Application.Services.Account.Profile;
using EP.Application.Services.Account.Profile.DTO.Request;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EP.API.Controllers.Account
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet]
        [Authorize(Policy = "OnlyNonBlockedUser")]
        public async Task<IActionResult> GetProfile()
        {
            var result = await _profileService.GetProfile();
            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result.Error);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUsername(string username)
        {
            var result = await _profileService.UpdateUsername(username);
            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result.Error);
        }

        [HttpPut]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequestDto changePasswordDto)
        {
            var result = await _profileService.ChangePassword(changePasswordDto);
            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result.Error);
        }
    }
}
