using AppUI.Models.DTOs;

namespace AppUI.Models.ViewModels;

public class StoreInfoVm
{
    public ExistentInformationDto? Information { get; set; }
    public List<ExistentWorkHourDto>? WorkHours { get; set; }
}
