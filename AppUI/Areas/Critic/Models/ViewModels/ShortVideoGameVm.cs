using System.ComponentModel.DataAnnotations;
using AppUI.Validators;
namespace AppUI.Areas.Critic.Models.ViewModels;
public class ShortVideoGameVm
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;    
}
