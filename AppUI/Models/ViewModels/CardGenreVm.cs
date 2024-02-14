using System.ComponentModel.DataAnnotations;

namespace AppUI.Models.ViewModels;
public class CardGenreVm
{
    [Key]
    public int GenreId { get; set; }
    public string Name { get; set; } = string.Empty;
    // public CardGenreVm(Genre Genre)
    // {
    //     GenreId = Genre.Id;
    //     Name = Genre.Name;
    // }
}