using AutoMapper;
using EP.Application.DTO_General.Errors;
using EP.Application.DTO_General.Extension;
using EP.Application.DTO_General.Generic;
using EP.Application.Services.ShowSeat.DTO;
using EP.Infrastructure.Enums;
using EP.Infrastructure.IConfiguration;
using EP.Shared.Exceptions.Messages;

namespace EP.Application.Services.ShowSeat
{
    public class ShowSeatService : IShowSeatService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ShowSeatService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<string>> ChangePriceToShowSeat(int showSeatId, decimal newPrice)
        {
            var result = new Result<string>();

            var showSeat = await _unitOfWork.ShowSeat.GetFirstOrDefaultAsync(x =>
                x.Id == showSeatId && x.Status == ShowSeatStatus.Available);
            if (showSeat == null)
            {
                result.Error = ErrorHandler.PopulateError(
                    (int)StatusCode.BadRequest,
                    "Object not found or its not available",
                    ErrorMessages.Generic.TypeBadRequest);

                return result;
            }

            if (showSeat.Price == newPrice)
            {
                result.Error = ErrorHandler.PopulateError(
                    (int)StatusCode.BadRequest,
                    "Price is same as it was",
                    ErrorMessages.Generic.TypeBadRequest);

                return result;
            }

            if (newPrice > 0)
            {
                showSeat.Price = newPrice;
                await _unitOfWork.SaveAsync();
                result.Content = $"Price Changed To {showSeat.Price}";
                return result;
            }


            result.Error = ErrorHandler.PopulateError(
                (int)StatusCode.BadRequest,
                "Price Should be more Than 0",
                ErrorMessages.Generic.TypeBadRequest);

            return result;

        }

        public async Task<Result<string>> CreateShowSeats(CreateShowSeatRequestDto requestDto)
        {
            var result = new Result<string>();
            if (requestDto.CinemaSeatIdList.Count <= 0)
            {
                result.Error = ErrorHandler.PopulateError(
                    (int)StatusCode.BadRequest,
                    "Show Seat should be more than zero",
                    ErrorMessages.Generic.TypeBadRequest);

                return result;
            }

            var showExist = await _unitOfWork.Show.GetFirstOrDefaultAsync(x => x.Id == requestDto.ShowId);
            if (showExist == null)
            {
                result.Error = ErrorHandler.PopulateError(
                    (int)StatusCode.BadRequest,
                    ErrorMessages.Generic.ObjectNotFound,
                    ErrorMessages.Generic.TypeBadRequest);

                return result;
            }

            var cinemaHall = await _unitOfWork.CinemaHall.GetFirstOrDefaultAsync(x => x.Id == showExist.CinemaHallId);
            if (cinemaHall == null)
            {
                result.Error = ErrorHandler.PopulateError(
                    (int)StatusCode.BadRequest,
                    ErrorMessages.Generic.ObjectNotFound,
                    ErrorMessages.Generic.TypeBadRequest);

                return result;
            }

            if (cinemaHall.TotalSeats < requestDto.CinemaSeatIdList.Count)
            {
                result.Error = ErrorHandler.PopulateError(
                    (int)StatusCode.BadRequest,
                    $"total seat {cinemaHall.TotalSeats} and request seat length {requestDto.CinemaSeatIdList.Count}",
                    ErrorMessages.Generic.TypeBadRequest);

                return result;
            }


            var validSeats = await _unitOfWork.ShowSeat.GetAllFilterAsync(x =>
                x.Status == ShowSeatStatus.Available
                && x.Status != ShowSeatStatus.Disabled
                && requestDto.ShowId == x.ShowId);

            if (cinemaHall.TotalSeats - validSeats.ToList().Count < 0)
            {
                result.Error = ErrorHandler.PopulateError(
                    (int)StatusCode.BadRequest,
                    $"Seat Is Already in Use or Not Available",
                    ErrorMessages.Generic.TypeBadRequest);

                return result;
            }


            var showSeats = new List<Infrastructure.Entities.ShowSeat>();
            foreach (var cinemaSeatId in requestDto.CinemaSeatIdList)
            {
                var showSeat = new Infrastructure.Entities.ShowSeat();

                var seat = await _unitOfWork.CinemaSeat.GetFirstOrDefaultAsync(x => x.Id == cinemaSeatId);

                if (seat == null)
                {
                    result.Error = ErrorHandler.PopulateError(
                        (int)StatusCode.BadRequest,
                        $"There is no Seat Anymore with this id = {cinemaSeatId}",
                        ErrorMessages.Generic.TypeBadRequest);

                    return result;
                }

                if (validSeats.Any(validSeat => validSeat.CinemaSeatId == seat.Id))
                {
                    result.Error = ErrorHandler.PopulateError(
                        (int)StatusCode.BadRequest,
                        $"Seat with this id {seat.Id} Is Already in Use",
                        ErrorMessages.Generic.TypeBadRequest);

                    return result;
                }

                showSeat.Price = seat.SeatType == SeatType.Vip ? requestDto.VipSeatPrice : requestDto.NormalSeatPrice;

                showSeat.ShowId = requestDto.ShowId;
                showSeat.CinemaSeatId = seat.Id;
                showSeat.CinemaSeat = seat; // For testing purpose

                showSeats.Add(showSeat);
            }

            if (await _unitOfWork.ShowSeat.AddRangeAsync(showSeats))
            {
                await _unitOfWork.SaveAsync();
                result.Content = "Show Seat ticket Added Successfully";

                return result;
            }


            result.Error = ErrorHandler.PopulateError(
                (int)StatusCode.BadRequest,
                ErrorMessages.Generic.UnableToProcess,
                ErrorMessages.Generic.TypeBadRequest);

            return result;
        }

