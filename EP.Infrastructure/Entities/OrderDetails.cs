using System.ComponentModel.DataAnnotations.Schema;
using EP.Infrastructure.Enums;
using EP.Infrastructure.IConfiguration;

namespace EP.Infrastructure.Entities
{
    public class OrderDetails : BaseEntity<int>
    {
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalPrice { get; set; }

        public string Status { get; set; } = OrderStatus.Pending.ToString(); // ექთენშენად შეიძლება გატანა კონსტანტად

        public int showId { get; set; }
        [ForeignKey(nameof(showId))]
        public Show Show { get; set; }

        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }

        public int ShowSeatId { get; set; }
        [ForeignKey(nameof(ShowSeatId))]
        public ShowSeat ShowSeat { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
