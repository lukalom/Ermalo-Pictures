using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EP.Application.DTO_General.Extension;
using EP.Application.DTO_General.Generic;
using EP.Application.Services.Account.Auth.DTO.Request;
using EP.Application.Services.Account.Auth.DTO.Response;
using EP.Application.Services.Account.DTO.Response;
using EP.Application.Token;
using EP.Infrastructure.Entities;
using EP.Infrastructure.Enums;
using EP.Infrastructure.IConfiguration;
using EP.Shared.Configuration;
using EP.Shared.Exceptions.Messages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace EP.Application.Services.Account.Auth
{
    public class AuthService : IAuthService
    {
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JwtConfig _jwtConfig;
        private readonly ITokenManager _tokenManager;
        private readonly SendGridConfig _sendGridConfig;
        private readonly IEmailSender _emailSender;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly FacebookConfig _facebookConfig;

        public AuthService(
            TokenValidationParameters tokenValidationParameters,
            IOptionsMonitor<JwtConfig> jwtMonitor,
            IOptionsMonitor<SendGridConfig> sendGridMonitor,
            IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ITokenManager tokenManager,
            IEmailSender emailSender,
            IHttpClientFactory httpClientFactory,
            IOptionsMonitor<FacebookConfig> fbMonitor)
        {
            _tokenValidationParameters = tokenValidationParameters;
            _sendGridConfig = sendGridMonitor.CurrentValue;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenManager = tokenManager;
            _emailSender = emailSender;
            _httpClientFactory = httpClientFactory;
            _facebookConfig = fbMonitor.CurrentValue;
            _jwtConfig = jwtMonitor.CurrentValue;
        }

        public async Task<Result<bool>> RevokeAll(Guid userId, string token)
        {
            var result = new Result<bool>();
            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);

            if (validatedToken is JwtSecurityToken jwtSecurityToken)
            {
                // check token algorithm
                var resultToken = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase);

                if (resultToken == false)
                {
                    result.Error = ErrorHandler.PopulateError(
                        (int)StatusCode.BadRequest,
                        "Invalid Tokens",
                        ErrorMessages.Generic.TypeBadRequest);

                    return result;
                }
            }

            var isRevoked = await _tokenManager.RevokeAll(userId);

            if (isRevoked)
            {
                await _unitOfWork.SaveAsync();
                result.Content = isRevoked;
                return result;
            }

            result.Error = ErrorHandler.PopulateError(
                (int)StatusCode.BadRequest,
                ErrorMessages.Generic.UnableToProcess,
                ErrorMessages.Generic.SomethingWentWrong);

            return result;
        }

        public async Task<Result<UserRegisterResponseDto>> Register(UserRegistrationRequestDto registrationDto)
        {
            var userExists = await _userManager.FindByEmailAsync(registrationDto.Email);
            var result = new Result<UserRegisterResponseDto>();
            if (userExists != null)
            {
                result.Error = ErrorHandler.PopulateError(
                    (int)StatusCode.NotFound,
                    ErrorMessages.Users.EmailAlreadyUsed,
                    ErrorMessages.Generic.TypeBadRequest);

                return result;
            }

            var newUser = new ApplicationUser()
            {
                Email = registrationDto.Email,
                UserName = registrationDto.UserName,
                PhoneNumber = registrationDto.PhoneNumber
            };

            var isCreated = await _userManager.CreateAsync(newUser, registrationDto.Password);
            if (!isCreated.Succeeded)
            {
                result.Error = ErrorHandler.PopulateError(
                    (int)StatusCode.BadRequest,
                    ErrorMessages.Generic.InvalidPayload,
                    ErrorMessages.Generic.TypeBadRequest);
                return result;
            }

            var emailConfirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);

            if (!string.IsNullOrEmpty(emailConfirmToken))
            {
                await SendEmailConfirmation(newUser, emailConfirmToken);
            }

            await _userManager.AddToRoleAsync(newUser, Infrastructure.Enums.Role.AppUser.ToString());

            var token = await GenerateJwtToken(newUser);

            var responseDto = new UserRegisterResponseDto
            {
                Token = token.Token,
                RefreshToken = token.RefreshToken,
                Errors = token.Errors,
                Success = token.Success,
                ConfirmEmail = "Confirm Email!"
            };

            result.Content = responseDto;
            return result;
        }

        public async Task SendEmailConfirmation(ApplicationUser user, string token)
        {
            var appUrl = _sendGridConfig.ApplicationUrl;

            var htmlMessage = $"<h1>Hello {user.UserName}</h1>" +
                              "<br><h3> Verify Email </h3>" +
                              "<p> Please click on the link below to verify your email address</p>" +
                              "<br/><hr/>" +
                              $"<a href =\"{appUrl}ConfirmEmail?userId={user.Id}&token={token}\">Confirm</a>";

            await _emailSender.SendEmailAsync(user.Email, "Confirm Email", htmlMessage);
        }

        public async Task<FacebookTokenValidationResult> ValidateFbAccessTokenAsync(string accessToken)
        {
            var formattedUrl = string.Format(_facebookConfig.TokenValidationUrl, accessToken, _facebookConfig.AppId,
                _facebookConfig.AppSecret);
            var result = await _httpClientFactory.CreateClient().GetAsync(formattedUrl);
            result.EnsureSuccessStatusCode();
            var responseAsString = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<FacebookTokenValidationResult>(responseAsString);
        }

        public async Task<FacebookUserInfoResult> GetFbUserInfoAsync(string accessToken)
        {
            var formattedUrl = string.Format(_facebookConfig.UserInfoUrl, accessToken);
            var result = await _httpClientFactory.CreateClient().GetAsync(formattedUrl);
            result.EnsureSuccessStatusCode();
            var responseAsString = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<FacebookUserInfoResult>(responseAsString);
        }

        public async Task<IdentityResult> ConfirmEmailAsync(ConfirmEmailRequestDto confirmDto)
        {
            var user = await _userManager.FindByIdAsync(confirmDto.userId);
            if (user.EmailConfirmed)
            {
                return IdentityResult.Failed(new IdentityError() { Description = "Email already confirmed", Code = 404.ToString() });
            }

            return await _userManager.ConfirmEmailAsync(user, confirmDto.token);
        }

        public async Task<Result<UserLoginResponseDto>> Login(UserLoginRequestDto loginDto)
        {
            var existingUser = await _userManager.FindByEmailAsync(loginDto.Email);
            var result = new Result<UserLoginResponseDto>();

            if (existingUser == null)
            {
                result.Error = ErrorHandler.PopulateError((int)StatusCode.NotFound,
                    ErrorMessages.Generic.InvalidPayload, ErrorMessages.Generic.TypeBadRequest);
                return result;
            }

            var isCorrect = await _userManager.CheckPasswordAsync(existingUser, loginDto.Password);
            if (!isCorrect)
            {
                result.Error = ErrorHandler.PopulateError((int)StatusCode.NotFound,
                    ErrorMessages.Generic.InvalidPayload, ErrorMessages.Generic.TypeBadRequest);
                return result;
            }

            var jwtToken = await GenerateJwtToken(existingUser);

            var responseDto = new UserLoginResponseDto
            {
                Token = jwtToken.Token,
                RefreshToken = jwtToken.RefreshToken,
                Errors = jwtToken.Errors,
                Success = jwtToken.Success
            };

            result.Content = responseDto;

            return result;
        }

        public async Task<Result<AuthResult>> VerifyTokenAndUpdate(TokenRequestDto tokenRequestDto)
        {
            var responseResult = new Result<AuthResult>();

            var tokenInVerification = await _tokenManager.GetPrincipalFromExpiredToken(tokenRequestDto.Token);

            //check expiry Date
            var utcExpiryDate =
                long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

            var expDate = await _tokenManager.UnixTimeStampToDateTime(utcExpiryDate);

            if (expDate > DateTime.UtcNow)
            {
                responseResult.Error = ErrorHandler.PopulateError(
                    (int)StatusCode.BadRequest,
                    ErrorMessages.Jwt.JwtNotExpired,
                    ErrorMessages.Generic.TypeBadRequest);

                return responseResult;
            }

            // check if refresh token exists
            var refreshToken = await _tokenManager.GetByRefreshToken(tokenRequestDto.RefreshToken);

            if (refreshToken == null)
            {
                responseResult.Error = ErrorHandler.PopulateError(
                    (int)StatusCode.NotFound,
                    ErrorMessages.Jwt.InvalidRefreshToken,
                    ErrorMessages.Generic.TypeBadRequest);

                return responseResult;
            }

            var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            if (refreshToken.JwtId != jti || refreshToken.IsDeleted)
            {
                responseResult.Error = ErrorHandler.PopulateError(
                    (int)StatusCode.BadRequest,
                    ErrorMessages.Jwt.InvalidTokenId,
                    ErrorMessages.Generic.TypeBadRequest);

                return responseResult;
            }

            if (refreshToken.ExpiryDate.ToUniversalTime() < DateTime.Now.ToUniversalTime())
            {
                responseResult.Error = ErrorHandler.PopulateError(
                    (int)StatusCode.BadRequest,
                    ErrorMessages.Jwt.RefreshTokenExpired,
                    ErrorMessages.Generic.TypeBadRequest);

                if (await _tokenManager.RevokeAll(new Guid(refreshToken.UserId)))
                {
                    return responseResult;
                }

                responseResult.Error = ErrorHandler.PopulateError(
                    (int)StatusCode.BadRequest,
                    ErrorMessages.Generic.SomethingWentWrong,
                    ErrorMessages.Generic.UnableToProcess);

                return responseResult;
            }

            var dbUser = await _userManager.FindByIdAsync(refreshToken.UserId);

            if (dbUser == null)
            {
                responseResult.Error = ErrorHandler.PopulateError(
                    (int)StatusCode.BadRequest,
                    ErrorMessages.Generic.InvalidPayload,
                    ErrorMessages.Generic.TypeBadRequest);

                return responseResult;
            }

            var token = await GenerateJwtToken(dbUser);

            responseResult.Content = token;

            return responseResult;

        }

        public async Task<AuthResult> GenerateJwtToken(ApplicationUser user)
        {
            var jwtHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

            var claims = await GetAllValidClaims(user);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Audience = JwtConfig.Audience,
                Issuer = JwtConfig.Issuer,
                Expires = DateTime.Now.AddMinutes(10),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),

            };

            var token = jwtHandler.CreateToken(tokenDescriptor);

            var jwtToken = jwtHandler.WriteToken(token);

            var tokenFromDb = await _unitOfWork.RefreshTokens
                .FindByCondition(x =>
                    x.UserId == user.Id && x.IsDeleted == false)
                .OrderByDescending(x => x.CreatedOnUtc)
                .FirstOrDefaultAsync();

            var refreshToken = new RefreshToken();
            if (tokenFromDb == null)
            {
                refreshToken.Token = _tokenManager.RandomString(35) + Guid.NewGuid();
                refreshToken.ExpiryDate = DateTime.Now.AddMinutes(30); //Todo 1 day
                refreshToken.CreatedOnUtc = DateTime.Now;
            }
            else
            {
                refreshToken.Token = tokenFromDb.Token;
                refreshToken.ExpiryDate = tokenFromDb.ExpiryDate;
                refreshToken.CreatedOnUtc = tokenFromDb.CreatedOnUtc;
            }

            refreshToken.JwtId = token.Id;
            refreshToken.UserId = user.Id;

            await _unitOfWork.RefreshTokens.AddAsync(refreshToken);
            await _unitOfWork.SaveAsync();

            return new AuthResult()
            {
                Token = jwtToken,
                Success = true,
                RefreshToken = refreshToken.Token
            };
        }

        private async Task<List<Claim>> GetAllValidClaims(ApplicationUser user)
        {
            var claims = new List<Claim>()
            {
                new("Id", user.Id),
                new ("isBlocked", user.IsBlocked.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(JwtRegisteredClaimNames.Sub, user.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                var role = await _roleManager.FindByNameAsync(userRole);

                if (role != null)
                {
                    claims.Add(new Claim(ClaimTypes.Role, userRole));
                    var roleClaims = await _roleManager.GetClaimsAsync(role);
                    claims.AddRange(roleClaims);
                }
            }

            return claims;
        }
    }
}
