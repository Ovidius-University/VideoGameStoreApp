using System.ComponentModel.DataAnnotations.Schema;

namespace AppUI.Models.Entities
{
    [Table("Genre")]
    public class Genre
    {
        [Column("GenreId")]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<VideoGame>? VideoGames { get; set; }
    }
}
