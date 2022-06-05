using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Services.Account.Role.DTO.Response
{
    public class RoleResult
    {
        public string Role { get; set; }
        public bool Success { get; set; }
        public List<string> Errors { get; set; }
    }
}
