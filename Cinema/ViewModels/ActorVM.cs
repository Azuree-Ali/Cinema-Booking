using Cinema.Models;

namespace Cinema.Services
{
    public class ActorVM
    {
        public IEnumerable<Actor> Actors { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
