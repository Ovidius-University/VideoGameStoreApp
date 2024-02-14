using System.ComponentModel.DataAnnotations;

namespace AppUI.Areas.Critic.Models.DTOs;
public class ReviewDto
{
    public int VideoGameId { get; set; }
    public int ReviewerId { get; set; }
    public required string Content { get; set; }
}
