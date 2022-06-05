using EP.Infrastructure.Entities;
using EP.Infrastructure.IConfiguration;
using Microsoft.EntityFrameworkCore;

namespace EP.Infrastructure.Data.DB_Seed
{
    public class CinemaHallSeed : ISeeder
    {
        private readonly IUnitOfWork _unitOfWork;

        public CinemaHallSeed(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public int Index { get; set; } = 3;

        public async Task Seed()
        {
            if (await _unitOfWork.CinemaHall.Query().CountAsync() == 0)
            {
                var cinemaHall = new CinemaHall
                {
                    CinemaId = 1,
                    TotalSeats = 30,
                    Name = "Hall 1",
                    Columns = 10,
                    Rows = 3
                };

                await _unitOfWork.CinemaHall.AddAsync(cinemaHall);
                await _unitOfWork.SaveAsync();
            }
        }
    }
}
