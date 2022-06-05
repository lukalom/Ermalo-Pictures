using EP.Application.DTO_General.Generic;
using EP.Application.Services.ShowSeat.DTO;

namespace EP.Application.Services.ShowSeat
{
    public interface IShowSeatService
    {
        Task<Result<List<GetShowSeatResponseDto>>> GetAllShowSeat(int showId);
        Task<Result<string>> CreateShowSeats(CreateShowSeatRequestDto requestDto);
        Task<Result<string>> DeleteShowSeats(int showId);
        Task<Result<string>> ChangePriceToShowSeat(int showSeatId, decimal newPrice);
    }
}
