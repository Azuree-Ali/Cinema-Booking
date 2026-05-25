using Cinema.DataAccess;
using Cinema.Models;
using Cinema.Utilities;
using Cinema.Services;
using Cinema.Services;
using Microsoft.AspNetCore.Mvc;
using Ecommerce529.Services;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Areas.Admin.Controllers
{
    [Area(CD.ADMIN_AREA)]
    public class CinemaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly CinemaService _cinemaService;

        public CinemaController()
        {
            _context = new ApplicationDbContext();
            _cinemaService = new CinemaService();
        }
        public IActionResult Index(string cinemaName, int page = 1)
        {
            var cinemas = _context.Cinemas.AsQueryable();
            //filter 
            if (cinemaName != null)
            {
                cinemas = cinemas.Where(c => c.Name.Contains(cinemaName));
                ViewBag.cinemaName = cinemaName;
            }
            // pagination 
            int totalPages = (int)Math.Ceiling(cinemas.Count() / 5.0);
            cinemas = cinemas.Skip((page - 1) * 5).Take(5);
            return View(new CinemaVM()
            {
                Cinemas = cinemas.AsEnumerable(),
                TotalPages = totalPages,
                CurrentPage = page
            });

        }
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Models.Cinema());
        }
        [HttpPost]
        public IActionResult Create(CreateCinemaVM createCinemaVM)
        {
            if (!ModelState.IsValid)
            {
                return View(createCinemaVM);
            }
            var cinema = new Models.Cinema()
            {
                Name = createCinemaVM.Name,
                Description = createCinemaVM.Description,
                Address = createCinemaVM.Address,
                Status = createCinemaVM.Status
            };
            if (createCinemaVM.ImageFile != null && createCinemaVM.ImageFile.Length > 0)
            {
                var fileName = _cinemaService.SaveFile(createCinemaVM.ImageFile);
                cinema.Img = fileName;
            }
            _context.Cinemas.Add(cinema);
            _context.SaveChanges();
            TempData["Success"] = "Cinema created successfully";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var cinemas = _context.Cinemas.FirstOrDefault(c => c.Id == id);
            if (cinemas is null)
            {
                return NotFound();
            }
            return View(cinemas);
        }
        [HttpPost]
        public IActionResult Edit(Models.Cinema cinema , IFormFile ImageFile)
        {
            if (!ModelState.IsValid)
            {
                return View(cinema);
            }
            var cinemaInDb = _context.Cinemas.AsNoTracking().FirstOrDefault(b => b.Id == cinema.Id);
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = _cinemaService.SaveFile(ImageFile);
                cinema.Img = fileName;
                _cinemaService.RemoveFile(cinemaInDb.Img);
            }
            else
            {
                cinema.Img = cinemaInDb.Img;
            }

            _context.Cinemas.Update(cinema);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int id)
        {
            var cinema = _context.Cinemas.FirstOrDefault(c => c.Id == id);

            if (cinema is null)
            {
                return NotFound();
            }
            _cinemaService.RemoveFile(cinema.Img);
            _context.Cinemas.Remove(cinema);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));

        }
    }
}
