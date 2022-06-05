using System.ComponentModel.DataAnnotations.Schema;
using EP.Infrastructure.Enums;
using EP.Infrastructure.IConfiguration;

namespace EP.Infrastructure.Entities
{
    public class ShowSeat : BaseEntity<int>
    {
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        public ShowSeatStatus Status { get; set; } = ShowSeatStatus.Available;

        public int CinemaSeatId { get; set; }
        [ForeignKey(nameof(CinemaSeatId))]
        public CinemaSeat CinemaSeat { get; set; }

        public int ShowId { get; set; }
        [ForeignKey(nameof(ShowId))]
        public Show Show { get; set; }
    }
}
