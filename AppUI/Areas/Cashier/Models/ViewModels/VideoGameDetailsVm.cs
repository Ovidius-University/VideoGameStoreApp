using System.ComponentModel.DataAnnotations;
namespace AppUI.Areas.Cashier.Models.ViewModels;
public class VideoGameDetailsVm
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    [Display(Name = "Release Year")]
    public short ReleaseYear { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    [Display(Name = "Is It Final?")]
    public bool IsFinal { get; set; }
    public required string Developers { get; set; }
    public required string Genre { get; set; }
}
