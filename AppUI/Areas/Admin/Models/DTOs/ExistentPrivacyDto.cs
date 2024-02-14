using AppUI.Models.Entities;
using AppUI.Validators;
using Microsoft.CodeAnalysis;
using System.ComponentModel.DataAnnotations;

namespace AppUI.Areas.Admin.Models.DTOs;

public class ExistentPrivacyDto
{
    public int Id { get; set; }
    [Required]
    public string Policy { get; set; } = string.Empty;
    public void ToEntity(ref Privacy ExistentPrivacy)
    {
        ExistentPrivacy.Policy = Policy;
    }
}
