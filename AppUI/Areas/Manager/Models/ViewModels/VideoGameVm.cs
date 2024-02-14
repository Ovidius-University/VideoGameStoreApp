using System.ComponentModel.DataAnnotations;
using AppUI.Validators;
namespace AppUI.Areas.Manager.Models.ViewModels;
public class VideoGameVm
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    [Display(Name ="Release Year"),Year]
    public short ReleaseYear { get; set; }
    [Price]
    public decimal Price { get; set; }
    [Stock]
    public int Stock { get; set; }
    [Display(Name = "Is It Final?")]
    public bool IsFinal { get; set; } = false;
    public List<ShortDeveloperVm>? ListDevelopers { get; set; }
}
