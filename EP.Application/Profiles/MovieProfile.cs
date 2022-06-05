using AutoMapper;
using EP.Application.Services.Movie.DTO.Request;
using EP.Application.Services.Movie.DTO.Response;
using EP.Infrastructure.Entities;
using EP.Infrastructure.Enums;

namespace EP.Application.Profiles
{
    public class MovieProfile : Profile
    {
        public MovieProfile()
        {
            CreateMap<EditMovieDto, Movie>();

            CreateMap<Movie, GetMovieResponseDto>()
                .ForMember(dest => dest.Genres,
                    opt =>
                        opt.MapFrom(src => src.Movie_Genres.Select(x => x.Genre.Name)));

            CreateMap<MovieUploadRequestDto, Movie>();

            CreateMap<MovieUploadRequestDto, CreateMovieResponseDto>()
                .ForMember(dest => dest.Language,
                    opt => opt.MapFrom(x =>

                        x.Language == (Language)1 ? Language.Eng.ToString() : Language.Geo.ToString()
                    ))
                .ForMember(dest => dest.ReleaseDate,
                    opt => opt.MapFrom(x => $"{x.ReleaseDate:MMMM dd} { x.ReleaseDate:H:mm}"));


            CreateMap<Movie, EditMovieResponseDto>()
                .ForMember(dest => dest.Language,
                    opt => opt.MapFrom(x =>

                        x.Language == (Language)1 ? Language.Eng.ToString() : Language.Geo.ToString()
                    ))
                .ForMember(dest => dest.ReleaseDate,
                    opt =>
                        opt.MapFrom(x => $"{x.ReleaseDate:MMMM dd} {x.ReleaseDate:H:mm}"));
        }
    }
}
