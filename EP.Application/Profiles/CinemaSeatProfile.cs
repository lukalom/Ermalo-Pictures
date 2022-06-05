using AutoMapper;
using EP.Application.Services.Cinema_Management.CinemaSeat.DTO;
using EP.Infrastructure.Entities;

namespace EP.Application.Profiles
{
    public class CinemaSeatProfile : Profile
    {
        public CinemaSeatProfile()
        {
            CreateMap<CinemaSeat, GetSeatResponseDto>()
            .ForMember(dest => dest.Seat, opt =>
                opt.MapFrom(x => $"row:{x.RowNumber} | column:{x.ColumnNumber}"))
            .ForMember(dest => dest.Type, opt =>
                opt.MapFrom(x => x.SeatType))
            .ForMember(dest => dest.Hall, opt =>
                opt.MapFrom(x => x.CinemaHall.Name));
        }
    }
}
