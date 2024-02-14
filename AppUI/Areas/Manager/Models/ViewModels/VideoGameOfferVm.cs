﻿using System.ComponentModel.DataAnnotations;

namespace AppUI.Areas.Manager.Models.ViewModels;
public class VideoGameOfferVm
{
    public int Id { get; set; }
    public required string Title { get; set; }
    [Display(Name ="Promo Text")]
    public required string PromoText { get; set; }
    public decimal Price { get; set; }
    [Display(Name ="New Price")]
    public decimal NewPrice { get; set; }
}
