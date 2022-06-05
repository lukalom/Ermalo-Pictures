using System.Security.Claims;
using EP.Application.Services.Account.Auth;
using EP.Application.Services.Account.Auth.DTO.Request;
using EP.Infrastructure.Entities;
using EP.Infrastructure.Enums;
using EP.Shared.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace EP.API.Controllers.Account
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public AuthController(IAuthService authService,
            UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _authService = authService;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail([FromHeader] ConfirmEmailRequestDto confirmDto)
        {
            if (string.IsNullOrEmpty(confirmDto.userId) && string.IsNullOrEmpty(confirmDto.token))
            {
                return BadRequestModelState();
            }

            confirmDto.token = confirmDto.token.Replace(' ', '+');
            var result = await _authService.ConfirmEmailAsync(confirmDto);
            if (result.Succeeded)
            {
                return Redirect("https://localhost:5001/swagger/index.html");
                //return Ok("Your email has been verified successfully");
            }

            return result.Errors.Any() ? BadRequest(result.Errors) : BadRequest("Something went wrong email is not verified");
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDto registrationRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestModelState();
            }

            var result = await _authService.Register(registrationRequestDto);

            if (result.IsSuccess)
            {
                return Ok(result.Content);
            }

            return BadRequest(result.Error);
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDto loginRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestModelState();
            }

            var result = await _authService.Login(loginRequestDto);

            if (result.IsSuccess)
            {
                return Ok(result.Content);
            }

            return BadRequest(result.Error);

        }

        [HttpPost]
        public async Task<IActionResult> RefreshToken(TokenRequestDto tokenRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestModelState();
            }

            var result = await _authService.VerifyTokenAndUpdate(tokenRequestDto);
            if (result.IsSuccess)
            {
                return Ok(result.Content);
            }

            return BadRequest(result.Error);

        }

        [HttpPost]
        public async Task<IActionResult> LoginWithFacebook(string accessToken)
        {
            var validatedTokenResult = await _authService.ValidateFbAccessTokenAsync(accessToken);

            if (!validatedTokenResult.Data.IsValid) throw new AppException("Invalid Facebook token");

            var userInfo = await _authService.GetFbUserInfoAsync(accessToken);

            var user = await _userManager.FindByEmailAsync(userInfo.Email);
            if (user == null)
            {
                var appUser = new ApplicationUser()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = userInfo.Email,
                    UserName = userInfo.Email,
                    EmailConfirmed = true
                };

                var createdResult = await _userManager.CreateAsync(appUser);
                //createdResult.Errors
                if (!createdResult.Succeeded) throw new AppException("Something went wrong");
                var roleResult = await _userManager.AddToRoleAsync(appUser, Role.AppUser.ToString());
                if (!roleResult.Succeeded) throw new AppException("Something went wrong");
                var registerUser = await _authService.GenerateJwtToken(appUser);
                if (!registerUser.Success) throw new Exception("Something went wrong token not generated");
                await _emailSender.SendEmailAsync(appUser.Email, "Ermalo Pictures",
                    $"<h1>Welcome to Ermalo Pictures your facebook account has been created successfully to our app ;)</h1>");
                return Ok(registerUser);
            }

            var result = await _authService.GenerateJwtToken(user);
            if (result.Success)
            {
                await _emailSender.SendEmailAsync(user.Email, "Ermalo Pictures",
                    $"<h1>Welcome to Ermalo Pictures you just logged in with facebook ;)</h1>");
                return Ok(result);
            };

            throw new Exception("Something went wrong token not generated");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var id = HttpContext.User.FindFirstValue("Id");
            string authHeader = HttpContext.Request.Headers["Authorization"];

            if (id == null || string.IsNullOrEmpty(authHeader))
            {
                return Unauthorized();
            }

            var token = authHeader.Split(" ")[1];

            var result = await _authService.RevokeAll(new Guid(id), token);

            if (result.IsSuccess)
            {
                Request.Headers["Authorization"] = string.Empty;
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