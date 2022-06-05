using EP.Application.Services.Cinema_Management.CinemaHall;
using EP.Application.Services.Cinema_Management.CinemaHall.DTO;
using EP.Application.Services.Cinema_Management.CinemaSeat;
using EP.Application.Services.Cinema_Management.CinemaSeat.DTO;
using EP.Infrastructure.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EP.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,SuperAdmin")]
    public class CinemaHallController : ControllerBase
    {
        private readonly ICinemaHallService _cinemaHallService;
        private readonly ICinemaSeatService _cinemaSeatService;
        public CinemaHallController(ICinemaHallService cinemaHallService, ICinemaSeatService cinemaSeatService)
        {
            _cinemaHallService = cinemaHallService;
            _cinemaSeatService = cinemaSeatService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get([FromQuery] GetCinemaHallByIdRequestDto requestDto)
        {
            var result = await _cinemaHallService.GetCinemaHallById(requestDto);
            if (result.IsSuccess)
            {
                return Ok(result.Content);
            }

            return BadRequest(result.Error);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(int cinemaId)
        {
            var result = await _cinemaHallService.GetAllCinemaHall(cinemaId);
            if (result.IsSuccess)
            {
                return Ok(result.Content);
            }

            return BadRequest(result.Error);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromQuery] CreateCinemaHallRequestDto requestDto)
        {
            var result = await _cinemaHallService.CreateCinemaHall(requestDto);
            if (result.IsSuccess)
            {
                return Ok(result.Content);
            }

            return BadRequest(result.Error);
        }

        //Add Seat Routes
        [HttpPost]
        public async Task<IActionResult> AddSeat([FromBody] AddCinemaSeatsRequestDto requestDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _cinemaSeatService.AddCinemaSeats(requestDto);
                if (result.IsSuccess)
                {
                    return Ok(result.Content);
                }

                return BadRequest(result.Error);
            }

            return BadRequestModelState();
        }

        [HttpGet]
        public async Task<IActionResult> GetSeats(int cinemaHallId)
        {
            var result = await _cinemaSeatService.GetSeats(cinemaHallId);
            if (result.IsSuccess)
            {
                return Ok(result.Content);
            }

            return BadRequest(result.Error);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSeatStatus(UpdateSeatStatusRequestDto requestDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _cinemaSeatService.UpdateSeatStatus(requestDto);
                if (result.IsSuccess)
                {
                    return Ok(result.Content);
                }

                return BadRequest(result.Error);
            }

            return BadRequestModelState();
        }

        private IActionResult BadRequestModelState()
        {
            var errorMessages = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));

            return BadRequest(new { error = errorMessages });
        }
    }
}
