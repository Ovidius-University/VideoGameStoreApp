using System.ComponentModel.DataAnnotations;

namespace AppUI.Areas.Manager.Models.DTOs;

public class ExistentGenreDto
{
    [Display(Name = "Genre")]
    public int Id { get; set; }
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
}
