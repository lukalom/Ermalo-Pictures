using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace EP.Application.Services.Account.Profile.DTO.Response
{
    public class GetProfileResponseDto
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

    }
}
