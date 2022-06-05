using AutoMapper;
using EP.Application.DTO_General.Extension;
using EP.Application.DTO_General.Generic;
using EP.Application.Services.Show.DTO.Request;
using EP.Application.Services.Show.DTO.Response;
using EP.Infrastructure.Enums;
using EP.Infrastructure.IConfiguration;
using EP.Shared.Exceptions;
using EP.Shared.Exceptions.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EP.Application.Services.Show
{
    public class ShowService : IShowService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ShowService> _logger;

        public ShowService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ShowService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<List<GetShowResponseDto>>> GetShows(int cinemaHallId)
        {
            var result = new Result<List<GetShowResponseDto>>();
            var cinemaHall = await _unitOfWork.CinemaHall.FindByCondition(x => x.Id == cinemaHallId)
                .FirstOrDefaultAsync();
            if (cinemaHall == null)
            {
                result.Error = ErrorHandler.PopulateError(
                    (int)StatusCode.BadRequest,
                    ErrorMessages.Generic.InvalidPayload,
                    ErrorMessages.Generic.TypeBadRequest);

                return result;
            }

            var show = await _unitOfWork.Show.GetAllFilterAsync(x => x.CinemaHallId == cinemaHallId && x.IsDeleted == false,
                includeProperties: "Movie");

            if (show == null)
            {
                result.Error = ErrorHandler.PopulateError(
                    (int)StatusCode.BadRequest,
                    ErrorMessages.Generic.InvalidPayload,
                    ErrorMessages.Generic.TypeBadRequest);

                return result;
            }

            var mappedDto = _mapper.Map<List<GetShowResponseDto>>(show);

            result.Content = mappedDto;
            return result;
        }

        public async Task<Result<string>> DeleteShow(int showId)
        {
            var result = new Result<string>();
            var show = await _unitOfWork.Show.GetFirstOrDefaultAsync(x => x.Id == showId);

            if (show == null)
            {
                result.Error = ErrorHandler.PopulateError(
                    (int)StatusCode.BadRequest,
                    ErrorMessages.Generic.InvalidPayload,
                    ErrorMessages.Generic.TypeBadRequest);

                return result;
            }

            if (_unitOfWork.Show.Remove(show))
            {
                await _unitOfWork.SaveAsync();
                result.Content = "show removed successfully";
                return result;
            }

            result.Error = ErrorHandler.PopulateError(
                (int)StatusCode.BadRequest,
                ErrorMessages.Generic.UnableToProcess,
                ErrorMessages.Generic.TypeBadRequest);

            return result;
        }

        public async Task<Result<GetShowResponseDto>> CreateShow(CreateShowRequestDto requestDto)
        {
            var result = new Result<GetShowResponseDto>();

            var cinemaHall = await _unitOfWork.CinemaHall.GetFirstOrDefaultAsync(x => x.Id == requestDto.CinemaHallId);

            if (cinemaHall == null)
            {
                result.Error = ErrorHandler.PopulateError(
                    (int)StatusCode.BadRequest,
                    ErrorMessages.Generic.ObjectNotFound,
                    ErrorMessages.Generic.TypeBadRequest);

                return result;
            }

            var movie = await _unitOfWork.Movie.GetFirstOrDefaultAsync(x => x.Id == requestDto.MovieId);

            if (movie == null)
            {
                result.Error = ErrorHandler.PopulateError(
                    (int)StatusCode.BadRequest,
                    ErrorMessages.Generic.ObjectNotFound,
                    ErrorMessages.Generic.TypeBadRequest);

                return result;
            }

            var show = new Infrastructure.Entities.Show()
            {
                Date = DateTime.Now,
                StartTime = requestDto.StarTime,
                EndTime = requestDto.EndTime,
                CinemaHallId = requestDto.CinemaHallId,
                MovieId = requestDto.MovieId,
                Movie = movie,
                CinemaHall = cinemaHall,
            };

            var mappedDto = _mapper.Map<GetShowResponseDto>(show);
            mappedDto.Movie = movie.Title;
            result.Content = mappedDto;

            var showResult = await _unitOfWork.Show.AddAsync(show);
            if (showResult)
            {
                _logger.LogInformation("Show Added Successfully");
                await _unitOfWork.SaveAsync();
                result.Content = mappedDto;
                return result;
            }

            result.Error = ErrorHandler.PopulateError(
                (int)StatusCode.BadRequest,
                "In this session movie already exist",
                ErrorMessages.Generic.TypeBadRequest);

            return result;
        }

        public async Task<Result<GetShowResponseDto>> EditShow(EditShowRequestDto requestDto)
        {
            var result = new Result<GetShowResponseDto>();

            var cinemaHall = await _unitOfWork.CinemaHall.GetFirstOrDefaultAsync(x => x.Id == requestDto.CinemaHallId);

            if (cinemaHall == null)
            {
                result.Error = ErrorHandler.PopulateError(
                    (int)StatusCode.BadRequest,
                    ErrorMessages.Generic.ObjectNotFound,
                    ErrorMessages.Generic.TypeBadRequest);

                return result;
            }

            var movie = await _unitOfWork.Movie.GetFirstOrDefaultAsync(x => x.Id == requestDto.MovieId);

            if (movie == null)
            {
                result.Error = ErrorHandler.PopulateError(
                    (int)StatusCode.BadRequest,
                    ErrorMessages.Generic.ObjectNotFound,
                    ErrorMessages.Generic.TypeBadRequest);

                return result;
            }

            var show = await _unitOfWork.Show.GetFirstOrDefaultAsync(x =>
                x.Id == requestDto.ShowId && x.IsDeleted == false);

            if (show == null) throw new AppException("Invalid show Id");

            show.StartTime = requestDto.StartTime;
            show.EndTime = requestDto.EndTime;
            show.MovieId = requestDto.MovieId;
            show.CinemaHallId = requestDto.CinemaHallId;
            await _unitOfWork.SaveAsync();

            var mappedDto = _mapper.Map<GetShowResponseDto>(show);
            mappedDto.Movie = movie.Title;

            result.Content = mappedDto;
            return result;

        }
    }
}
