using System.ComponentModel.DataAnnotations;
using AppUI.Validators;
namespace AppUI.Areas.Manager.Models.DTOs;

public class NewVideoGameDto
{
    [Required, MaxLength(400)]
    public required string Title { get; set; }
    [Required, MaxLength(400)]
    public required string Description { get; set; }
    [Required, Display(Name = "Genre")]
    public required int GenreId { get; set; }
    [Required]
    [Display(Name = "Release year"), Year]
    public short ReleaseYear { get; set; }
    [DisplayFormat(DataFormatString = "{0:###0.00}")]
    [Display(Name = "Price"), Price]
    public decimal Price { get; set; }
    [Display(Name = "Stock"), Stock]
    public int Stock { get; set; }

}
