using AutoMapper;
using EP.Application.Services.ShowSeat.DTO;
using EP.Infrastructure.Entities;

namespace EP.Application.Profiles
{
    public class ShowSeatProfile : Profile
    {
        public ShowSeatProfile()
        {
            CreateMap<ShowSeat, GetShowSeatResponseDto>()
                .ForMember(dest => dest.CinemaHall,
                    opt => opt.MapFrom(x => x.Show.CinemaHall.Name))
                .ForMember(dest => dest.Price,
                    opt => opt.MapFrom(x => $"{x.Price}$"))
                .ForMember(dest => dest.SeatId,
                    opt => opt.MapFrom(x => x.CinemaSeatId))
                .ForMember(dest => dest.Seat_Type,
                    opt => opt.MapFrom(x => x.CinemaSeat.SeatType.ToString()))
                .ForMember(dest => dest.Movie,
                    opt => opt.MapFrom(x => x.Show.Movie.Title))
                .ForMember(dest => dest.RowNumber,
                    opt => opt.MapFrom(x => x.CinemaSeat.RowNumber))
                .ForMember(dest => dest.ColumnNumber,
                    opt => opt.MapFrom(x => x.CinemaSeat.ColumnNumber))
                .ForMember(dest => dest.SeatStatus,
                    opt => opt.MapFrom(x => x.Status));

        }
    }
}
