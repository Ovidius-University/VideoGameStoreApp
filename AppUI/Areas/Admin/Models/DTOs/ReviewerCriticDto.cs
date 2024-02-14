using System.ComponentModel.DataAnnotations;
namespace AppUI.Areas.Admin.Models.DTOs;

public class ReviewerCriticDto
{
    [Key]
    public int ReviewerId { get; set; }
    public string Reviewer { get; set; } = string.Empty;
    [Display(Name ="Critic")]
    public int CriticId { get; set; }
}
