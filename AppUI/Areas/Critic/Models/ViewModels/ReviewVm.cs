using System.ComponentModel.DataAnnotations;
using AppUI.Validators;
namespace AppUI.Areas.Critic.Models.ViewModels;
public class ReviewVm
{
    public int ReviewerId { get; set; }
    public int VideoGameId { get; set; }
    public string Content { get; set; } = string.Empty;
    [Display(Name = "Is It Final?")]
    public bool IsFinal { get; set; } = false;
    public ShortReviewerVm? Reviewer { get; set; }
    public ShortVideoGameVm? VideoGame { get; set; }
}
