using AppUI.Models.CustomIdentity;
using AppUI.Areas.Admin.Models.DTOs;
namespace AppUI.Profiles;
public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<AppUser, ExistentUserDto>()
            .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Email, s => s.MapFrom(src => src.UserName));
    }
}