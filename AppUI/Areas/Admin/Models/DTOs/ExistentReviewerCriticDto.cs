using System.ComponentModel.DataAnnotations;
namespace AppUI.Areas.Admin.Models.DTOs;
public class ExistentReviewerCriticDto
{
    public int CriticId { get; set; }
    public int ReviewerId { get; set; }
}