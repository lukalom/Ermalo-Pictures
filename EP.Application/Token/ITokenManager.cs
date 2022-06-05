using System.Security.Claims;
using EP.Infrastructure.Entities;

namespace EP.Application.Token
{
    public interface ITokenManager
    {
        Task<RefreshToken?> GetByRefreshToken(string refreshToken);
        Task<bool> RevokeAll(Guid userId);
        Task<ClaimsPrincipal> GetPrincipalFromExpiredToken(string? token);
        public Task<DateTime> UnixTimeStampToDateTime(long unixDate);
        public string RandomString(int length);
    }
}
