using EP.Application.Services.ShowSeat;
using EP.Application.Services.ShowSeat.DTO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EP.API.Controllers.Show_And_ShowSeat
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "ShowPolicy")]
    public class ShowSeatController : ControllerBase
    {
        private readonly IShowSeatService _showSeatService;

        public ShowSeatController(IShowSeatService showSeatService)
        {
            _showSeatService = showSeatService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllShowSeat(int showId)
        {
            var result = await _showSeatService.GetAllShowSeat(showId);
            if (result.IsSuccess)
            {
                return Ok(result.Content);
            }

            return BadRequest(result.Error);
        }

        [HttpPost]
        public async Task<IActionResult> CreateShowSeat([FromForm] CreateShowSeatRequestDto requestDto)
        {
            var result = await _showSeatService.CreateShowSeats(requestDto);
            if (result.IsSuccess)
            {
                return Ok(result.Content);
            }

            return BadRequest(result.Error);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteShowSeats(int showId)
        {
            var result = await _showSeatService.DeleteShowSeats(showId);
            if (result.IsSuccess)
            {
                return Ok(result.Content);
            }

            return BadRequest(result.Error);
        }

        [HttpPut]
        public async Task<IActionResult> ChangePriceToShowSeat(int showSeatId, decimal newPrice)
        {
            var result = await _showSeatService.ChangePriceToShowSeat(showSeatId, newPrice);
            if (result.IsSuccess)
            {
                return Ok(result.Content);
            }

            return BadRequest(result.Error);
        }
    }
}
