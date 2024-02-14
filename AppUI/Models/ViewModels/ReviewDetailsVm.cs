namespace AppUI.Models.ViewModels;
public class ReviewDetailsVm
{
    public int VideoGameId { get; set; }
    public string VideoGame { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int ReviewerId { get; set; }
    public string Reviewer { get; set; } = string.Empty;
}