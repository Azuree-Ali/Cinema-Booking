using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema.Models
{
    public class MovieSubImages
    {
        public int Id { get; set; }
        public string Img { get; set; }
        public int MovieId { get; set; }
        [ForeignKey(nameof(MovieId))]
        public Movie Movie { get; set; } 
    }
}
