using AppUI.Areas.Admin.Models.DTOs;

namespace AppUI.Areas.Admin.Models.ViewModels;

public class ReviewerCriticsVm
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<ExistentUserDto>? listCritics { get; set; }
}