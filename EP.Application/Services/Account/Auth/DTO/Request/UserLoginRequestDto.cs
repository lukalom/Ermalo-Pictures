using System.ComponentModel.DataAnnotations;

namespace EP.Application.Services.Account.Auth.DTO.Request
{
    public class UserLoginRequestDto
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
