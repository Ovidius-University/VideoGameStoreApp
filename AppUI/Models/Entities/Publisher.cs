using System.ComponentModel.DataAnnotations.Schema;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AppUI.Models.Entities
{
    [Table ("Publisher")]
    public class Publisher
    {
        [Column("Publisher_Id")]
        public int Id { get; set; }
        public string Name { get; set; }=string.Empty;
        public ICollection<VideoGame>? VideoGames { get; set; }
        public ICollection<PublisherManager>? Managers { get; set; }
    }
}
