using System.ComponentModel.DataAnnotations;
using AppUI.Validators;
namespace AppUI.Areas.Delivery.Models.ViewModels;
public class ShortVideoGameVm
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Stock {  get; set; }
    public bool IsFinal { get; set; }
}
