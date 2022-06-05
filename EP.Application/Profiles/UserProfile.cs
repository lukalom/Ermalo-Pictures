using AutoMapper;
using EP.Application.Services.Account.Profile.DTO.Response;
using EP.Infrastructure.Entities;

namespace EP.Application.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<ApplicationUser, GetProfileResponseDto>()
                .ForMember(dest => dest.Email,
                    from => from.MapFrom(x => x.Email))
                .ForMember(dest => dest.UserName,
                    from => from.MapFrom(x => x.UserName))
                .ForMember(dest => dest.Phone,
                    from => from.MapFrom(x => x.PhoneNumber));
        }
    }
}
