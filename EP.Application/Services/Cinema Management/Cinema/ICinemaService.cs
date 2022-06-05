using EP.Application.DTO_General.Generic;
using EP.Application.Services.Cinema_Management.Cinema.DTO.Request;
using EP.Application.Services.Cinema_Management.Cinema.DTO.Response;

namespace EP.Application.Services.Cinema_Management.Cinema
{
    public interface ICinemaService
    {

        Task<Result<GetCinemaResponseDto>> GetCinema(string name);
        
        Task<Result<EditCinemaResponseDto>> EditCinema(EditCinemaRequestDto requestDto);
        Task<Result<AddCinemaResponseDto>> AddCinema(AddCinemaRequestDto request);
        Task<Result<DeleteCinemaResponseDto>> DeleteCinema(DeleteCinemaRequestDto request);
        Task<Result<RestoreCinemaResponseDto>> RestoreCinema(RestoreCinemaRequestDto request);

    }
}
