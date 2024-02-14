using AppUI.Areas.Admin.Models.DTOs;
using AppUI.Areas.Admin.Models.ViewModels;
using AppUI.Models.DTOs;
using AppUI.Models.ViewModels;
namespace AppUI.Profiles;
public class PrivacyProfile : Profile
{
    public PrivacyProfile()
    {
        CreateMap<Areas.Admin.Models.DTOs.ExistentPrivacyDto, Privacy>();
        CreateMap<Privacy, Areas.Admin.Models.DTOs.ExistentPrivacyDto>();
        CreateMap<Privacy, Models.DTOs.ExistentPrivacyDto>();
    }
}