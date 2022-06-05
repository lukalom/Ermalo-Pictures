using System.Security.Claims;
using AutoMapper;
using EP.Application.DTO_General.Extension;
using EP.Application.DTO_General.Generic;
using EP.Application.Services.Account.Profile.DTO.Request;
using EP.Application.Services.Account.Profile.DTO.Response;
using EP.Infrastructure.Entities;
using EP.Infrastructure.IConfiguration;
using EP.Shared.Exceptions.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace EP.Application.Services.Account.Profile
{
    public class ProfileService : IProfileService
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProfileService(
            IMapper mapper,
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<Result<GetProfileResponseDto>> GetProfile()
        {
            var result = new Result<GetProfileResponseDto>();

            var loggedInUserId = _httpContextAccessor.HttpContext.User.FindFirstValue("Id");

            if (loggedInUserId == null)
            {
                result.Error = ErrorHandler.PopulateError(400,
                    ErrorMessages.Profile.UserNotFound,
                    ErrorMessages.Generic.TypeBadRequest);


                return result;
            }

            var profile = await _userManager.FindByIdAsync(loggedInUserId);

            if (profile == null)
            {
                result.Error = ErrorHandler.PopulateError(400,
                    ErrorMessages.Profile.UserNotFound,
                    ErrorMessages.Generic.TypeBadRequest);

                return result;
            }

            var mappedProfile = _mapper.Map<GetProfileResponseDto>(profile);
            result.Content = mappedProfile;

            return result;
        }

        public async Task<Result<string>> UpdateUsername(string username)
        {
            var loggedInUserId = _httpContextAccessor.HttpContext.User.FindFirstValue("Id");
            var result = new Result<string>();

            if (loggedInUserId == null)
            {
                result.Error = ErrorHandler.PopulateError(400,
                    ErrorMessages.Profile.UserNotFound,
                    ErrorMessages.Generic.TypeBadRequest);

                return result;
            }

            var user = await _userManager.FindByIdAsync(loggedInUserId);


            if (user == null)
            {
                result.Error = ErrorHandler.PopulateError(400,
                    ErrorMessages.Profile.UserNotFound,
                    ErrorMessages.Generic.TypeBadRequest);

                return result;
            }

            user.UserName = username;
            var userResult = await _userManager.UpdateAsync(user);

            if (userResult.Succeeded)
            {
                result.Content = $"Profile Username Changed to {username}";
                return result;
            }

            result.Error = ErrorHandler.PopulateError(400,
                ErrorMessages.Generic.SomethingWentWrong,
                ErrorMessages.Generic.TypeBadRequest);

            return result;

        }

        public async Task<Result<string>> ChangePassword(ChangePasswordRequestDto changePasswordDto)
        {
            var loggedInUserId = _httpContextAccessor.HttpContext.User.FindFirstValue("Id");
            var result = new Result<string>();

            if (loggedInUserId == null)
            {
                result.Error = ErrorHandler.PopulateError(400,
                    ErrorMessages.Profile.UserNotFound,
                    ErrorMessages.Generic.TypeBadRequest);

                return result;
            }

            var user = await _userManager.FindByIdAsync(loggedInUserId);

            if (user == null)
            {
                result.Error = ErrorHandler.PopulateError(400,
                    ErrorMessages.Profile.UserNotFound,
                    ErrorMessages.Generic.TypeBadRequest);

                return result;
            }

            if (changePasswordDto.currentPassword == changePasswordDto.newPassword)
            {
                result.Error = ErrorHandler.PopulateError(400,
                    ErrorMessages.Generic.InvalidPayload,
                    ErrorMessages.Generic.TypeBadRequest);

                return result;
            }

            var passResult = await _userManager.ChangePasswordAsync(user, changePasswordDto.currentPassword, changePasswordDto.newPassword);

            if (passResult.Succeeded)
            {
                result.Content = "Password Has been Changed successfully";
                return result;
            }

            result.Error = ErrorHandler.PopulateError(400,
                ErrorMessages.Generic.SomethingWentWrong,
                ErrorMessages.Generic.TypeBadRequest);

            return result;
        }
    }
}
