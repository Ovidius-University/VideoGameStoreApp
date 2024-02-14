using AppUI.Models.CustomIdentity;
using AppUI.Areas.Admin.Models.DTOs;
using AppUI.Areas.Manager.Models.DTOs;
using AppUI.Areas.Manager.Models.ViewModels;
using AppUI.Areas.Critic.Models.DTOs;
using AppUI.Areas.Critic.Models.ViewModels;
using AppUI.Models.ViewModels;
using AppUI.Models.DTOs;
using AppUI.Models.Entities;
namespace AppUI.Profiles;
public class UserDataProfile : Profile
{
    public UserDataProfile()
    {
        CreateMap<UserData, UserDataVm>()
            .ForMember(d => d.User, s => s.MapFrom(src => src.User!.UserName))
            .ForMember(d => d.Gender, s => s.MapFrom(src => src.Gender!.Name));

        CreateMap<NewUserDataDto, UserData>();

        CreateMap<UserData, ExistentUserDataDto>()
            .ForMember(d => d.User, s => s.MapFrom(src => src.User!.UserName))
            .ForMember(d => d.Gender, s => s.MapFrom(src => src.Gender!.Name));
    }
}