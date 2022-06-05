using EP.Infrastructure.Entities;
using EP.Infrastructure.IConfiguration;
using Microsoft.EntityFrameworkCore;

namespace EP.Infrastructure.Data.DB_Seed
{
    public class CinemaSeed : ISeeder
    {
        private readonly IUnitOfWork _unitOfWork;

        public CinemaSeed(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public int Index { get; set; } = 2;

        public async Task Seed()
        {
            if (await _unitOfWork.Cinema.Query().CountAsync() == 0)
            {
                var cinema = new Cinema() { Name = "Cavea" };
                await _unitOfWork.Cinema.AddAsync(cinema);
                await _unitOfWork.SaveAsync();
            }
        }
    }
}
