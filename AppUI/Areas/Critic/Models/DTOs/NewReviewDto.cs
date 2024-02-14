using System.ComponentModel.DataAnnotations;
using AppUI.Validators;
namespace AppUI.Areas.Critic.Models.DTOs;

public class NewReviewDto
{
    [Display(Name = "Pick a Video Game:")]
    public required int VideoGameId { get; set; }
    public required int ReviewerId { get; set; }

    [Required, MaxLength(100)]
    public required string Content { get; set; }
}
