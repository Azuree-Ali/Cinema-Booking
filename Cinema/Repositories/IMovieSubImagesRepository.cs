using Cinema.Models;

namespace Cinema.Repositories
{
    public interface IMovieSubImagesRepository : IRepository<MovieSubImages>
    {
        public void DeleteRange(IEnumerable<MovieSubImages> movieSubImages);

    }
}
