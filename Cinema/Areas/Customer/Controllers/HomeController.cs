using Cinema.DataAccess;
using Cinema.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
    

namespace Cinema.Areas.Customer.Controllers
{
    [Area(CD.CUSTOMER_AREA)]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var movies = _context.Movies
                .AsNoTracking()
                .ToList();
            return View(movies);
        }
        public IActionResult Details(int id)
        {
            var movie = _context.Movies
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .Include(m => m.Actors)
                .Include(m => m.SubImages)
                .FirstOrDefault(m => m.Id == id);
            if (movie == null)
                return NotFound();
            return View(movie);
        }
    }
}