using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EP.Infrastructure.Entities;
using EP.Infrastructure.IConfiguration;
using EP.Shared.Configuration;
using EP.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EP.Application.Token
{
    public class TokenManager : ITokenManager

    {
        private readonly JwtConfig _jwtOptions;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TokenManager> _logger;

        public TokenManager(
            IOptionsMonitor<JwtConfig> optionsMonitor,
            IUnitOfWork unitOfWork,
            ILogger<TokenManager> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _jwtOptions = optionsMonitor.CurrentValue;
        }


        public Task<ClaimsPrincipal> GetPrincipalFromExpiredToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtOptions.Secret)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters,
                out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return Task.FromResult(principal);
        }

        public string RandomString(int length)
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(x => x[random.Next(x.Length)]).ToArray());
        }

        public Task<DateTime> UnixTimeStampToDateTime(long unixDate)
        {
            // Sets the time to 1, Jan, 1970
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            // Add the number of seconds from 1 Jan
            dateTime = dateTime.AddSeconds(unixDate).ToUniversalTime();
            return Task.FromResult(dateTime);
        }

        public async Task<RefreshToken?> GetByRefreshToken(string refreshToken)
        {
            try
            {
                return await _unitOfWork.RefreshTokens.FindByCondition(x =>
                        x.Token.ToLower() == refreshToken.ToLower() && x.IsDeleted == false)
                    .OrderByDescending(x => x.CreatedOnUtc)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<bool> RevokeAll(Guid userId)
        {
            try
            {
                var tokens = await _unitOfWork.RefreshTokens.FindByCondition(x
                    => x.UserId == userId.ToString() && x.IsDeleted == false).ToListAsync();
                //_unitOfWork.RefreshTokens.RemoveRange(tokens); //force delete
                foreach (var refreshToken in tokens)
                {
                    refreshToken.IsDeleted = true;
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }

        }
    }
}
