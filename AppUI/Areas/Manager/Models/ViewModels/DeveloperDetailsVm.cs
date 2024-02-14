using System.ComponentModel.DataAnnotations;
namespace AppUI.Areas.Manager.Models.ViewModels;
public class DeveloperDetailsVm
{
    public int Id { get; set; }
    [Display(Name = "Developer")]
    public string FullName{ get; set; }=string.Empty;
    [Display(Name ="Birthday"),DisplayFormat(DataFormatString ="{0:d MMMM yyyy}")]
    public DateTime Birthday { get; set; }
}
