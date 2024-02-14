using System.ComponentModel.DataAnnotations;

namespace AppUI.Areas.Admin.Models.DTOs;
public class VideoGameDto
{
    public int VideoGameId { get; set; }
    public required string Title { get; set; }
    public required int Stock { get; set; }
}
