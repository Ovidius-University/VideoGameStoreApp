using AppUI.Areas.Cashier.Models.ViewModels;

namespace AppUI.Areas.Cashier.Models.DTOs;
public class VideoGameEditDto
{
    public ExistentVideoGameDto? ExistentVideoGame { get; set; }
    public List<ShortDeveloperVm>? ListDevelopers { get; set; }
}
