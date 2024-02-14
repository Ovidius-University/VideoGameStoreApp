using System.ComponentModel.DataAnnotations;

namespace AppUI.Areas.Admin.Models.DTOs;
public class NewReviewerDto
{
    [MaxLength(50)]
    public string Name { get; set; }=string.Empty;
}
