using EP.Application.DTO_General.Generic;
using EP.Application.Services.Show.DTO.Request;
using EP.Application.Services.Show.DTO.Response;

namespace EP.Application.Services.Show
{
    public interface IShowService
    {
        Task<Result<List<GetShowResponseDto>>> GetShows(int cinemaHallId);
        Task<Result<GetShowResponseDto>> CreateShow(CreateShowRequestDto requestDto);
        Task<Result<string>> DeleteShow(int showId);
        Task<Result<GetShowResponseDto>> EditShow(EditShowRequestDto requestDto);
    }
}
