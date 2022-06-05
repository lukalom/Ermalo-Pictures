using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EP.Infrastructure.Entities;
using EP.Infrastructure.IConfiguration;
using Microsoft.EntityFrameworkCore;

namespace EP.Infrastructure.Data.DB_Seed
{
    public class ShowSeed : ISeeder
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShowSeed(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public int Index { get; set; } = 7;

        public async Task Seed()
        {
            if (await _unitOfWork.Show.Query().CountAsync() == 0)
            {
                var show = new Show
                {
                    Date = DateTime.Now,
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(2),
                    MovieId = 1,
                    CinemaHallId = 1
                };
                await _unitOfWork.Show.AddAsync(show);
                await _unitOfWork.SaveAsync();
            }
        }
    }
}
