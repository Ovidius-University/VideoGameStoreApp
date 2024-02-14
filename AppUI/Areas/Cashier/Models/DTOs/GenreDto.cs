using System.ComponentModel.DataAnnotations;

namespace AppUI.Areas.Cashier.Models.DTOs;
public class GenreDto
{
    [Display(Name = "Genre")]
    public required int GenreId { get; set; }
    public required string Name { get; set; }
}
