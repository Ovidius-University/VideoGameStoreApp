namespace AppUI.Areas.Manager.Models.ViewModels
{
    public class IndexVideoGamesVm
    {
        public required string Publisher { get; set; }
        public List<VideoGameVm>? ListVideoGames { get; set; }
    }
}
