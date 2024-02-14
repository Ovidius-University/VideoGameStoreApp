namespace AppUI.Models.ViewModels;
public class VideoGameDetailsVm
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int GenreId { get; set; }
    public string Genre { get; set; } = string.Empty;
    public int PublisherId { get; set; }
    public string Publisher { get; set; } = string.Empty;
    public List<CardDeveloperVm>? ListDevelopers { get; set; }
    public string Developers { get; set; } = string.Empty;
    public List<CardReviewerVm>? ListReviewers { get; set; }
    public string Reviewers { get; set; } = string.Empty;
    public short ReleaseYear { get; set; }
    public decimal Price { get; set; }
    public decimal NewPrice { get; set; }
    public int Stock { get; set; }
    public string PromoText { get; set; } = string.Empty;
}