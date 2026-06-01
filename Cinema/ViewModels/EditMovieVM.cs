using Cinema.Models;

namespace Cinema.ViewModels
{
    public class EditMovieVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public bool Status { get; set; }
        public DateTime DateTime { get; set; }
        public decimal Rating { get; set; }
        public int CategoryId { get; set; }
        public int CinemaId { get; set; }
        public string CurrentMainImage { get; set; }
        public IFormFile? MainImage { get; set; }
        public List<IFormFile>? NewSubImages { get; set; }
        public List<MovieSubImages> ExistingSubImages { get; set; } = new();
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
        public IEnumerable<Models.Cinema> Cinemas { get; set; } = new List<Models.Cinema>();
    }
}
