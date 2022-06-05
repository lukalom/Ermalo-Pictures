using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EP.Application.Services.DiscountCoupon.DTO
{
    public class DiscountCouponDTO
    {
        [Required]
        public string Code { get; set; }

        [Required]
        public int Uses { get; set; }

        [Required]
        public int Discount { get; set; }
    }
}
