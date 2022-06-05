using EP.Application.DTO_General.Generic;
using EP.Application.Services.Account.Profile.DTO.Request;
using EP.Application.Services.Account.Profile.DTO.Response;

namespace EP.Application.Services.Account.Profile
{
    public interface IProfileService
    {
        Task<Result<GetProfileResponseDto>> GetProfile();
        Task<Result<string>> ChangePassword(ChangePasswordRequestDto changePasswordDto);
        Task<Result<string>> UpdateUsername(string username);
    }
}
