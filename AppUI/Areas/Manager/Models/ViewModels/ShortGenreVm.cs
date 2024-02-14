using System.ComponentModel.DataAnnotations;
namespace AppUI.Areas.Manager.Models.ViewModels;
public class ShortGenreVm
{
    public int GenreId { get; set; }
    [Display(Name = "Genre")]
    public string Name { get; set; }=string.Empty;
    //public ShortGenreVm(int id, string name)
    //{
    //    Id = id;
    //    Name = name ;
    //}
}
