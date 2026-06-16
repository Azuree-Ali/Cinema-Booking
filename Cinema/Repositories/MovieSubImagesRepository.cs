using Cinema.DataAccess;
using Cinema.Models;

namespace Cinema.Repositories
{
    public class MovieSubImagesRepository : Repository<MovieSubImages>, IMovieSubImagesRepository
    {
        public MovieSubImagesRepository(ApplicationDbContext context) : base(context)
        {
        }
        public void DeleteRange(IEnumerable<MovieSubImages> movieSubImages)
        {
            _context.MovieSubImages.RemoveRange(movieSubImages);
        }
    }
}
