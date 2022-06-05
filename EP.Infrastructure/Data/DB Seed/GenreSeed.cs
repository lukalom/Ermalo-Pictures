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
    public class GenreSeed : ISeeder
    {
        private readonly IUnitOfWork _unitOfWork;

        public GenreSeed(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public int Index { get; set; } = 5;

        public async Task Seed()
        {
            if (await _unitOfWork.Genre.Query().CountAsync() == 0)
            {
                var genres = new List<Genre>()
                {
                    new (){Name = "Action"},
                    new (){Name = "Drama"},
                    new (){Name = "Comedy"},
                    new (){Name = "Fantasy"},
                    new (){Name = "Western"},
                    new (){Name = "Georgian"},
                    new (){Name = "Sci-Fi"},
                    new (){Name = "Romantic"},

                };
                await _unitOfWork.Genre.AddRangeAsync(genres);
                await _unitOfWork.SaveAsync();
            }
        }
    }
}
