using System.ComponentModel.DataAnnotations;

namespace AppUI.Models.ViewModels;
public class CardDeveloperVm
{
    [Key]
    public int DeveloperId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    [DisplayFormat(DataFormatString = "{0:yyyy}")]
    public DateTime Birthday { get; set; }
    // public CardDeveloperVm(Developer Developer)
    // {
    //     DeveloperId = Developer.Id;
    //     LastName = Developer.LastName;
    //     FirstName = Developer.FirstName;
    //     Birthday = Developer.Birthday;
    // }
}