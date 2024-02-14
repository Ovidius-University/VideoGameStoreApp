using System.ComponentModel.DataAnnotations.Schema;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace AppUI.Models.Entities;
[Table("Video_Game_Offer")]
public class VideoGameOffer
{
    [Column("VideoGameOfferId")]
    public int Id { get; set; }
    [ForeignKey(nameof(Id))]
    public VideoGame? VideoGame { get; set; }
    public string PromoText { get; set; } = string.Empty;
    [Column(TypeName = "decimal(7,2)")]
    public decimal NewPrice { get; set; }
}