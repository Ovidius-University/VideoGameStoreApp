using System.ComponentModel.DataAnnotations;
using AppUI.Areas.Manager.Models.ViewModels;
using AppUI.Validators;
namespace AppUI.Areas.Manager.Models.DTOs;

public class ExistentVideoGameDto
{
    public int Id { get; set; }
    [Required, MaxLength(400)]
    public required string Title { get; set; }
    [Required, MaxLength(400)]
    public required string Description { get; set; }
    [Required, Display(Name = "Genre")]
    public required int GenreId { get; set; }
    [Required]
    [Display(Name = "Release year"), Year]
    public short ReleaseYear { get; set; }
    [DisplayFormat(DataFormatString = "{0:##0.00}")]
    [Required, Display(Name = "Price"), Price]
    public decimal Price { get; set; }
    [Required, Display(Name = "Stock (Setting it to 0 will remove it from everyone's shopping cart)"), Stock]
    public int Stock { get; set; }
    [Display(Name = "Is It Final? (Making it not final will remove it from everyone's shopping cart)")]
    public bool IsFinal { get; set; }
    public void ToEntity(ref VideoGame ExistentVideoGame)
    {
        if (ExistentVideoGame.Id == Id)
        {
            ExistentVideoGame.Title = Title;
            ExistentVideoGame.Description = Description;
            ExistentVideoGame.GenreId = GenreId;
            ExistentVideoGame.Price = Price;
            ExistentVideoGame.ReleaseYear = ReleaseYear;
            ExistentVideoGame.Stock = Stock;
            ExistentVideoGame.IsFinal = IsFinal;
        }
    }
}

