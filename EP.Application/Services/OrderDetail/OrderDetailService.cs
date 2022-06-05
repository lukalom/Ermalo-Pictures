using AutoMapper;
using EP.Application.DTO_General.Extension;
using EP.Application.DTO_General.Generic;
using EP.Application.Extensions;
using EP.Application.Services.OrderDetail.DTO;
using EP.Infrastructure.Entities;
using EP.Infrastructure.Enums;
using EP.Infrastructure.IConfiguration;
using EP.Shared.Exceptions;
using EP.Shared.Exceptions.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace EP.Application.Services.OrderDetail
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<OrderDetailService> _logger;

        public OrderDetailService(IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<OrderDetailService> logger)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<Result<CreateOrderResponseDto>> CreateOrder(CreateOrderRequestDto createOrderDto)
        {
            var result = new Result<CreateOrderResponseDto>();

            if (createOrderDto.CinemaSeatIdList.Any(seatId =>
                    createOrderDto.CinemaSeatIdList.Select(x => x == seatId).Count() > 1))
            {
                throw new AppException("You Cannot buy same seat more than 1");
            }

            var show = await _unitOfWork.Show.GetFirstOrDefaultAsync(x =>
                x.Id == createOrderDto.ShowId && x.IsDeleted == false, "Movie");
            if (show == null)
            {
                result.Error = ErrorHandler.PopulateError((int)StatusCode.BadRequest,
                    ErrorMessages.Generic.ObjectNotFound,
                    ErrorMessages.Generic.TypeBadRequest);

                return result;
            }

            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                result.Error = ErrorHandler.PopulateError((int)StatusCode.BadRequest,
                    ErrorMessages.Users.UserNotFound,
                    ErrorMessages.Generic.TypeBadRequest);

                return result;
            }

            var showSeatList = new List<Infrastructure.Entities.ShowSeat>();
            var totalPrice = decimal.Zero;
            var orderDetails = new List<OrderDetails>();

            foreach (var seatId in createOrderDto.CinemaSeatIdList)
            {
                var showSeat = await _unitOfWork.ShowSeat.GetFirstOrDefaultAsync(x =>
                    x.CinemaSeatId == seatId && x.Status == ShowSeatStatus.Available);

                if (showSeat == null)
                {
                    result.Error = ErrorHandler.PopulateError((int)StatusCode.BadRequest,
                        $"Seat is not Available right now",
                        ErrorMessages.Generic.TypeBadRequest);

                    return result;
                }

                totalPrice += showSeat.Price;
                showSeat.Status = ShowSeatStatus.Booked;

                showSeatList.Add(showSeat);
                orderDetails.Add(new OrderDetails()
                {
                    UserId = user.Id,
                    showId = createOrderDto.ShowId,
                    Status = OrderStatus.Processing.ToString(),
                    ShowSeatId = showSeat.Id,
                    CreatedAt = DateTime.Now,
                    TotalPrice = showSeat.Price
                });
            }

            //orderDetails.ForEach(x => x.TotalPrice = totalPrice);

            if (await _unitOfWork.OrderDetail.AddRangeAsync(orderDetails))
            {
                await _unitOfWork.SaveAsync();
                result.Content = new CreateOrderResponseDto()
                {
                    TotalPrice = totalPrice,
                    Email = user.Email,
                    Status = orderDetails[0].Status,
                    Movie = show.Movie.Title,
                    SeatIdList = createOrderDto.CinemaSeatIdList
                };

                return result;
            }

            result.Error = ErrorHandler.PopulateError((int)StatusCode.BadRequest,
                ErrorMessages.Generic.SomethingWentWrong,
                ErrorMessages.Generic.TypeBadRequest);

            return result;
        }

        public async Task<Result<List<GetUserApprovedOrderResponseDto>>> GetUserApprovedOrders()
        {
            var result = new Result<List<GetUserApprovedOrderResponseDto>>();

            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue("Id");
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                result.Error = ErrorHandler.PopulateError((int)StatusCode.BadRequest,
                    ErrorMessages.Users.UserNotFound,
                    ErrorMessages.Generic.TypeBadRequest);

                return result;
            }

            var approvedOrders = await _unitOfWork.OrderDetail.GetAllFilterAsync(x =>
                x.UserId == user.Id && x.Status == OrderStatus.Approved.ToString(), "ShowSeat");

            if (!approvedOrders.Any())
            {
                result.Error = ErrorHandler.PopulateError((int)StatusCode.BadRequest,
                    "You Don't Have any Orders yet",
                    ErrorMessages.Generic.TypeBadRequest);

                return result;
            }

            foreach (var approvedOrder in approvedOrders)
            {
                await _unitOfWork.CinemaSeat.GetFirstOrDefaultAsync(x => x.Id == approvedOrder.ShowSeat.CinemaSeatId);
                await _unitOfWork.Show.GetFirstOrDefaultAsync(x => x.Id == approvedOrder.showId, "Movie");
            }

            var mappedOrders = _mapper.Map<List<GetUserApprovedOrderResponseDto>>(approvedOrders);

            result.Content = mappedOrders;
            return result;
        }
    }
}
