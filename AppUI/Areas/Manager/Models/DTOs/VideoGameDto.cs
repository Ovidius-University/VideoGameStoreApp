using System.ComponentModel.DataAnnotations;

namespace AppUI.Areas.Manager.Models.DTOs;
public class VideoGameDto
{
    public int VideoGameId { get; set; }
    public required string Title { get; set; }
}
