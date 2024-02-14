using AppUI.Models.CustomIdentity;
using AppUI.Areas.Admin.Models.DTOs;
using AppUI.Areas.Manager.Models.DTOs;
using AppUI.Areas.Manager.Models.ViewModels;

namespace AppUI.Profiles
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<AppRole, NewRoleDto>()
                .ForMember(d => d.Name, s => s.MapFrom(src => src.Name));

            CreateMap<AppRole, ExistentRole>()
                .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
                .ForMember(d => d.Name, s => s.MapFrom(src => src.Name));
        }
    }
}