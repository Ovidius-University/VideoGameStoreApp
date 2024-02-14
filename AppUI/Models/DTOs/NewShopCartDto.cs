using System.ComponentModel.DataAnnotations;
using AppUI.Validators;
namespace AppUI.Models.DTOs;

public class NewShopCartDto
{
    [Display(Name = "Pick a Video Game:")]
    public required int VideoGameId { get; set; }
    public required int CustomerId { get; set; }
    [Quantity]
    public required int Quantity { get; set; }
}
