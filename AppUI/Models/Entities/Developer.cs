using System.ComponentModel.DataAnnotations.Schema;

namespace AppUI.Models.Entities;

[Table("Developer")]
public class Developer
{
    [Column("Developer_Id")]
    public int Id { get; set; }

    [Column("FirstName")]
    public string FirstName { get; set; }=string.Empty;

    [Column("LastName")]
    public string LastName { get; set; } = string.Empty;

    public DateTime Birthday { get; set; }

    public ICollection<DeveloperVideoGame>? VideoGames { get; set; }
}
