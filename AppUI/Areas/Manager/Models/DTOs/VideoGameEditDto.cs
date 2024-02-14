using AppUI.Areas.Manager.Models.ViewModels;

namespace AppUI.Areas.Manager.Models.DTOs;
public class VideoGameEditDto
{
    public ExistentVideoGameDto? ExistentVideoGame { get; set; }
    public List<ShortDeveloperVm>? ListDevelopers { get; set; }
}
