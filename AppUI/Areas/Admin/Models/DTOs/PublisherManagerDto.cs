using System.ComponentModel.DataAnnotations;
namespace AppUI.Areas.Admin.Models.DTOs;

public class PublisherManagerDto
{
    [Key]
    public int PublisherId { get; set; }
    public string Publisher { get; set; } = string.Empty;
    [Display(Name ="Manager")]
    public int ManagerId { get; set; }
}
