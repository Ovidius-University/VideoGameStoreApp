using AppUI.Areas.Manager.Models.DTOs;
using AppUI.Areas.Manager.Models.ViewModels;
using AppUI.Areas.Cashier.Models.DTOs;
using AppUI.Areas.Cashier.Models.ViewModels;
using AppUI.Models.ViewModels;
namespace AppUI.Profiles;
public class DeveloperProfile : Profile
{
    public DeveloperProfile()
    {
        //CreateMap<NewDeveloperDto, Developer>();

        CreateMap<NewDeveloperDto, Developer>()
            .ForMember(d => d.LastName, s => s.MapFrom(src => src.LastName))
            .ForMember(d => d.FirstName, s => s.MapFrom(src => src.FirstName))
            .ForMember(d => d.Birthday, s => s.MapFrom(src => src.Birthday));

        CreateMap<Developer, ExistentDeveloperDto>()
            .ForMember(d => d.DeveloperId, s => s.MapFrom(src => src.Id));

        CreateMap<Developer, Areas.Manager.Models.ViewModels.ShortDeveloperVm>()
            .ForMember(d => d.DeveloperId, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.FullName, s => s.MapFrom(src => $"{src.FirstName} {src.LastName} ({src.Birthday.Year})"));

        CreateMap<DeveloperVideoGame, Areas.Manager.Models.ViewModels.ShortDeveloperVm>()
            .ForMember(d => d.DeveloperId, s => s.MapFrom(src => src.Developer!.Id))
            .ForMember(d => d.FullName, s => s.MapFrom(src => $"{src.Developer!.FirstName} {src.Developer!.LastName} ({src.Developer!.Birthday.Year})"));

        CreateMap<Developer, Areas.Cashier.Models.ViewModels.ShortDeveloperVm>()
            .ForMember(d => d.DeveloperId, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.FullName, s => s.MapFrom(src => $"{src.FirstName} {src.LastName} ({src.Birthday.Year})"));

        CreateMap<DeveloperVideoGame, Areas.Cashier.Models.ViewModels.ShortDeveloperVm>()
            .ForMember(d => d.DeveloperId, s => s.MapFrom(src => src.Developer!.Id))
            .ForMember(d => d.FullName, s => s.MapFrom(src => $"{src.Developer!.FirstName} {src.Developer!.LastName} ({src.Developer!.Birthday.Year})"));

        CreateMap<Developer, DeveloperDetailsVm>()
            .ForMember(d => d.FullName, s => s.MapFrom(src => $"{src.FirstName} {src.LastName}"));

        CreateMap<Developer, DeveloperAddVideoGameDto>()
            .ForMember(d => d.DeveloperId, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Developer, s => s.MapFrom(src => $"{src.FirstName} {src.LastName} ({src.Birthday.Year})"));

        CreateMap<Developer, CardDeveloperVm>()
            .ForMember(d => d.DeveloperId, s => s.MapFrom(src => src.Id));
    }
}