using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AppUI.Models.Entities;
[Table("Review")]
[PrimaryKey(nameof(VideoGameId), nameof(ReviewerId))]
public class Review
{
    public int VideoGameId { get; set; }
    [ForeignKey(nameof(VideoGameId))]
    public VideoGame? VideoGame { get; set; }
    public int ReviewerId { get; set; }
    [ForeignKey(nameof(ReviewerId))]
    public Reviewer? Reviewer { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsFinal { get; set; } = false;
}