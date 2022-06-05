using EP.Infrastructure.Entities;
using EP.Infrastructure.Enums;
using EP.Infrastructure.IConfiguration;
using Microsoft.EntityFrameworkCore;

namespace EP.Infrastructure.Data.DB_Seed
{
    public class ShowSeatSeed : ISeeder
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShowSeatSeed(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public int Index { get; set; } = 8;

        public async Task Seed()
        {
            if (await _unitOfWork.ShowSeat.Query().CountAsync() == 0)
            {
                var showSeatList = new List<ShowSeat>();
                for (int i = 1; i < 31; i++)
                {
                    showSeatList.Add(new ShowSeat()
                    {
                        Price = i % 2 == 0 ? 10 : 15,
                        Status = ShowSeatStatus.Available,
                        CinemaSeatId = i,
                        ShowId = 1
                    });
                }

                await _unitOfWork.ShowSeat.AddRangeAsync(showSeatList);
                await _unitOfWork.SaveAsync();
            }
        }
    }
}
