using Cinema.DataAccess;
using Cinema.Models;
using Cinema.Utilities;
using Cinema.Services;
using Cinema.Services;
using Microsoft.AspNetCore.Mvc;
using Ecommerce529.Services;
using Microsoft.EntityFrameworkCore;
using Cinema.Repositories;

namespace Cinema.Areas.Admin.Controllers
{
    [Area(CD.ADMIN_AREA)]
    public class CinemaController : Controller
    {
        //private readonly ApplicationDbContext _context;
        private readonly IRepository<Models.Cinema> _cinemaRepository;
        private readonly CinemaService _cinemaService;

        public CinemaController(IRepository<Models.Cinema> cinemaRepository)
        {
            _cinemaRepository = cinemaRepository;
            _cinemaService = new CinemaService();
        }
        public async Task<IActionResult> Index(string cinemaName, int page = 1)
        {
            var cinemas = await _cinemaRepository.GetAllAsync();
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
        public async Task<IActionResult> Create(CreateCinemaVM createCinemaVM)
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
            await _cinemaRepository.CreateAsync(cinema);
            await _cinemaRepository.CommitAsync();
            TempData["Success"] = "Cinema created successfully";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var cinemas = await _cinemaRepository.GetOneAsync(c => c.Id == id);
            if (cinemas is null)
            {
                return NotFound();
            }
            return View(cinemas);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Models.Cinema cinema, IFormFile ImageFile)
        {
            if (!ModelState.IsValid)
            {
                return View(cinema);
            } 
            var cinemaInDb = await _cinemaRepository.GetOneAsync(b => b.Id == cinema.Id);
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

            _cinemaRepository.Update(cinema);
            await _cinemaRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            var cinema = await _cinemaRepository.GetOneAsync(c => c.Id == id);

            if (cinema is null)
            {
                return NotFound();
            }
            _cinemaService.RemoveFile(cinema.Img);
            _cinemaRepository.Delete(cinema);
            await _cinemaRepository.CommitAsync();
            return RedirectToAction(nameof(Index));

        }
    }
}
