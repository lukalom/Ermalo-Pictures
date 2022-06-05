using EP.Application.DTO_General.Generic;
using EP.Application.Services.Movie.DTO.Request;
using EP.Application.Services.Movie.DTO.Response;

namespace EP.Application.Services.Movie
{
    public interface IMovieService
    {
        Task<PagedResult<GetAllMoviesResponseDto>> GetAll(MovieParameters movieParameters);
        Task<Result<GetMovieResponseDto>> GetById(int? id);
        Task<Result<bool>> Delete(int? id);
        Task<Result<CreateMovieResponseDto>> CreateMovie(MovieUploadRequestDto modelDto);
        Task<Result<EditMovieResponseDto>> EditMovie(EditMovieDto modelDto);

    }
}
