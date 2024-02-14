using System.ComponentModel.DataAnnotations;

namespace AppUI.Areas.Cashier.Models.DTOs;
public class VideoGameDto
{
    public int VideoGameId { get; set; }
    public required string Title { get; set; }
    public required int Stock { get; set; }
}
