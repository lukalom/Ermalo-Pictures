using AutoMapper;
using EP.Application.DTO_General.Errors;
using EP.Application.DTO_General.Extension;
using EP.Application.DTO_General.Generic;
using EP.Application.Extensions;
using EP.Application.Services.Movie.DTO.Request;
using EP.Application.Services.Movie.DTO.Response;
using EP.Infrastructure.Entities;
using EP.Infrastructure.Enums;
using EP.Infrastructure.IConfiguration;
using EP.Shared.Exceptions;
using EP.Shared.Exceptions.Messages;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EP.Application.Services.Movie
{
    public class MovieService : IMovieService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IMapper _mapper;
        private readonly ILogger<MovieService> _logger;

        public MovieService(
            IUnitOfWork unitOfWork,
            IWebHostEnvironment hostEnvironment,
            IMapper mapper,
            ILogger<MovieService> logger)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PagedResult<GetAllMoviesResponseDto>> GetAll(MovieParameters movieParameters)
        {
            var result = new PagedResult<GetAllMoviesResponseDto>();

            var cachedMoviesResult = MemoryCache<PagedResult<GetAllMoviesResponseDto>>
                .GetOrCreate($"{movieParameters.Page}{movieParameters.PageSize}"); //sg

            _logger.LogInformation("caching movies in memory");

            if (cachedMoviesResult != null) return cachedMoviesResult;

            var paginatedData = await _unitOfWork.Movie.Query()
                .AsNoTracking()
                .OrderBy(h => h.CreatedOnUtc)
                .PaginateAsync(movieParameters.Page, movieParameters.PageSize);

            var metaData = new MetaData()
            {
                CurrentPage = paginatedData.CurrentPage,
                TotalItems = paginatedData.TotalItems,
                TotalPages = paginatedData.TotalPages,
                HasNext = paginatedData.HasNext,
                HasPrevious = paginatedData.HasPrevious,
            };

            if (!paginatedData.Items.Any())
            {
                result.Error = ErrorHandler.PopulateError((int)StatusCode.NotFound,
                    ErrorMessages.Generic.ObjectNotFound,
                    ErrorMessages.Generic.InvalidRequest);

                return result;
            }

            var movieResDto = new List<GetMovieResponseDto>();
            foreach (var movie in paginatedData.Items)
            {
                movie.Movie_Genres = (await _unitOfWork.MovieGenres
                        .GetAllFilterAsync(x => x.MovieId == movie.Id, "Genre")
                    as List<Movie_Genre> ?? null) ?? throw new AppException("No genres");

                var genres = new List<string>();
                movie.Movie_Genres.ForEach(x => genres.Add(x.Genre.Name.ToString()));

                movieResDto.Add(new GetMovieResponseDto()
                {
                    Title = movie.Title,
                    Description = movie.Description,
                    DurationInMinutes = movie.DurationInMinutes,
                    Language = movie.Language.ToString(),
                    ReleaseDate = movie.ReleaseDate.ToString("d"),
                    Country = movie.Country,
                    Director = movie.Director,
                    Genres = genres,
                    ImageUrl = movie.ImageUrl
                });
            }

            var response = new GetAllMoviesResponseDto()
            {
                MetaData = metaData,
                Movies = movieResDto
            };

            MemoryCache<PagedResult<GetAllMoviesResponseDto>>.GetOrCreate($"{movieParameters.Page}{movieParameters.PageSize}",
                () => result);

            result.Content = response;
            return result;
        }

        public async Task<Result<GetMovieResponseDto>> GetById(int? id)
        {
            var result = new Result<GetMovieResponseDto>();

            if (id is null or 0)
            {
                result.Error = new Error()
                {
                    Code = (int)StatusCode.NotFound,
                    Message = ErrorMessages.Generic.InvalidPayload,
                    Type = ErrorMessages.Generic.TypeBadRequest
                };

                return result;
            }

            var movie = await _unitOfWork.Movie.GetFirstOrDefaultAsync(x => x.Id == id);
            if (movie == null)
            {
                result.Error = new Error()
                {
                    Code = (int)StatusCode.NotFound,
                    Message = ErrorMessages.Generic.ObjectNotFound,
                    Type = ErrorMessages.Generic.TypeBadRequest
                };

                return result;
            }

            await _unitOfWork.MovieGenres.GetAllFilterAsync(x => x.MovieId == id, "Genre");

            var mappedMovie = _mapper.Map<GetMovieResponseDto>(movie);
            result.Content = mappedMovie;

            return result;
        }

        public async Task<Result<CreateMovieResponseDto>> CreateMovie(MovieUploadRequestDto model)
        {
            var result = new Result<CreateMovieResponseDto>();

            if (model.File == null) throw new AppException("Please Upload file");

            var wwWebRootPathPath = _hostEnvironment.WebRootPath;
            var fileName = Guid.NewGuid().ToString();
            var uploads = Path.Combine(wwWebRootPathPath, @"Images\Movies");
            var extension = Path.GetExtension(model.File.FileName);

            await using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
            {
                await model.File.CopyToAsync(fileStreams);
            }

            var movie = _mapper.Map<Infrastructure.Entities.Movie>(model);
            movie.ImageUrl = @"\Images\Movies\" + fileName + extension;
            await _unitOfWork.Movie.AddAsync(movie);
            await _unitOfWork.SaveAsync();

            var movieGenres = model.GenreIdList
                .Select(genreId => new Movie_Genre() { MovieId = movie.Id, GenreId = genreId }).ToList();

            if (await _unitOfWork.MovieGenres.AddRangeAsync(movieGenres)) await _unitOfWork.SaveAsync();

            var movieDto = _mapper.Map<CreateMovieResponseDto>(model);
            movieDto.Image = movie.ImageUrl;
            foreach (var movieGenre in movieGenres)
            {
                var genre = await _unitOfWork.Genre.GetFirstOrDefaultAsync(x => x.Id == movieGenre.GenreId);
                movieDto.Genres.Add(genre.Name);
            }

            result.Content = movieDto;
            return result;
        }

        public async Task<Result<EditMovieResponseDto>> EditMovie(EditMovieDto model)
        {
            var result = new Result<EditMovieResponseDto>();

            var movie = await _unitOfWork.Movie.GetFirstOrDefaultAsync(x => x.Id == model.MovieId);
            if (movie == null) throw new AppException("Invalid movie");

            var wwWebRootPath = _hostEnvironment.WebRootPath;

            if (model.File != null)
            {
                var fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwWebRootPath, @"Images\Movies");
                var extension = Path.GetExtension(model.File.FileName);

                if (model.ImageUrl != null)
                {
                    var oldImagePath = Path.Combine(wwWebRootPath, model.ImageUrl.TrimStart('\\'));
                    if (File.Exists(oldImagePath))
                    {
                        File.Delete(oldImagePath);
                    }
                }

                await using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    await model.File.CopyToAsync(fileStreams);
                }

                model.ImageUrl = @"\Images\Movies\" + fileName + extension;
            }

            if (_unitOfWork.MovieGenres.RemoveRange(await _unitOfWork.MovieGenres.GetAllFilterAsync(x => x.MovieId == model.MovieId)))
            {
                var movieGenre = model.GenreIdList
                    .Select(genreId => new Movie_Genre() { MovieId = model.MovieId, GenreId = genreId }).ToList();
                await _unitOfWork.MovieGenres.AddRangeAsync(movieGenre);
            }

            //var mappedMovie = _mapper.Map<Infrastructure.Entities.Movie>(model);
            movie.Country = model.Country;
            movie.ImageUrl = model.ImageUrl;
            movie.Description = model.Description;
            movie.Title = model.Title;
            movie.Language = model.Language;
            movie.DurationInMinutes = model.DurationInMinutes;
            movie.ReleaseDate = model.ReleaseDate;
            movie.Director = model.Director;
            await _unitOfWork.SaveAsync();

            var movieGenres = await _unitOfWork.MovieGenres.GetAllFilterAsync(x => x.MovieId == movie.Id);
            var mappedMovieDto = _mapper.Map<EditMovieResponseDto>(movie);
            foreach (var movieGenre in movieGenres)
            {
                var genre = await _unitOfWork.Genre.GetFirstOrDefaultAsync(x => x.Id == movieGenre.GenreId);
                mappedMovieDto.Genres.Add(genre.Name);
            }
            mappedMovieDto.ImageUrl = movie.ImageUrl;

            result.Content = mappedMovieDto;
            return result;
        }

        public async Task<Result<bool>> Delete(int? id)
        {
            var result = new Result<bool>();

            if (id is null or 0)
            {
                result.Error = new Error()
                {
                    Code = (int)StatusCode.NotFound,
                    Message = ErrorMessages.Generic.InvalidPayload,
                    Type = ErrorMessages.Generic.TypeBadRequest
                };

                return result;
            }

            var movie = await _unitOfWork.Movie.GetFirstOrDefaultAsync(x => x.Id == id);

            if (movie == null)
            {
                result.Error = ErrorHandler.PopulateError((int)StatusCode.NotFound,
                    ErrorMessages.Generic.ObjectNotFound,
                    ErrorMessages.Generic.InvalidRequest);

                return result;
            }

            var wwWebRootPath = _hostEnvironment.WebRootPath;
            var imagePath = Path.Combine(wwWebRootPath, movie.ImageUrl.TrimStart('\\'));
            if (File.Exists(imagePath))
            {
                File.Delete(imagePath);
            }

            if (_unitOfWork.Movie.Remove(movie))
            {
                await _unitOfWork.SaveAsync();
                result.Content = true;
                return result;
            }

            result.Error = ErrorHandler.PopulateError((int)StatusCode.NotFound,
                ErrorMessages.Generic.UnableToProcess,
                ErrorMessages.Generic.SomethingWentWrong);

            return result;
        }
    }
}

