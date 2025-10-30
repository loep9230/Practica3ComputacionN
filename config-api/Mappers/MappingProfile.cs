using AutoMapper;
using config_api.Dtos;
using config_domain;

namespace config_api.Mappers
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<ParcialEntornoDto, Entornos>()
                .ForAllMembers(o => o.Condition((dto,e,f) => f != null));
            CreateMap<ParcialVariableDto, Variables>()
                .ForAllMembers(o => o.Condition((dto, e, f) => f != null));
        }
    }
}
