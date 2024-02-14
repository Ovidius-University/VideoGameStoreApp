using AppUI.Validators;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
namespace AppUI.Models.Entities;
[Table("Video_Game")]
public class VideoGame
{
    [Column("VideoGameId")]
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int GenreId { get; set; }
    [ForeignKey(nameof(GenreId))]
    public Genre? Genre { get; set; }
    [Column(TypeName = "decimal(7,2)"), Price]
    public decimal Price { get; set; }
    public int PublisherId { get; set; }
    [ForeignKey(nameof(PublisherId))]
    public Publisher? Publisher { get; set; }
    [Column("PublishedOn"), Year]
    public short ReleaseYear { get; set; }
    [Stock]
    public int Stock {  get; set; }
    public bool IsFinal { get; set; } = false;
    //public Image Image { get; set; }
    public VideoGameOffer? Offer { get; set; }
    public ICollection<DeveloperVideoGame>? Developers { get; set; }
}