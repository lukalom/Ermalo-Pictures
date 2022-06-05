using AutoMapper;
using EP.Infrastructure.Entities;

namespace EP.Application.Profiles
{
    public class NbgCurrencyProfile : Profile
    {
        public NbgCurrencyProfile()
        {
            CreateMap<NbgCurrency, NbgCurrency>().ForMember(dest => dest.Id,
                opt => opt.Ignore());
        }
    }
}
