using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Services.Account.DTO
{
    public class TokenData
    {
        public string JwtToken { get; set; }

        public string RefreshToken { get; set; }
    }
}
