using System.ComponentModel.DataAnnotations;
using AppUI.Models.Entities;
using AppUI.Validators;
namespace AppUI.Areas.Manager.Models.DTOs;
public class ExistentDeveloperDto
{
    public int DeveloperId { get; set; }
    [MaxLength(50),Required, Display(Name = "First Name:")]
    public string FirstName { get; set; } = string.Empty;
    [MaxLength(50),Required, Display(Name = "Last Name:")]
    public string LastName { get; set; } = string.Empty;
    [Required,Birthday,Display(Name = "Birth Date:"),DataType(DataType.Date), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
    public DateTime Birthday { get; set; }

    public void ToEntity(ref Developer ExistentDeveloper)
    {
        if(ExistentDeveloper.Id == DeveloperId)
        {
            ExistentDeveloper.FirstName = FirstName;
            ExistentDeveloper.LastName = LastName;
            ExistentDeveloper.Birthday = Birthday;
        }
    }
}
