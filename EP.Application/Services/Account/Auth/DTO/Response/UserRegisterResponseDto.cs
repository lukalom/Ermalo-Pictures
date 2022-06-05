using System.Security.AccessControl;

namespace EP.Application.Services.Account.Auth.DTO.Response
{
    public class UserRegisterResponseDto : AuthResult
    {
        public string ConfirmEmail { get; set; }
    }
}
