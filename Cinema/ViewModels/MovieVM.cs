using Cinema.Models;

namespace Cinema.ViewModels
{
    public class MovieVM
    {
        public IEnumerable<Movie> Movies { get; set; } = new List<Movie>();
        public string? MovieName { get; set; }
        public int? CategoryId { get; set; }
        public int? CinemaId { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
