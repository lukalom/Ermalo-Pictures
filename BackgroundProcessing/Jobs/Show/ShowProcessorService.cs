using EP.Infrastructure.Enums;
using EP.Infrastructure.IConfiguration;
using Microsoft.EntityFrameworkCore;

namespace BackgroundProcessing.Jobs.Show
{
    public class ShowProcessorService : IShowProcessorService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShowProcessorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task DeleteEndTimeShows()
        {
            var shows = await _unitOfWork.Show.FindByCondition(x => x.EndTime < DateTime.Now && x.IsDeleted == false)
                .OrderByDescending(x => x.CreatedOnUtc)
                .Take(10).ToListAsync();

            if (shows.Any())
            {
                foreach (var show in shows)
                {
                    var showSeats = await _unitOfWork.ShowSeat.FindByCondition(x =>
                            x.ShowId == show.Id && x.IsDeleted == false)
                        .ToListAsync();
                    showSeats.ForEach(x =>
                    {
                        x.Status = ShowSeatStatus.Disabled;
                        x.IsDeleted = true;
                    });
                    show.IsDeleted = true;
                }

                await _unitOfWork.SaveAsync();
            }

            await Task.CompletedTask;
        }
    }
}
