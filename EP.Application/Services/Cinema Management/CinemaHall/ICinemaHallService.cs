using EP.Application.DTO_General.Generic;
using EP.Application.Services.Cinema_Management.CinemaHall.DTO;

namespace EP.Application.Services.Cinema_Management.CinemaHall
{
    public interface ICinemaHallService
    {
        Task<Result<string>> CreateCinemaHall(CreateCinemaHallRequestDto requestDto);
        Task<Result<List<GetCinemaHallResponseDto>>> GetAllCinemaHall(int cinemaId);
        Task<Result<GetCinemaHallResponseDto>> GetCinemaHallById(GetCinemaHallByIdRequestDto requestDto);
    }
}
