using System.ComponentModel.DataAnnotations;

namespace EP.Application.Services.OrderDetail.DTO
{
    public class CreateOrderRequestDto
    {
        public CreateOrderRequestDto()
        {
            CinemaSeatIdList = new List<int>();
        }

        [Required]
        public int ShowId { get; set; }

        [Required]
        public List<int> CinemaSeatIdList { get; set; }
    }
}
