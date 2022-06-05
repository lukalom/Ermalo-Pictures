using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Services.Account.Auth.DTO.Request
{
    public class ConfirmEmailRequestDto
    {
        public string userId { get; set; }

        public string token { get; set; }
    }
}
