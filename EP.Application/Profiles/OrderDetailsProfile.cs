using AutoMapper;
using EP.Application.Services.OrderDetail.DTO;
using EP.Infrastructure.Entities;

namespace EP.Application.Profiles
{
    public class OrderDetailsProfile : Profile
    {
        public OrderDetailsProfile()
        {

            CreateMap<OrderDetails, GetUserApprovedOrderResponseDto>()
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(x => x.Status))
                .ForMember(dest => dest.Movie,
                    opt =>
                        opt.MapFrom(x => x.Show.Movie.Title))
                .ForMember(dest => dest.Seat,
                    opt =>
                        opt.MapFrom(x => $"Row:{x.ShowSeat.CinemaSeat.RowNumber} | Col:{x.ShowSeat.CinemaSeat.ColumnNumber}"))
                .ForMember(dest => dest.CreatedDate,
                    opt =>
                        opt.MapFrom(x => x.CreatedAt.ToString("F")))
                .ForMember(dest => dest.TotalPrice,
                    opt =>
                        opt.MapFrom(x => x.TotalPrice))
                .ForMember(dest => dest.TicketPrice,
                    opt =>
                        opt.MapFrom(x => x.ShowSeat.Price));
        }
    }
}
