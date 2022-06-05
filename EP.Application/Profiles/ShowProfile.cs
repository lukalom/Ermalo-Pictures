using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using EP.Application.Services.Show.DTO.Request;
using EP.Application.Services.Show.DTO.Response;
using EP.Infrastructure.Entities;

namespace EP.Application.Profiles
{
    public class ShowProfile : Profile
    {
        public ShowProfile()
        {
            CreateMap<Show, GetShowResponseDto>()
                .ForMember(dest => dest.Date,
                    opt => opt.MapFrom(x => $"{x.Date:MMMM dd} { x.Date:H:mm}"))
                .ForMember(dest => dest.StartTime,
                    opt => opt.MapFrom(x => $"{ x.StartTime:H:mm}"))
                .ForMember(dest => dest.EndTime,
                    opt => opt.MapFrom(x => $"{ x.EndTime:H:mm}"))
                .ForMember(dest => dest.Movie,
                    opt => opt.MapFrom(x => x.Movie.Title))
                .ForMember(dest => dest.HallName,
                    opt => opt.MapFrom(x => x.CinemaHall.Name));

        }
    }
}
