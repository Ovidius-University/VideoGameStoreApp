using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AppUI.Models.Entities;
[Table("Video_Game_Developer")]
[PrimaryKey(nameof(VideoGameId), nameof(DeveloperId))]
public class DeveloperVideoGame
{
    public int VideoGameId { get; set; }
    [ForeignKey(nameof(VideoGameId))]
    public VideoGame? VideoGame { get; set; }
    public int DeveloperId { get; set; }
    [ForeignKey(nameof(DeveloperId))]
    public Developer? Developer { get; set; }
}