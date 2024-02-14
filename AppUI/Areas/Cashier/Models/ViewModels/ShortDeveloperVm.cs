using System.ComponentModel.DataAnnotations;
namespace AppUI.Areas.Cashier.Models.ViewModels;
public class ShortDeveloperVm
{
    public int DeveloperId { get; set; }
    [Display(Name = "Developers")]
    public string FullName { get; set; }=string.Empty;
    //public ShortDeveloperVm(int id, string firstname, string lastname)
    //{
    //    Id = id;
    //    FullName = $"{lastname} {firstname}" ;
    //}
}
