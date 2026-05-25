using Cinema.Models;

namespace Cinema.Services
{
    public class CategoryVM
    {
        public IEnumerable<Category> Categories { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
