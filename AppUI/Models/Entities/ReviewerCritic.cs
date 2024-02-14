using AppUI.Models.CustomIdentity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppUI.Models.Entities;
[Table("Reviewer_Critic")]
[PrimaryKey(nameof(CriticId), nameof(ReviewerId))]
public class ReviewerCritic
{
    [Column("UserId")]
    public int CriticId { get; set; }
    public int ReviewerId { get; set; }
    [ForeignKey(nameof(CriticId))]
    public AppUser? Critic { get; set; }
    [ForeignKey(nameof(ReviewerId))]
    public Reviewer? Reviewer { get; set; }
}