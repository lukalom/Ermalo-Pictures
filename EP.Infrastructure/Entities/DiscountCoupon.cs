using EP.Infrastructure.IConfiguration;
using System.ComponentModel.DataAnnotations;

namespace EP.Infrastructure.Entities
{
    public class DiscountCoupon : BaseEntity<int>
    {
        [Required]
        public string Code { get; set; }

        [Required]
        public int Uses { get; set; }
        [Required]
        public int Discount { get; set; }
    }
}
