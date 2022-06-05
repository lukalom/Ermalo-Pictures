using EP.Application.Services.Show;
using EP.Application.Services.Show.DTO.Request;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EP.API.Controllers.Show_And_ShowSeat
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ShowController : ControllerBase
    {
        private readonly IShowService _showService;

        public ShowController(IShowService showService)
        {
            _showService = showService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetShows(int cinemaHallId)
        {
            var result = await _showService.GetShows(cinemaHallId);
            if (result.IsSuccess)
            {
                return Ok(result.Content);
            }

            return BadRequest(result.Error);
        }

        [HttpPost]
        [Authorize(Policy = "ShowPolicy")]
        public async Task<IActionResult> CreateShow([FromForm] CreateShowRequestDto requestDto)
        {
            var result = await _showService.CreateShow(requestDto);
            if (result.IsSuccess)
            {
                return Ok(result.Content);
            }

            return BadRequest(result.Error);
        }

        [HttpDelete]
        [Authorize(Policy = "ShowPolicy")]
        public async Task<IActionResult> DeleteShow(int showId)
        {
            var result = await _showService.DeleteShow(showId);
            if (result.IsSuccess)
            {
                return Ok(result.Content);
            }

            return BadRequest(result.Error);
        }

        [HttpPut]
        [Authorize(Policy = "ShowPolicy")]
        public async Task<IActionResult> EditShow(EditShowRequestDto requestDto)
        {
            var result = await _showService.EditShow(requestDto);
            if (result.IsSuccess)
            {
                return Ok(result.Content);
            }

            return BadRequest(result.Error);
        }
    }
}
