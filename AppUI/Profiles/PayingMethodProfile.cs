using AppUI.Areas.Admin.Models.DTOs;
using AppUI.Areas.Admin.Models.ViewModels;
using AppUI.Areas.Cashier.Models.DTOs;
using AppUI.Areas.Cashier.Models.ViewModels;
using AppUI.Models.DTOs;
using AppUI.Models.ViewModels;
namespace AppUI.Profiles;
public class PayingMethodProfile : Profile
{
    public PayingMethodProfile()
    {
        CreateMap<NewPayingMethodDto, PayingMethod>();
        CreateMap<PayingMethod, ExistentPayingMethodDto>();

        CreateMap<PayingMethod, Models.DTOs.PayingMethodDto>()
            .ForMember(d => d.PayingMethodId, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Name, s => s.MapFrom(src => $"{src.Name}"));

        CreateMap<PayingMethod, Areas.Admin.Models.DTOs.PayingMethodDto>()
            .ForMember(d => d.PayingMethodId, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Name, s => s.MapFrom(src => $"{src.Name}"));

        CreateMap<PayingMethod, Areas.Cashier.Models.DTOs.PayingMethodDto>()
            .ForMember(d => d.PayingMethodId, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Name, s => s.MapFrom(src => $"{src.Name}"));
    }
}