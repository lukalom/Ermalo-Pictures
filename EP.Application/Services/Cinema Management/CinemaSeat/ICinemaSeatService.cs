using EP.Application.DTO_General.Generic;
using EP.Application.Services.Cinema_Management.CinemaSeat.DTO;

namespace EP.Application.Services.Cinema_Management.CinemaSeat
{
    public interface ICinemaSeatService
    {
        Task<Result<string>> AddCinemaSeats(AddCinemaSeatsRequestDto requestDto);
        Task<Result<List<GetSeatResponseDto>>> GetSeats(int cinemaHallId);
        Task<Result<string>> UpdateSeatStatus(UpdateSeatStatusRequestDto requestDto);
    }
}
