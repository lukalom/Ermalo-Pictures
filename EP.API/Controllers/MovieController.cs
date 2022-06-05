using EP.Application.Services.Movie;
using EP.Application.Services.Movie.DTO.Request;
using EP.Application.Services.Movie.DTO.Response;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,SuperAdmin")]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService _movieService;
        public MovieController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(int? id)
        {
            var result = await _movieService.GetById(id);
            if (result.IsSuccess)
            {
                return Ok(result.Content);
            }

            return BadRequest(result.Error);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] MovieParameters movieParameters)
        {
            var result = await _movieService.GetAll(movieParameters);
            if (result.IsSuccess)
            {
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.Content.MetaData));
                return Ok(result.Content.Movies);
            }

            return BadRequest(result.Error);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] MovieUploadRequestDto modelDto)
        {
            var result = await _movieService.CreateMovie(modelDto);
            if (result.IsSuccess)
            {
                return Ok(result.Content);
            }

            return BadRequest(result.Error);
        }

        [HttpPut]
        public async Task<IActionResult> Edit([FromForm] EditMovieDto model)
        {
            var result = await _movieService.EditMovie(model);
            if (result.IsSuccess)
            {
                return Ok(result.Content);
            }

            return BadRequest(result.Error);

        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {
            var result = await _movieService.Delete(id);
            if (result.IsSuccess)
            {
                return Ok(result.Content);
            }

            return BadRequest(result.Error);
        }

    }
}