        public async Task<Result<List<GetShowSeatResponseDto>>> GetAllShowSeat(int showId)
        {
            var result = new Result<List<GetShowSeatResponseDto>>();

            var showExist = await _unitOfWork.Show.GetFirstOrDefaultAsync(x =>
                x.Id == showId && x.IsDeleted == false, "Movie");
            if (showExist == null)
            {
                result.Error = ErrorHandler.PopulateError(
                    (int)StatusCode.BadRequest,
                    ErrorMessages.Generic.ObjectNotFound,
                    ErrorMessages.Generic.TypeBadRequest);

                return result;
            }

            var cinemaHall = await _unitOfWork.CinemaHall.GetFirstOrDefaultAsync(x => x.Id == showExist.CinemaHallId);
            if (cinemaHall == null)
            {
                result.Error = ErrorHandler.PopulateError(
                    (int)StatusCode.BadRequest,
                    ErrorMessages.Generic.ObjectNotFound,
                    ErrorMessages.Generic.TypeBadRequest);

                return result;
            }

            var showSeats = await _unitOfWork.ShowSeat.GetAllFilterAsync(
                x => x.ShowId == showId && x.Status != ShowSeatStatus.Disabled, "CinemaSeat");
            var mappedShowSeats = _mapper.Map<List<GetShowSeatResponseDto>>((List<Infrastructure.Entities.ShowSeat>)showSeats);
            result.Content = mappedShowSeats;

            return result;
        }

        public async Task<Result<string>> DeleteShowSeats(int showId)
        {
            var result = new Result<string>();

            var showExist = await _unitOfWork.Show.GetFirstOrDefaultAsync(x => x.Id == showId, "Movie");
            if (showExist == null)
            {
                result.Error = ErrorHandler.PopulateError(
                    (int)StatusCode.BadRequest,
                    ErrorMessages.Generic.ObjectNotFound,
                    ErrorMessages.Generic.TypeBadRequest);

                return result;
            }

            var cinemaHall = await _unitOfWork.CinemaHall.GetFirstOrDefaultAsync(x => x.Id == showExist.CinemaHallId);
            if (cinemaHall == null)
            {
                result.Error = ErrorHandler.PopulateError(
                    (int)StatusCode.BadRequest,
                    ErrorMessages.Generic.ObjectNotFound,
                    ErrorMessages.Generic.TypeBadRequest);

                return result;
            }

            var showSeats = await _unitOfWork.ShowSeat.GetAllFilterAsync(x =>
                x.ShowId == showId && x.Status != ShowSeatStatus.Disabled && x.IsDeleted == false);

            if (showSeats.Any())
            {
                foreach (var showSeat in showSeats)
                {
                    showSeat.Status = ShowSeatStatus.Disabled;
                }
                await _unitOfWork.SaveAsync();
                result.Content = "Show Seats Disabled successfully";
                return result;
            }

            result.Error = new Error()
            {
                Type = ErrorMessages.Generic.TypeBadRequest,
                Message = ErrorMessages.Generic.UnableToProcess,
                Code = (int)StatusCode.BadRequest
            };

            return result;
        }

    }
}
