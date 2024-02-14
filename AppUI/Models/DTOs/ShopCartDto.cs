using AppUI.Validators;
using System.ComponentModel.DataAnnotations;

namespace AppUI.Models.DTOs;
public class ShopCartDto
{
    public int VideoGameId { get; set; }
    public int CustomerId { get; set; }
    [Quantity]
    public required int Quantity { get; set; }
}
