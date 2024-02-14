using System.ComponentModel.DataAnnotations;

namespace AppUI.Areas.Admin.Models.DTOs;

public class ExistentGenderDto
{
    public int Id { get; set; }
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    //public List<ExistentOrderDto>? Orders { get; set; }
}
