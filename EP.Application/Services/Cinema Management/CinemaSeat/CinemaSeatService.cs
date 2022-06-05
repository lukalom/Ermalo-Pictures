using AutoMapper;
using EP.Application.DTO_General.Extension;
using EP.Application.DTO_General.Generic;
using EP.Application.Services.Cinema_Management.CinemaSeat.DTO;
using EP.Infrastructure.Enums;
using EP.Infrastructure.IConfiguration;
using EP.Shared.Exceptions.Messages;

namespace EP.Application.Services.Cinema_Management.CinemaSeat
{
    public class CinemaSeatService : ICinemaSeatService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CinemaSeatService(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<string>> AddCinemaSeats(AddCinemaSeatsRequestDto requestDto)
        {
            var result = new Result<string>();

            var cinemaHall = await _unitOfWork.CinemaHall.GetFirstOrDefaultAsync(x => x.Id == requestDto.CinemaHallId);
            if (cinemaHall == null)
            {
                result.Error = ErrorHandler.PopulateError((int)StatusCode.NotFound,
                    ErrorMessages.Generic.ObjectNotFound,
                    ErrorMessages.Generic.TypeBadRequest);
                return result;
            }

            if (!(requestDto.SeatList.Count > 0 && requestDto.SeatList.Count <= cinemaHall.TotalSeats))
            {
                result.Error = ErrorHandler.PopulateError((int)StatusCode.NotFound,
                    "seat length should be more than 0 and less than total seat",
                    ErrorMessages.Generic.TypeBadRequest);
                return result;
            }

            if (requestDto.SeatList.Select(matrix => requestDto.SeatList.Where(x =>
                    x.ColumnNumber == matrix.ColumnNumber && x.RowNumber == matrix.RowNumber))
                .Any(validPayload => validPayload.Count() >= 2))
            {
                result.Error = ErrorHandler.PopulateError((int)StatusCode.NotFound,
                    $"row number and column number is invalid or already in use",
                    ErrorMessages.Generic.TypeBadRequest);
                return result;
            }

            var seats = new List<Infrastructure.Entities.CinemaSeat>();
            foreach (var seatInfo in requestDto.SeatList)
            {
                var seat = await _unitOfWork.CinemaSeat.GetFirstOrDefaultAsync(x => x.CinemaHallId == requestDto.CinemaHallId &&
                    x.ColumnNumber == seatInfo.ColumnNumber && x.RowNumber == seatInfo.RowNumber);

                if (seat != null)
                {
                    result.Error = ErrorHandler.PopulateError((int)StatusCode.NotFound,
                        $"seat with this rowNumber {seatInfo.RowNumber} and columnNumber {seatInfo.ColumnNumber} already exists",
                        ErrorMessages.Generic.TypeBadRequest);
                    return result;
                }

                seats.Add(new Infrastructure.Entities.CinemaSeat()
                {
                    CinemaHallId = requestDto.CinemaHallId,
                    ColumnNumber = seatInfo.ColumnNumber,
                    RowNumber = seatInfo.RowNumber,
                    SeatType = seatInfo.SeatType
                });
            }

            var resultSeat = await _unitOfWork.CinemaSeat.AddRangeAsync(seats);

            if (resultSeat)
            {
                await _unitOfWork.SaveAsync();
                result.Content = $"Seats added successfully";
                return result;
            }

            result.Error = ErrorHandler.PopulateError((int)StatusCode.NotFound,
                ErrorMessages.Generic.UnableToProcess,
                ErrorMessages.Generic.TypeBadRequest);
            return result;
        }

        public async Task<Result<List<GetSeatResponseDto>>> GetSeats(int cinemaHallId)
        {
            var result = new Result<List<GetSeatResponseDto>>();

            var cinemaHall = await _unitOfWork.CinemaHall.GetFirstOrDefaultAsync(x => x.Id == cinemaHallId);
            if (cinemaHall == null)
            {
                result.Error = ErrorHandler.PopulateError((int)StatusCode.NotFound,
                    ErrorMessages.Generic.ObjectNotFound,
                    ErrorMessages.Generic.TypeBadRequest);
                return result;
            }

            var seats = await _unitOfWork.CinemaSeat.GetAllFilterAsync(x => x.CinemaHallId == cinemaHallId);
            if (!seats.Any())
            {
                result.Error = ErrorHandler.PopulateError((int)StatusCode.NotFound,
                    $"No Seats in this hall {cinemaHall.Name}",
                    ErrorMessages.Generic.TypeBadRequest);
                return result;
            }

            var mappedSeats = _mapper.Map<List<GetSeatResponseDto>>(seats);

            result.Content = mappedSeats;
            return result;
        }

        public async Task<Result<string>> UpdateSeatStatus(UpdateSeatStatusRequestDto requestDto)
        {
            var result = new Result<string>();

            var cinemaHall = await _unitOfWork.CinemaHall.GetFirstOrDefaultAsync(x => x.Id == requestDto.CinemaHallId);
            if (cinemaHall == null)
            {
                result.Error = ErrorHandler.PopulateError((int)StatusCode.NotFound,
                    ErrorMessages.Generic.ObjectNotFound,
                    ErrorMessages.Generic.TypeBadRequest);
                return result;
            }

            var seat = await _unitOfWork.CinemaSeat.GetFirstOrDefaultAsync(x => x.Id == requestDto.SeatId);
            if (seat == null)
            {
                result.Error = ErrorHandler.PopulateError((int)StatusCode.NotFound,
                    ErrorMessages.Generic.InvalidPayload,
                    ErrorMessages.Generic.TypeBadRequest);
                return result;
            }

            if (requestDto.SeatType is SeatType.Medium or SeatType.Vip)
            {
                seat.SeatType = requestDto.SeatType;
                await _unitOfWork.SaveAsync();
                result.Content = $"Status successfully updated to {requestDto.SeatType}";
                return result;
            }

            result.Error = ErrorHandler.PopulateError((int)StatusCode.NotFound,
                ErrorMessages.Generic.InvalidPayload,
                ErrorMessages.Generic.TypeBadRequest);
            return result;
        }
    }
}
