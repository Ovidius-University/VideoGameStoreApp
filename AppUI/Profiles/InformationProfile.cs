using AppUI.Areas.Admin.Models.DTOs;
using AppUI.Areas.Admin.Models.ViewModels;
using AppUI.Models.DTOs;
using AppUI.Models.ViewModels;
namespace AppUI.Profiles;
public class InformationProfile : Profile
{
    public InformationProfile()
    {
        CreateMap<Areas.Admin.Models.DTOs.ExistentInformationDto, Information>();
        CreateMap<Information, Areas.Admin.Models.DTOs.ExistentInformationDto>();
        CreateMap<Information, Models.DTOs.ExistentInformationDto>();
    }
}