using System.ComponentModel.DataAnnotations;

namespace AppUI.Areas.Manager.Models.DTOs;
public class NewGenreDto
{
    [MaxLength(50)]
    public string Name { get; set; }=string.Empty;
}
