using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Services.Account.Claims.DTO.Request
{
    public class AddClaimsToUserRequestDto
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string ClaimName { get; set; }

        [Required]
        public string ClaimValue { get; set; }
    }
}
