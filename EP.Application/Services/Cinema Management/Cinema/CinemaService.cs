using EP.Application.DTO_General.Extension;
using EP.Application.DTO_General.Generic;
using EP.Application.Services.Cinema_Management.Cinema.DTO.Request;
using EP.Application.Services.Cinema_Management.Cinema.DTO.Response;
using EP.Infrastructure.Enums;
using EP.Infrastructure.IConfiguration;
using EP.Shared.Exceptions;
using EP.Shared.Exceptions.Messages;

namespace EP.Application.Services.Cinema_Management.Cinema
{
    public class CinemaService : ICinemaService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CinemaService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<GetCinemaResponseDto>> GetCinema(string name)
        {
            var result = new Result<GetCinemaResponseDto>();
            var cinema = await _unitOfWork.Cinema.GetFirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower() && x.IsDeleted == false);

            if (cinema == null)
            {
                result.Error = ErrorHandler.PopulateError(
                    (int)StatusCode.BadRequest,
                    ErrorMessages.Generic.ObjectNotFound,
                    ErrorMessages.Generic.TypeBadRequest);

                return result;
            }

            var responseDto = new GetCinemaResponseDto();

            var cinemaHalls = await _unitOfWork.CinemaHall.GetAllFilterAsync(x => x.CinemaId == cinema.Id && x.IsDeleted == false);
            foreach (var cinemaHall in cinemaHalls)
            {
                var show = await _unitOfWork.Show.GetFirstOrDefaultAsync(x => x.CinemaHallId == cinemaHall.Id, includeProperties: "Movie");
                responseDto.Shows.Add(show);
            }

            responseDto.CinemaHall = cinemaHalls.ToList();
            result.Content = responseDto;

            return result;
        }
        public async Task<Result<AddCinemaResponseDto>> AddCinema(AddCinemaRequestDto request)
        {
            var result = new Result<AddCinemaResponseDto>();
            var cinema = await _unitOfWork.Cinema.GetFirstOrDefaultAsync(x => x.Name == request.Name);

            if (cinema != null)
            {
                result.Error = ErrorHandler.PopulateError(
                    (int)StatusCode.Dublicate,
                    ErrorMessages.Generic.AlreadyExists,
                    ErrorMessages.Generic.TypeBadRequest);

                return result;
            }

            var entity = new Infrastructure.Entities.Cinema
            {
                Name = request.Name
            };

            await _unitOfWork.Cinema.AddAsync(entity);
            await _unitOfWork.SaveAsync();

            result.Content = new AddCinemaResponseDto
            {
                Name = entity.Name,
                CreatedOnUtc = entity.CreatedOnUtc,
                IsDeleted = entity.IsDeleted
            };

            return result;
        }
        public async Task<Result<EditCinemaResponseDto>> EditCinema(EditCinemaRequestDto request)
        {
            var result = new Result<EditCinemaResponseDto>();
            var cinema = await _unitOfWork.Cinema.GetSingleOrDefaultAsync(request.Id);

            if (cinema == null)
            {
                result.Error = ErrorHandler.PopulateError(
                    (int)StatusCode.BadRequest,
                    ErrorMessages.Generic.ObjectNotFound,
                    ErrorMessages.Generic.TypeBadRequest);

                return result;
            }
            cinema.Name = request.Name;
            await _unitOfWork.SaveAsync();

            result.Content = new EditCinemaResponseDto
            {
                Name = cinema.Name
            };

            return result;
        }
        public async Task<Result<DeleteCinemaResponseDto>> DeleteCinema(DeleteCinemaRequestDto request)
        {
            var result = new Result<DeleteCinemaResponseDto>();
            if (request.Id <= 0)
            {
                throw new AppException("Id Should not be Zero or less");
            }

            var cinema = await _unitOfWork.Cinema.GetFirstOrDefaultAsync(x => x.Id == request.Id && x.IsDeleted == false);
            if (cinema == null)
            {
                result.Error = ErrorHandler.PopulateError(
                   (int)StatusCode.NotFound,
                   ErrorMessages.Generic.ObjectNotFound,
                   ErrorMessages.Generic.TypeBadRequest);

                return result;
            }
            if (cinema.IsDeleted == true)
            {
                result.Content = new DeleteCinemaResponseDto
                {
                    Name = cinema.Name,
                    IsDeleted = true,
                    Message = "Cinema is already deleted"
                };
                return result;
            }
            cinema.IsDeleted = true;
            await _unitOfWork.SaveAsync();

            result.Content = new DeleteCinemaResponseDto
            {
                Name = cinema.Name,
                IsDeleted = true,
                Message = "Cinema Deleted Successfully"
            };

            return result;
        }
        public async Task<Result<RestoreCinemaResponseDto>> RestoreCinema(RestoreCinemaRequestDto request)
        {
            var result = new Result<RestoreCinemaResponseDto>();

            if (request.Id <= 0)
            {
                throw new AppException("Id Should not be Zero or less");
            }

            var cinema = await _unitOfWork.Cinema.GetFirstOrDefaultAsync(x => x.Id == request.Id && x.IsDeleted == true);
            if (cinema == null)
            {
                result.Error = ErrorHandler.PopulateError(
                   (int)StatusCode.NotFound,
                   ErrorMessages.Generic.ObjectNotFound,
                   ErrorMessages.Generic.TypeBadRequest);

                return result;
            }
            cinema.IsDeleted = false;
            await _unitOfWork.SaveAsync();

            result.Content = new RestoreCinemaResponseDto
            {
                Name = cinema.Name,
                IsDeleted = false,
                Message = "Cinema is restored"
            };
            return result;

        }
    }
}
