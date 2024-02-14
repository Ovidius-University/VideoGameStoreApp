using System.ComponentModel.DataAnnotations;
using AppUI.Validators;
namespace AppUI.Areas.Delivery.Models.DTOs;
public class OrderFeedBackDto
{
    public int Id { get; set; }
    [Display(Name = "Total Cost"), Price]
    public decimal Cost { get; set; }
    [MaxLength(100)]
    public required string Comment { get; set; }
    public required string Customer { get; set; }
    public bool IsNewFeedBack { get; set; } = true;
    public List<ExistentOrderContentDto>? VideoGames { get; set; }
}
