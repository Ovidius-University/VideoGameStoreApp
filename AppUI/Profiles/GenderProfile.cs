using AppUI.Areas.Admin.Models.DTOs;
using AppUI.Areas.Admin.Models.ViewModels;
using AppUI.Areas.Cashier.Models.DTOs;
using AppUI.Areas.Cashier.Models.ViewModels;
using AppUI.Models.DTOs;
using AppUI.Models.ViewModels;
namespace AppUI.Profiles;
public class GenderProfile : Profile
{
    public GenderProfile()
    {
        CreateMap<NewGenderDto, Gender>();
        CreateMap<Gender, ExistentGenderDto>();

        CreateMap<Gender, Models.DTOs.GenderDto>()
            .ForMember(d => d.GenderId, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Name, s => s.MapFrom(src => $"{src.Name}"));

        CreateMap<Gender, Areas.Admin.Models.DTOs.GenderDto>()
            .ForMember(d => d.GenderId, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Name, s => s.MapFrom(src => $"{src.Name}"));
    }
}