using System.ComponentModel.DataAnnotations;
namespace AppUI.Areas.Critic.Models.ViewModels;
public class ShortReviewerVm
{
    public int ReviewerId { get; set; }
    [Display(Name = "Reviewer")]
    public string Name { get; set; }=string.Empty;
    //public ShortDeveloperVm(int id, string firstname, string lastname)
    //{
    //    Id = id;
    //    FullName = $"{lastname} {firstname}" ;
    //}
}
