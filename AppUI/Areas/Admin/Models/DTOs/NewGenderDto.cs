using System.ComponentModel.DataAnnotations;

namespace AppUI.Areas.Admin.Models.DTOs;
public class NewGenderDto
{
    [MaxLength(50)]
    public string Name { get; set; }=string.Empty;
}
