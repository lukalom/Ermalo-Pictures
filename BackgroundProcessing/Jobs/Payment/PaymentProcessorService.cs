using EP.Infrastructure.Enums;
using EP.Infrastructure.IConfiguration;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;

namespace BackgroundProcessing.Jobs.Payment
{
    public class PaymentProcessorService : IPaymentProcessorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PaymentProcessorService> _logger;

        public PaymentProcessorService(IUnitOfWork unitOfWork,
            ILogger<PaymentProcessorService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task CancelInvalidPayment()
        {
            var ordersList = await _unitOfWork.OrderDetail.FindByCondition(x =>
                    x.CreatedAt.AddMinutes(5) < DateTime.Now
                    && x.IsDeleted == false
                    && (x.Status == OrderStatus.Cancelled.ToString()
                        || x.Status == OrderStatus.Approved.ToString()))
                .OrderByDescending(x => x.CreatedOnUtc)
                .Take(10)
                .ToListAsync();


            if (ordersList.Any())
            {
                var service = new SessionService();
                foreach (var order in ordersList)
                {

                    var payment = await _unitOfWork.Payment.FindByCondition(x =>
                        x.OrderDetailId == order.Id
                        && x.IsDeleted == false
                        && x.TransactionDate.AddMinutes(5) < DateTime.Now
                        && (x.Status == PaymentStatus.Approved.ToString()
                            || x.Status == PaymentStatus.Processing.ToString()))
                        .FirstOrDefaultAsync();

                    if (payment == null) _logger.LogInformation($"Payment is not created yet order id = {order.Id}");
                    if (payment != null)
                    {
                        if (payment.Status == PaymentStatus.Approved.ToString()) continue;

                        if (payment.Status == PaymentStatus.Processing.ToString())
                        {
                            payment.Status = PaymentStatus.Cancelled.ToString();
                            if (payment.DiscountCouponId != null)
                            {
                                var coupon = await _unitOfWork.DiscountCoupon.GetSingleOrDefaultAsync(payment.DiscountCouponId.Value);
                                coupon.Uses += 1;
                            }
                            payment.IsDeleted = true;
                            order.Status = OrderStatus.Cancelled.ToString();
                            var showSeat =
                                await _unitOfWork.ShowSeat.GetFirstOrDefaultAsync(x => x.Id == order.ShowSeatId);
                            showSeat.Status = ShowSeatStatus.Available;

                            var session = await service.ExpireAsync(payment.SessionId, cancellationToken: CancellationToken.None);
                            if (session.Status.ToLower() == "expired")
                            {
                                _logger.LogInformation("Session has been Expired");
                            }
                        }

                    }
                }
                await _unitOfWork.SaveAsync();
            }



            await Task.CompletedTask;
        }
    }
}
