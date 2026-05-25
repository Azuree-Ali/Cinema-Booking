using Cinema.Models;

namespace Cinema.Services
{
    public class CinemaVM
    {
        public IEnumerable<Models.Cinema> Cinemas { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
