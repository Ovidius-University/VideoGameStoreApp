namespace AppUI.Models.ViewModels;
public class CardReviewVm
{
    public int VideoGameId { get; set; }
    public int ReviewerId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string VideoGame { get; set; } = string.Empty;
    public string Reviewer { get; set; } = string.Empty;
    // public CardReviewVm()
    // {
    // }
    // public CardReviewVm(Review Review)
    // {
    //     if(Review is not null)
    //     {
    //         Id=Review.VideoGameId;
    //         Content=Review.Title;
    //         Reviewer=string.Join(", ",Review.Reviewer!.Select(a=>$"{a.Reviewer!.Name}"));
    //     }
    // }
}