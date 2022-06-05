using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Services.Account.Profile.DTO.Request
{
    public class ChangePasswordRequestDto
    {
        public string currentPassword { get; set; }
        public string newPassword { get; set; }
    }
}
