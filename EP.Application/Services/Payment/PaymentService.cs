using AutoMapper;
using EP.Application.DTO_General.Extension;
using EP.Application.DTO_General.Generic;
using EP.Application.Services.Currency.DTO;
using EP.Application.Services.DiscountCoupon;
using EP.Application.Services.DiscountCoupon.DTO;
using EP.Application.Services.Payment.DTO.Request;
using EP.Application.Services.Payment.DTO.Response;
using EP.Application.Services.QRCode;
using EP.Application.Services.UserContext;
using EP.Infrastructure.Entities;
using EP.Infrastructure.Enums;
using EP.Infrastructure.IConfiguration;
using EP.Infrastructure.Services.Currency;
using EP.Infrastructure.Services.Stripe;
using EP.Infrastructure.Services.Stripe.DTO;
using EP.Shared.Exceptions;
using EP.Shared.Exceptions.Messages;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stripe.Checkout;

namespace EP.Application.Services.Payment
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PaymentService> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IDiscountCouponService _discountCoupon;
        private readonly IStripeService _stripeService;
        private readonly INbgCurrencyService _nbgCurrencyService;
        private readonly IUserContext _userContext;
        private readonly IQrCodeService _qrCodeService;
        private readonly IMapper _mapper;

        public PaymentService(
            IUnitOfWork unitOfWork,
            ILogger<PaymentService> logger,
            IEmailSender emailSender,
            IDiscountCouponService discountCoupon,
            INbgCurrencyService nbgCurrencyService,
            IUserContext userContext,
            IQrCodeService qrCodeService,
            IStripeService stripeService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _emailSender = emailSender;
            _discountCoupon = discountCoupon;
            _nbgCurrencyService = nbgCurrencyService;
            _userContext = userContext;
            _qrCodeService = qrCodeService;
            _stripeService = stripeService;
            _mapper = mapper;
        }

        public async Task<Result<List<CreatePaymentResponseDto>>> CreatePayment(CreatePaymentRequestDto requestDto)
        {
            var result = new Result<List<CreatePaymentResponseDto>>();

            var loggedInUser = _userContext.UserId;
            if (loggedInUser == null)
            {
                result.Error = ErrorHandler.PopulateError(
                    (int)StatusCode.BadRequest,
                    "Please Logged in",
                    ErrorMessages.Generic.TypeBadRequest);

                _logger.LogInformation("Invalid Payment Method");

                return result;
            }

            DiscountCouponDTO? coupon = null;
            if (!string.IsNullOrEmpty(requestDto.DiscountCoupon))
            {
                var couponRes = await _discountCoupon.RemoveUse(requestDto.DiscountCoupon);
                if (!couponRes.IsSuccess)
                {
                    result.Error = couponRes.Error;
                    return result;
                }

                coupon = couponRes.Content;
            }

            var validPayments = new List<Payments>();
            var responseDto = new List<CreatePaymentResponseDto>();
            foreach (var orderId in requestDto.OrderIdList)
            {
                var paymentResDto = new CreatePaymentResponseDto();

                var orderDetail = await _unitOfWork.OrderDetail.GetFirstOrDefaultAsync(x =>
                x.Status == OrderStatus.Processing.ToString() && x.Id == orderId, "ShowSeat");

                if (orderDetail == null)
                {
                    result.Error = ErrorHandler.PopulateError(
                        (int)StatusCode.BadRequest,
                        "Order does not exists",
                        ErrorMessages.Generic.TypeBadRequest);

                    _logger.LogInformation($"Order does not exists");

                    return result;
                }

                if (orderDetail.UserId != loggedInUser.ToString())
                {
                    result.Error = ErrorHandler.PopulateError(
                        (int)StatusCode.BadRequest,
                        "Invalid User logged in",
                        ErrorMessages.Generic.TypeBadRequest);

                    _logger.LogInformation($"Order does not exists");

                    return result;
                }

                if (orderDetail.CreatedAt.AddMinutes(5) < DateTime.Now)
                {
                    result.Error = ErrorHandler.PopulateError(
                        (int)StatusCode.BadRequest,
                        "5 Minute Has Been Expired",
                        ErrorMessages.Generic.TypeBadRequest);

                    _logger.LogInformation($"5 Minute Has Been Expired");

                    return result;
                }

                var payment = new Payments()
                {
                    PaymentMethod = requestDto.PaymentMethod,
                    TransactionDate = DateTime.Now,
                    OrderDetailId = orderDetail.Id,
                    TransactionUniqueCode = Guid.NewGuid()
                };

                if (requestDto.CurrencyCode == PaymentCurrency.GEL)
                {
                    payment.Amount = orderDetail.TotalPrice;
                }
                else
                {
                    var convertedValue = await _nbgCurrencyService
                        .Convert(new ConvertCurrencyDto(requestDto.CurrencyCode.ToString(),
                            (double)orderDetail.TotalPrice));
                    payment.Amount = (decimal)convertedValue;
                }


                if (coupon != null)
                {
                    payment.Amount = payment.Amount * coupon.Discount / 100;
                    payment.DiscountCouponId = _unitOfWork.DiscountCoupon.GetFirstOrDefaultAsync(x =>
                        x.Code == coupon.Code && x.IsDeleted == false).GetAwaiter().GetResult().Id;
                }

                validPayments.Add(payment);

                var showSeat = await _unitOfWork.ShowSeat.GetFirstOrDefaultAsync(x =>
                    x.Id == orderDetail.ShowSeatId);
                showSeat.Status = ShowSeatStatus.Sold;

                var cinemaSeat =
                    await _unitOfWork.CinemaSeat.GetFirstOrDefaultAsync(x => x.Id == showSeat.CinemaSeatId);
                paymentResDto.RowNumber = cinemaSeat.RowNumber;
                paymentResDto.ColumnNumber = cinemaSeat.ColumnNumber;

                var show = await _unitOfWork.Show.GetFirstOrDefaultAsync(x =>
                    x.Id == showSeat.ShowId, "CinemaHall");
                paymentResDto.StartTime = show.StartTime.ToString("MM/dd/yyyy hh:mm tt");
                paymentResDto.Hall = show.CinemaHall.Name;
                paymentResDto.Movie = _unitOfWork.Movie.GetFirstOrDefaultAsync(x => x.Id == show.MovieId)
                    .GetAwaiter().GetResult().Title;

                paymentResDto.Price = payment.Amount;
                paymentResDto.TransactionUniqueCode = payment.QrCodeImgUrl;
                orderDetail.Status = OrderStatus.Approved.ToString();

                responseDto.Add(paymentResDto);
            }

            if (await _unitOfWork.Payment.AddRangeAsync(validPayments))
            {
                _logger.LogInformation("Payment Created Successfully");

                for (int i = 0; i < validPayments.Count; i++)
                {
                    var imgUrl = _qrCodeService.GenerateQrCode(validPayments[i].TransactionUniqueCode.ToString());
                    responseDto[i].TransactionUniqueCode = validPayments[i].TransactionUniqueCode.ToString();
                    validPayments[i].QrCodeImgUrl = imgUrl;
                    responseDto[i].QrCodeUrl = imgUrl;
                }

                var paymentData = new StripePaymentRequestDto()
                {
                    SessionData = _mapper.Map<List<StripeSessionDto>>(responseDto),
                    Currency = requestDto.CurrencyCode.ToString()
                };
                var stripePaymentDto = await _stripeService.Pay(paymentData);
                responseDto.First().PaymentUrl = stripePaymentDto.PaymentUrl;
                validPayments.ForEach(x =>
                {
                    x.SessionId = stripePaymentDto.SessionId;
                    x.Status = PaymentStatus.Processing.ToString();
                });

                var htmlMessage = "<p>Please click on the link below to pay on stripe</p>" +
                                          "<p>you have 5 minutes to pay</p>" +
                                          "<br/><hr/>" +
                                          $"<a href =\"{stripePaymentDto.PaymentUrl}\">Stripe</a>";

                await _emailSender.SendEmailAsync(_userContext.Email, "Payment Details", htmlMessage);

                await _unitOfWork.SaveAsync(); //Status Processing
                result.Content = responseDto;
                return result;
            }

            _logger.LogInformation("Something Went wrong payment failed");
            throw new AppException("Something Went wrong payment failed");
        }

        public async Task<Result<bool>> ConfirmPayment(string uniqueCode)
        {
            var result = new Result<bool>();

            var payment = await _unitOfWork.Payment.GetFirstOrDefaultAsync(x =>
                x.TransactionUniqueCode.ToString() == uniqueCode
                && x.IsDeleted == false);
            if (payment == null || payment.Status == PaymentStatus.Cancelled.ToString()) throw new AppException("Invalid Payment");
            if (payment.Status == PaymentStatus.Approved.ToString())
            {
                result.Content = true;
                return result;
            }

            var service = new SessionService();
            var session = await service.GetAsync(payment.SessionId);

            if (session.PaymentStatus.ToLower() == "paid")
            {
                var payments = await _unitOfWork.Payment.FindByCondition(x =>
                    x.SessionId == payment.SessionId
                    && x.IsDeleted == false
                    && x.Status == PaymentStatus.Processing.ToString()).ToListAsync();
                payments.ForEach(x => x.Status = PaymentStatus.Approved.ToString());
                await _unitOfWork.SaveAsync(); //status approved
                result.Content = true;

            }

            return result;
        }

    }
}
