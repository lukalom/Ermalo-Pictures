using EP.Application.DTO_General.Generic;
using EP.Application.Services.Account.Claims.DTO.Request;

namespace EP.Application.Services.Account.Claims
{
    public interface IClaimsService
    {
        Task<Result<List<string>>> GetAllClaims(string email);
        Task<Result<string>> AddClaimsToUser(AddClaimsToUserRequestDto requestDto);
    }
}
