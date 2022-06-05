using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EP.Infrastructure.Enums;
using EP.Infrastructure.IConfiguration;

namespace EP.Infrastructure.Entities
{
    public class Payments : BaseEntity<int>
    {
        [Column(TypeName = "decimal(18, 2)")]
        [Required]
        public decimal Amount { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        public int? DiscountCouponId { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; }

        [Required]
        public int OrderDetailId { get; set; }
        [ForeignKey(nameof(OrderDetailId))]
        public OrderDetails OrderDetails { get; set; }

        [Required]
        public string QrCodeImgUrl { get; set; }

        [Required]
        public string SessionId { get; set; }

        public Guid TransactionUniqueCode { get; set; } = Guid.NewGuid();

        public string Status { get; set; } = PaymentStatus.Pending.ToString();
    }
}
