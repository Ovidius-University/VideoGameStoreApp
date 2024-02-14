namespace AppUI.Models.ViewModels;
public class CardVideoGameVm
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string PromoText { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public string Developers { get; set; } = string.Empty;
    public short ReleaseYear { get; set; }
    public decimal Price { get; set; }
    public decimal NewPrice { get; set; }
    public int Stock { get; set; }
    // public CardVideoGameVm()
    // {
    // }
    // public CardVideoGameVm(VideoGame VideoGame)
    // {
    //     if(VideoGame is not null)
    //     {
    //         Id=VideoGame.Id;
    //         Title=VideoGame.Title;
    //         ReleaseYear=VideoGame.PublishedOn;
    //         Price=VideoGame.Price;
    //         NewPrice=VideoGame.Offer?.NewPrice??0;
    //         Developers=string.Join(", ",VideoGame.Developers!.Select(a=>$"{a.Developer!.FirstName} {a.Developer!.LastName}"));
    //     }
    // }
}