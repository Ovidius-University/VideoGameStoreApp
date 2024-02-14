using AppUI.Areas.Admin.Models.DTOs;
using AppUI.Areas.Admin.Models.ViewModels;
using AppUI.Models.DTOs;
using AppUI.Models.ViewModels;
namespace AppUI.Profiles;
public class WorkHourProfile : Profile
{
    public WorkHourProfile()
    {
        CreateMap<Areas.Admin.Models.DTOs.ExistentWorkHourDto, WorkHour>();
        CreateMap<WorkHour, Areas.Admin.Models.DTOs.ExistentWorkHourDto>()
            .ForMember(d => d.StartHour, s => s.MapFrom(src => src.StartHour))
            .ForMember(d => d.EndHour, s => s.MapFrom(src => src.EndHour));
        CreateMap<WorkHour, Models.DTOs.ExistentWorkHourDto>()
            .ForMember(d => d.StartHour, s => s.MapFrom(src => src.StartHour))
            .ForMember(d => d.EndHour, s => s.MapFrom(src => src.EndHour));
    }
}