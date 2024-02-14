using System.ComponentModel.DataAnnotations;
using AppUI.Areas.Manager.Models.ViewModels;
namespace AppUI.Areas.Manager.Models.DTOs;
public class DeveloperAddVideoGameDto
{
    public int DeveloperId { get; set; }
    public string Developer { get; set; } = string.Empty;
    [Display(Name ="Pick a Video Game:")]
    public int VideoGameId { get; set; }
}
