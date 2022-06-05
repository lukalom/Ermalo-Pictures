using System.ComponentModel.DataAnnotations;

namespace EP.Application.Services.Account.Auth.DTO.Request
{
    public class TokenRequestDto
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}
