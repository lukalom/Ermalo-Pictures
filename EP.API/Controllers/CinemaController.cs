using EP.Application.Services.Cinema_Management.Cinema;
using EP.Application.Services.Cinema_Management.Cinema.DTO.Request;
using EP.Application.Services.Cinema_Management.Cinema.DTO.Response;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EP.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,SuperAdmin")]
    public class CinemaController : ControllerBase
    {
        private readonly ICinemaService _cinemaService;

        public CinemaController(ICinemaService cinemaService)
        {
            _cinemaService = cinemaService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCinema(string name)
        {
            var result = await _cinemaService.GetCinema(name);
            if (result.IsSuccess)
            {
                return Ok(result.Content);
            }

            return BadRequest(result.Error);
        }

        [HttpPut]
        public async Task<IActionResult> EditCinema(EditCinemaRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Wrong request");
            }
            var result = await _cinemaService.EditCinema(request);
            if (result.IsSuccess)
            {
                return Ok(result.Content);
            }

            return BadRequest(result.Error);
        }

        [HttpPost]
        public async Task<IActionResult> AddCinema(AddCinemaRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _cinemaService.AddCinema(request);
            if (result.IsSuccess)
            {
                return Ok(result.Content);
            }

            return BadRequest(result.Error);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCinema(DeleteCinemaRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _cinemaService.DeleteCinema(request);
            if (result.IsSuccess)
            {
                return Ok(result.Content);
            }

            return BadRequest(result.Error);
        }

        [HttpPatch]
        public async Task<IActionResult> RestoreCinema(RestoreCinemaRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _cinemaService.RestoreCinema(request);
            if (result.IsSuccess)
            {
                return Ok(result.Content);
            }

            return BadRequest(result.Error);
        }

    }
}
