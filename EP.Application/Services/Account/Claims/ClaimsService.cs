using System.Security.Claims;
using EP.Application.DTO_General.Extension;
using EP.Application.DTO_General.Generic;
using EP.Application.Services.Account.Claims.DTO.Request;
using EP.Infrastructure.Entities;
using EP.Infrastructure.Enums;
using EP.Shared.Exceptions.Messages;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace EP.Application.Services.Account.Claims
{
    public class ClaimsService : IClaimsService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ClaimsService> _logger;

        public ClaimsService(
            UserManager<ApplicationUser> userManager,
            ILogger<ClaimsService> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<Result<List<string>>> GetAllClaims(string email)
        {
            var result = new Result<List<string>>();
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogInformation($"The user with the {email} does not exists");
                result.Error = ErrorHandler.PopulateError(
                    (int)StatusCode.NotFound,
                    ErrorMessages.Generic.InvalidPayload,
                    ErrorMessages.Generic.TypeBadRequest);

                return result;
            }

            var userClaims = await _userManager.GetClaimsAsync(user);

            result.Content = userClaims.Select(x => $"Name = {x.Type} : Value = {x.Value}").ToList();

            return result;
        }

        public async Task<Result<string>> AddClaimsToUser(AddClaimsToUserRequestDto requestDto)
        {
            var result = new Result<string>();
            var user = await _userManager.FindByEmailAsync(requestDto.Email);
            if (user == null)
            {
                _logger.LogInformation($"The user with the {requestDto.Email} does not exists");
                result.Error = ErrorHandler.PopulateError(
                    (int)StatusCode.NotFound,
                    ErrorMessages.Generic.InvalidPayload,
                    ErrorMessages.Generic.TypeBadRequest);

                return result;
            }

            var userClaim = new Claim(requestDto.ClaimName, requestDto.ClaimValue);

            var resultClaim = await _userManager.AddClaimAsync(user, userClaim);

            if (resultClaim.Succeeded)
            {
                result.Content = $"User {user.Email} has a claim {requestDto.ClaimName}";

                return result;
            }

            _logger.LogInformation($"The user with the {requestDto.Email} does not exists");
            result.Error = ErrorHandler.PopulateError(
                (int)StatusCode.NotFound,
                $"Unable to add claim {requestDto.ClaimName} to the user {user.Email}",
                ErrorMessages.Generic.TypeBadRequest);

            return result;
        }

    }
}
