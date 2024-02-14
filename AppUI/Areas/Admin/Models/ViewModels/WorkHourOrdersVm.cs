using AppUI.Areas.Admin.Models.DTOs;

namespace AppUI.Areas.Admin.Models.ViewModels;

public class WorkHourOrdersVm
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string StartHour { get; set; } = string.Empty;
    public string EndHour { get; set; } = string.Empty;
    public string IsWorkDay { get; set; } = string.Empty;
    //public List<ExistentOrderDto>? listOrders { get; set; }
}