using AutoMapper;
using EP.Application.Services.Cinema_Management.CinemaHall.DTO;
using EP.Infrastructure.Entities;

namespace EP.Application.Profiles
{
    public class CinemaHallProfile : Profile
    {
        public CinemaHallProfile()
        {
            CreateMap<CinemaHall, GetCinemaHallResponseDto>()
                .ForMember(dest => dest.hallName,
                    opt => opt.MapFrom(x => x.Name))
                .ForMember(dest => dest.totalSeat,
                    opt => opt.MapFrom(x => x.TotalSeats));
        }
    }
}
