using System.ComponentModel.DataAnnotations;
using AppUI.Validators;
namespace AppUI.Areas.Delivery.Models.DTOs;

public class NewOrderContentDto
{
    public required int OrderId { get; set; }
    [Display(Name = "Pick a video game")]
    public required int VideoGameId { get; set; }
    [Price]
    [Display(Name = "Unit Price")]
    public decimal UnitPrice { get; set; }
    [Quantity]
    public required int Quantity { get; set; }
}
