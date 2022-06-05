using EP.Application.DTO_General.Generic;
using EP.Application.Services.Account.Auth.DTO.Request;
using EP.Application.Services.Account.Auth.DTO.Response;
using EP.Application.Services.Account.DTO.Response;
using EP.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EP.Application.Services.Account.Auth
{
    public interface IAuthService
    {
        Task<Result<UserRegisterResponseDto>> Register(UserRegistrationRequestDto registrationDto);
        Task<Result<UserLoginResponseDto>> Login([FromBody] UserLoginRequestDto loginDto);
        Task<Result<AuthResult>> VerifyTokenAndUpdate(TokenRequestDto tokenRequestDto);
        Task<AuthResult> GenerateJwtToken(ApplicationUser user);
        Task<Result<bool>> RevokeAll(Guid userId, string token);
        Task<IdentityResult> ConfirmEmailAsync(ConfirmEmailRequestDto confirmDto);
        Task SendEmailConfirmation(ApplicationUser user, string token);
        Task<FacebookTokenValidationResult> ValidateFbAccessTokenAsync(string accessToken);
        Task<FacebookUserInfoResult> GetFbUserInfoAsync(string accessToken);
    }
}
