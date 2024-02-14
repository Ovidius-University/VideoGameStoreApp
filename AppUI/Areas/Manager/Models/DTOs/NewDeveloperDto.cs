using System.ComponentModel.DataAnnotations;
using AppUI.Validators;
namespace AppUI.Areas.Manager.Models.DTOs;
public class NewDeveloperDto
{
    [Display(Name = "First Name"), MaxLength(50,ErrorMessage ="{0} needs to be at max {1} characters!")]
    public string FirstName { get; set; } = string.Empty;
    [Display(Name = "Last Name"), MaxLength(50, ErrorMessage = "{0} needs to be at max {1} characters!")]
    public string LastName { get; set; } = string.Empty;
    [Display(Name ="Birth Date"),Birthday,DataType(DataType.Date),DisplayFormat(ApplyFormatInEditMode =true, DataFormatString = "{0:dd-MM-yyyy}")]
    public DateTime Birthday { get; set; }
}
