using Cinema.Models;

namespace Cinema.ViewModels
{
    public class CreateMovieVM
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public bool Status { get; set; }
        public DateTime DateTime { get; set; }
        public decimal Rating { get; set; }
        public int CategoryId { get; set; }
        public int CinemaId { get; set; }
        public List<int> ActorIds { get; set; } = new();
        public IFormFile MainImage { get; set; }
        public List<IFormFile>? SubImages { get; set; }
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
        public IEnumerable<Models.Cinema> Cinemas { get; set; } = new List<Models.Cinema>();
    }
}
