using AutoMapper;
using EP.Application.Services.Currency.DTO;
using EP.Application.Services.DiscountCoupon;
using EP.Application.Services.Payment;
using EP.Application.Services.Payment.DTO.Request;
using EP.Application.Services.QRCode;
using EP.Application.Services.UserContext;
using EP.Infrastructure.Entities;
using EP.Infrastructure.Enums;
using EP.Infrastructure.IConfiguration;
using EP.Infrastructure.Repository;
using EP.Infrastructure.Services.Currency;
using EP.Infrastructure.Services.Stripe;
using EP.Infrastructure.Services.Stripe.DTO;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace EP.XUnitTests
{
    public class PaymentServiceTests
    {
        private readonly PaymentService _service;
        private readonly Mock<ILogger<PaymentService>> _loggerMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<IEmailSender> _emailSenderMock = new();
        private readonly Mock<IDiscountCouponService> _discountCouponServiceMock = new();
        private readonly Mock<INbgCurrencyService> _nbgCurrencyServiceMock = new();
        private readonly Mock<IUserContext> _userContextMock = new();
        private readonly Mock<IQrCodeService> _qrCodeServiceMock = new();
        private readonly Mock<IStripeService> _stripeServiceMock = new();
        private readonly Mock<IMapper> _mapperMock = new();

        public PaymentServiceTests()
        {
            _service = new PaymentService(
                _unitOfWorkMock.Object,
                _loggerMock.Object,
                _emailSenderMock.Object,
                _discountCouponServiceMock.Object,
                _nbgCurrencyServiceMock.Object,
                _userContextMock.Object,
                _qrCodeServiceMock.Object,
                _stripeServiceMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public void CreatePayment_Success()
        {
            var orderDetailsMock = new Mock<IGenericRepository<OrderDetails, int>>();
            var nbgCurrencyMock = new Mock<IGenericRepository<NbgCurrency, int>>();
            var showSeatMock = new Mock<IGenericRepository<ShowSeat, int>>();
            var cinemaSeatMock = new Mock<IGenericRepository<CinemaSeat, int>>();
            var showMock = new Mock<IGenericRepository<Show, int>>();
            var movieMock = new Mock<IGenericRepository<Movie, int>>();
            var paymentMock = new Mock<IGenericRepository<Payments, int>>();

            var requestDto = new CreatePaymentRequestDto
            {
                CurrencyCode = PaymentCurrency.EUR,
                DiscountCoupon = null,
                OrderIdList = new List<int> { 1 },
                PaymentMethod = PaymentMethod.Mastercard
            };

            var userId = Guid.NewGuid();

            _userContextMock.SetupGet(x => x.UserId).Returns(userId);

            nbgCurrencyMock.Setup(x =>
                    x.FindByCondition(It.IsAny<Expression<Func<NbgCurrency, bool>>>()))
                .Returns(new List<NbgCurrency>
                {
                    new NbgCurrency {
                        Code = PaymentCurrency.EUR.ToString(),
                        Rate = 3
                        }
                }.AsQueryable());

            _unitOfWorkMock.Setup(x => x.SaveAsync()).Returns(Task.CompletedTask);
            _unitOfWorkMock.SetupGet(x => x.OrderDetail).Returns(orderDetailsMock.Object);
            _unitOfWorkMock.SetupGet(x => x.ShowSeat).Returns(showSeatMock.Object);
            _unitOfWorkMock.SetupGet(x => x.CinemaSeat).Returns(cinemaSeatMock.Object);
            _unitOfWorkMock.SetupGet(x => x.Show).Returns(showMock.Object);
            _unitOfWorkMock.SetupGet(x => x.Movie).Returns(movieMock.Object);
            _unitOfWorkMock.SetupGet(x => x.Payment).Returns(paymentMock.Object);

            orderDetailsMock.Setup(x =>
                x.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<OrderDetails, bool>>>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new OrderDetails
                {
                    Status = OrderStatus.Processing.ToString(),
                    Id = 1,
                    showId = 1,
                    CreatedAt = DateTime.Now,
                    CreatedOnUtc = DateTime.Now,
                    IsDeleted = false,
                    ShowSeatId = 1,
                    TotalPrice = 15,
                    UserId = userId.ToString()
                }));

            showSeatMock.Setup(x =>
                x.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<ShowSeat, bool>>>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new ShowSeat
                {
                    ShowId = 1,
                    Id = 1,
                }));

            cinemaSeatMock.Setup(x =>
                x.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<CinemaSeat, bool>>>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new CinemaSeat
                {
                    Id = 1,
                })); ;


            showMock.Setup(x =>
                x.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Show, bool>>>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new Show
                {
                    Id = 1,
                    MovieId = 1,
                    CinemaHallId = 1,
                    CinemaHall = new CinemaHall
                    {
                        Id = 1,
                        Name = "Test Hall"
                    }
                }));


            movieMock.Setup(x =>
                x.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Movie, bool>>>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new Movie
                {
                    Id = 1,
                }));

            paymentMock.Setup(x => x.AddRangeAsync(It.IsAny<IEnumerable<Payments>>()))
                .Returns(Task.FromResult(true));


            _nbgCurrencyServiceMock.Setup(x =>
                x.Convert(It.IsAny<ConvertCurrencyDto>())).ReturnsAsync(15);

            _qrCodeServiceMock.Setup(x => x.GenerateQrCode(It.IsAny<string>()))
                .Returns("af8bfa88-f661-4a84-8067-a0e73f214fc3.JPEG");

            _stripeServiceMock.Setup(x =>
                x.Pay(It.IsAny<StripePaymentRequestDto>()))
                .ReturnsAsync(new StripePaymentDto
                {
                    PaymentUrl = "",
                    SessionId = ""
                });
            _emailSenderMock.Setup(x =>
                    x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);



            var result = _service.CreatePayment(requestDto).GetAwaiter().GetResult();


            Assert.NotNull(result);
            Assert.Null(result.Error);
        }

        [Fact]
        public void CreatePayment_UserLoggedIn()
        {
            //Arrange
            var requestDto = new CreatePaymentRequestDto
            {
                CurrencyCode = PaymentCurrency.EUR,
                DiscountCoupon = null,
                OrderIdList = new List<int> { 1 },
                PaymentMethod = PaymentMethod.Mastercard
            };

            _userContextMock.SetupGet(x => x.UserId)
                           .Returns((Guid?)null);

            //Act
            var result = _service.CreatePayment(requestDto).GetAwaiter().GetResult();

            //Assert
            Assert.NotNull(result.Error);
            Assert.Null(result.Content);
        }

        [Fact]
        public void CreatePayment_OrderDetail_DoesNotExist()
        {
            //Arrange   
            var orderDetailsMock = new Mock<IGenericRepository<OrderDetails, int>>();
            var requestDto = new CreatePaymentRequestDto
            {
                CurrencyCode = PaymentCurrency.EUR,
                DiscountCoupon = null,
                OrderIdList = new List<int> { 1 },
                PaymentMethod = PaymentMethod.Mastercard
            };

            //Act
            var orderDetail = orderDetailsMock.Setup(x =>
                    x.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<OrderDetails, bool>>>(), It.IsAny<string>()))
                .ReturnsAsync(null as OrderDetails);

            var result = _service.CreatePayment(requestDto).GetAwaiter().GetResult();

            //Assert
            Assert.NotNull(result.Error);

        }


    }
}
