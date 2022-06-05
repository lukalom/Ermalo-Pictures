using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EP.Infrastructure.IConfiguration;
using Microsoft.AspNetCore.Identity;

namespace EP.Infrastructure.Entities
{
    public class RefreshToken : BaseEntity<int>
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public string JwtId { get; set; }
        public DateTime AddedDate { get; set; } = DateTime.Now; //IssuedAt
        public DateTime ExpiryDate { get; set; } // ExpiresAt 

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }
    }
}
