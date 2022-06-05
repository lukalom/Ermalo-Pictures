using EP.Infrastructure.Enums;
using EP.Infrastructure.IConfiguration;
using Microsoft.EntityFrameworkCore;

namespace BackgroundProcessing.Jobs.OrderDetails
{
    public class OrderDetailsProcessorService : IOrderDetailsProcessorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrderDetailsProcessorService> _logger;

        public OrderDetailsProcessorService(IUnitOfWork unitOfWork,
            ILogger<OrderDetailsProcessorService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task CancelInvalidOrders()
        {
            var orders = await _unitOfWork.OrderDetail.FindByCondition(x =>
                    x.CreatedAt.AddMinutes(5) < DateTime.Now
                    && x.IsDeleted == false
                    && x.Status == OrderStatus.Processing.ToString()
                    || x.Status == OrderStatus.Cancelled.ToString())
                .OrderByDescending(x => x.CreatedOnUtc)
                .Take(10)
                .ToListAsync();

            if (orders.Any())
            {
                foreach (var order in orders)
                {
                    order.Status = OrderStatus.Cancelled.ToString();
                    _logger.LogInformation("Order Status Cancelled");
                    var showSeat =
                        await _unitOfWork.ShowSeat.GetFirstOrDefaultAsync(x => x.Id == order.ShowSeatId);
                    showSeat.Status = ShowSeatStatus.Available;
                }

                await _unitOfWork.SaveAsync();
            }

            await Task.CompletedTask;
        }
    }
}
