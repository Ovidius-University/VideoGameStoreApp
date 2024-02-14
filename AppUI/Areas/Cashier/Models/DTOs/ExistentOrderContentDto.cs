using System.ComponentModel.DataAnnotations;
using AppUI.Validators;
namespace AppUI.Areas.Cashier.Models.DTOs;

public class ExistentOrderContentDto
{
    public int OrderId { get; set; }
    public int VideoGameId { get; set; }
    [Display(Name = "Video Game")]
    public string VideoGame { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public string Customer { get; set; } = string.Empty;
    [Quantity]
    public required int Quantity { get; set; }
    [Price]
    [Display(Name = "Unit Price")]
    public required decimal UnitPrice { get; set; }
    public void ToEntity(ref OrderContent ExistentOrderContent)
    {
        if (ExistentOrderContent.VideoGameId == VideoGameId && ExistentOrderContent.OrderId == OrderId)
        {
            ExistentOrderContent.Quantity = Quantity;
            ExistentOrderContent.UnitPrice = UnitPrice;
        }
    }
}
