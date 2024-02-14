using System.ComponentModel.DataAnnotations;
namespace AppUI.Areas.Critic.Models.ViewModels;
public class ReviewDetailsVm
{
    public int VideoGameId { get; set; }
    public int ReviewerId { get; set; }
    public string Content { get; set; } = string.Empty;
    [Display(Name = "Is It Final?")]
    public bool IsFinal { get; set; }
    public required string Reviewer { get; set; }
    public required string VideoGame { get; set; }
}
