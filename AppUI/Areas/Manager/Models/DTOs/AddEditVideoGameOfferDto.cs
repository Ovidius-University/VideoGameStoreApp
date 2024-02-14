using System.ComponentModel.DataAnnotations;
using AppUI.Validators;
namespace AppUI.Areas.Manager.Models.DTOs;
public class AddEditVideoGameOfferDto
{
    public int Id { get; set; }
    //public required string Title { get; set; }
    //public required string Developers { get; set; }
    [Required, Display(Name ="Promo Text")]
    public required string PromoText { get; set; }
    [Display(Name = "Old Price"),Price]
    public decimal Price { get; set; }
    [Required, Display(Name = "New Price"), SmallerPrice(nameof(Price)), Price]
    public decimal NewPrice { get; set; }
    //public bool IsNewOffer { get; set; } = true;
}
