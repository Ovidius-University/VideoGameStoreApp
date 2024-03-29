﻿using AppUI.Validators;
using System.ComponentModel.DataAnnotations;

namespace AppUI.Models.ViewModels;
public class ShopCartDetailsVm
{
    public int VideoGameId { get; set; }
    public int CustomerId { get; set; }
    [Display(Name = "Unit Price")]
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    [Display(Name = "Total Price")]
    public decimal TotalPrice { get; set; }
    public required string Customer { get; set; }
    public required string VideoGame { get; set; }
}
