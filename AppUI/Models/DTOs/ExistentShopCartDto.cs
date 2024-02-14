using System.ComponentModel.DataAnnotations;
using AppUI.Validators;
namespace AppUI.Models.DTOs;

public class ExistentShopCartDto
{

    public int VideoGameId { get; set; }
    public string VideoGame { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public string Customer { get; set; } = string.Empty;
    [Quantity]
    public required int Quantity { get; set; }
    public void ToEntity(ref ShopCart ExistentShopCart)
    {
        if (ExistentShopCart.VideoGameId == VideoGameId && ExistentShopCart.CustomerId == CustomerId)
        {
            ExistentShopCart.Quantity = Quantity;
        }
    }
}
