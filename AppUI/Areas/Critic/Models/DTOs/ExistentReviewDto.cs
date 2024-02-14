using System.ComponentModel.DataAnnotations;
using AppUI.Validators;
namespace AppUI.Areas.Critic.Models.DTOs;

public class ExistentReviewDto
{

    public int VideoGameId { get; set; }
    public string VideoGame {  get; set; } = string.Empty;
    public int ReviewerId { get; set; }
    public string Reviewer { get; set; } = string.Empty;
    [Required, MaxLength(100)]
    public required string Content { get; set; }
    [Display(Name = "Is It Final?")]
    public bool IsFinal { get; set; }    
    public void ToEntity(ref Review ExistentReview)
    {
        if (ExistentReview.VideoGameId == VideoGameId && ExistentReview.ReviewerId == ReviewerId)
        {
            ExistentReview.Content = Content;
            ExistentReview.IsFinal = IsFinal;
        }
    }
}
