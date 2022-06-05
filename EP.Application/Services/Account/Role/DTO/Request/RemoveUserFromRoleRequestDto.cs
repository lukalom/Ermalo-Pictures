using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Services.Account.Role.DTO.Request
{
    public class RemoveUserFromRoleRequestDto
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string RoleName { get; set; }
    }
}
