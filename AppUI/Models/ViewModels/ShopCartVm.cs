using System.ComponentModel.DataAnnotations;
using AppUI.Validators;
namespace AppUI.Models.ViewModels;
public class ShopCartVm
{
    public int CustomerId { get; set; }
    public int VideoGameId { get; set; }
    [Display(Name = "Unit Price"), Price]
    public decimal UnitPrice { get; set; }
    [Quantity]
    public int Quantity { get; set; }
    [Display(Name = "Total Price"), Price]
    public decimal TotalPrice { get; set; }
    public ShortVideoGameVm? VideoGame { get; set; }
}
