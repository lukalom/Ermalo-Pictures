using AutoMapper;
using EP.Application.DTO_General.Extension;
using EP.Application.DTO_General.Generic;
using EP.Application.Services.Cinema_Management.CinemaHall.DTO;
using EP.Infrastructure.Enums;
using EP.Infrastructure.IConfiguration;
using EP.Shared.Exceptions.Messages;

namespace EP.Application.Services.Cinema_Management.CinemaHall
{
    public class CinemaHallService : ICinemaHallService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CinemaHallService(IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<Result<string>> CreateCinemaHall(CreateCinemaHallRequestDto requestDto)
        {
            var result = new Result<string>();
            var cinema = await _unitOfWork.Cinema.GetFirstOrDefaultAsync(x => x.Id == requestDto.cinemaId);
            if (cinema == null)
            {
                result.Error = ErrorHandler.PopulateError((int)StatusCode.NotFound,
                    ErrorMessages.Generic.ObjectNotFound,
                    ErrorMessages.Generic.TypeBadRequest);
                return result;
            }

            var hallExist = await _unitOfWork.CinemaHall.GetFirstOrDefaultAsync(x => x.Name == requestDto.hallName);

            if (hallExist != null)
            {
                result.Error = ErrorHandler.PopulateError((int)StatusCode.BadRequest,
                    $"hall already exist with tis name {requestDto.hallName}",
                    ErrorMessages.Generic.TypeBadRequest);
                return result;
            }

            var hallResult = await _unitOfWork.CinemaHall.AddAsync(new Infrastructure.Entities.CinemaHall()
            {
                Name = requestDto.hallName,
                TotalSeats = requestDto.totalSeats,
                CinemaId = cinema.Id,
                Rows = requestDto.Rows,
                Columns = requestDto.Columns
            });

            if (hallResult)
            {
                result.Content = $"Hall created successfully {requestDto.hallName}";
                await _unitOfWork.SaveAsync();
                return result;
            }

            result.Error = ErrorHandler.PopulateError((int)StatusCode.BadRequest,
                ErrorMessages.Generic.UnableToProcess,
                ErrorMessages.Generic.TypeBadRequest);
            return result;
        }

        public async Task<Result<List<GetCinemaHallResponseDto>>> GetAllCinemaHall(int cinemaId)
        {
            var result = new Result<List<GetCinemaHallResponseDto>>();
            var cinema = await _unitOfWork.Cinema.GetFirstOrDefaultAsync(x => x.Id == cinemaId);
            if (cinema == null)
            {
                result.Error = ErrorHandler.PopulateError((int)StatusCode.BadRequest,
                    $"cinema with this id does not exists",
                    ErrorMessages.Generic.TypeBadRequest);
                return result;
            }

            var halls = await _unitOfWork.CinemaHall.GetAllFilterAsync(x => x.CinemaId == cinemaId);

            if (halls.Any())
            {
                var mappedResult = _mapper.Map<List<GetCinemaHallResponseDto>>(halls);
                result.Content = mappedResult;
                return result;
            }

            result.Error = ErrorHandler.PopulateError((int)StatusCode.BadRequest,
                "hall does not exist",
                ErrorMessages.Generic.TypeBadRequest);
            return result;

        }

        public async Task<Result<GetCinemaHallResponseDto>> GetCinemaHallById(GetCinemaHallByIdRequestDto requestDto)
        {
            var result = new Result<GetCinemaHallResponseDto>();
            var cinema = await _unitOfWork.Cinema.GetFirstOrDefaultAsync(x => x.Id == requestDto.cinemaId);
            if (cinema == null)
            {
                result.Error = ErrorHandler.PopulateError((int)StatusCode.BadRequest,
                    $"cinema with this id does not exists",
                    ErrorMessages.Generic.TypeBadRequest);
                return result;
            }

            var hall = await _unitOfWork.CinemaHall.GetFirstOrDefaultAsync(x =>
                x.Id == requestDto.hallId && x.CinemaId == requestDto.cinemaId);

            if (hall == null)
            {
                result.Error = ErrorHandler.PopulateError((int)StatusCode.BadRequest,
                    ErrorMessages.Generic.ObjectNotFound,
                    ErrorMessages.Generic.TypeBadRequest);
                return result;
            }

            var mappedHall = _mapper.Map<GetCinemaHallResponseDto>(hall);
            result.Content = mappedHall;
            return result;
        }

    }
}
