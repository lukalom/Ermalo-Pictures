using EP.Infrastructure.Entities;
using EP.Infrastructure.Enums;
using EP.Infrastructure.IConfiguration;
using Microsoft.EntityFrameworkCore;

namespace EP.Infrastructure.Data.DB_Seed
{
    public class CinemaSeatSeed : ISeeder
    {
        private readonly IUnitOfWork _unitOfWork;

        public CinemaSeatSeed(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public int Index { get; set; } = 4;

        public async Task Seed()
        {
            if (await _unitOfWork.CinemaSeat.Query().CountAsync() == 0)
            {
                var cinemaSeats = new List<CinemaSeat>();
                for (int i = 1; i < 4; i++)
                {
                    for (int j = 1; j < 11; j++)
                    {
                        cinemaSeats.Add(new CinemaSeat()
                        {
                            CinemaHallId = 1,
                            ColumnNumber = j,
                            RowNumber = i,
                            SeatType = j % 2 == 0 ? SeatType.Medium : SeatType.Vip
                        });
                    }
                }

                await _unitOfWork.CinemaSeat.AddRangeAsync(cinemaSeats);
                await _unitOfWork.SaveAsync();
            }
        }
    }
}
