using Cinema.DataAccess;
using Cinema.Models;
using Cinema.Utilities;
using Cinema.ViewModels;
using Ecommerce529.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Areas.Admin.Controllers
{
    [Area(CD.ADMIN_AREA)]
    public class MovieController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly MovieService _movieService;

        public MovieController()
        {
            _context = new ApplicationDbContext();
            _movieService = new MovieService();
        }
        public IActionResult Index(MovieVM movieVM, int page = 1)
        {
            var movies = _context.Movies
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .AsQueryable();

            // Search By Name
            if (!string.IsNullOrWhiteSpace(movieVM.MovieName))
            {
                movies = movies.Where(m =>
                    m.Name.Contains(movieVM.MovieName));
            }
            // Filter By Category
            if (movieVM.CategoryId.HasValue)
            {
                movies = movies.Where(m =>
                    m.CategoryId == movieVM.CategoryId);
            }
            // Filter By Cinema
            if (movieVM.CinemaId.HasValue)
            {
                movies = movies.Where(m =>
                    m.CinemaId == movieVM.CinemaId);
            }
            int pageSize = 5;
            int totalPages = (int)Math.Ceiling(
                movies.Count() / (double)pageSize);
            var result = movies
                .OrderByDescending(m => m.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            movieVM.Movies = result;
            movieVM.TotalPages = totalPages;
            movieVM.CurrentPage = page;
            return View(movieVM);
        }
        [HttpGet]
        public IActionResult Create()
        {
            var vm = new CreateMovieVM
            {
                Categories = _context.Categories.ToList(),
                Cinemas = _context.Cinemas.ToList()
            };
            return View(vm);
        }
        [HttpPost]
        public IActionResult Create(CreateMovieVM createMovieVM)
        {
            var movie = new Movie()
            {
                Name = createMovieVM.Name,
                Description = createMovieVM.Description,
                Price = createMovieVM.Price,
                Status = createMovieVM.Status,
                DateTime = createMovieVM.DateTime,
                Rating = createMovieVM.Rating,
                CategoryId = createMovieVM.CategoryId,
                CinemaId = createMovieVM.CinemaId,

            };
            // Main Image
            if (createMovieVM.MainImage != null)
            {
                movie.MainImg = _movieService.SaveFile(createMovieVM.MainImage);
            }
            _context.Movies.Add(movie);
            _context.SaveChanges();
            // Sub Images
            if (createMovieVM.SubImages != null)
            {
                foreach (var image in createMovieVM.SubImages)
                {
                    var fileName = _movieService.SaveFile(image , ProductImageType.SubImage);
                    _context.MovieSubImages.Add(new MovieSubImages()
                    {
                        Img = fileName,
                        MovieId = movie.Id
                    });
                }
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var movie = _context.Movies
                .Include(m => m.SubImages)
                .FirstOrDefault(m => m.Id == id);

            if (movie == null)
                return NotFound();

            var vm = new EditMovieVM()
            {
                Id = movie.Id,
                Name = movie.Name,
                Description = movie.Description,
                Price = movie.Price,
                Status = movie.Status,
                DateTime = movie.DateTime,
                Rating = movie.Rating,
                CategoryId = movie.CategoryId,
                CinemaId = movie.CinemaId,
                CurrentMainImage = movie.MainImg,
                ExistingSubImages = movie.SubImages,
                Categories = _context.Categories.ToList(),
                Cinemas = _context.Cinemas.ToList()
            };

            return View(vm);
        }
        [HttpPost]
        public IActionResult Edit(EditMovieVM vm)
        {
            //if (!ModelState.IsValid)
            //{
            //    vm.ExistingSubImages = _context.MovieSubImages
            //        .Where(x => x.MovieId == vm.Id)
            //        .ToList();

            //    return View(vm);
            //}

            var movie = new Movie
            {
                Id = vm.Id,
                Name = vm.Name,
                Description = vm.Description,
                Price = vm.Price,
                Status = vm.Status,
                DateTime = vm.DateTime,
                Rating = vm.Rating,
                CategoryId = vm.CategoryId,
                CinemaId = vm.CinemaId
            };

            var oldMovie = _context.Movies
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == vm.Id);

            if (oldMovie == null)
                return NotFound();

            // Main Image
            if (vm.MainImage != null)
            {
                movie.MainImg = _movieService.SaveFile(vm.MainImage);
                if (!string.IsNullOrEmpty(oldMovie.MainImg))
                    _movieService.RemoveFile(oldMovie.MainImg);
            }
            else
            {
                movie.MainImg = oldMovie.MainImg;
            }
            _context.Movies.Update(movie);
            _context.SaveChanges();

            // Sub Images
            if (vm.NewSubImages != null && vm.NewSubImages.Any())
            {
                foreach (var image in vm.NewSubImages)
                {
                    var fileName = _movieService.SaveFile(image , ProductImageType.SubImage);
                    _context.MovieSubImages.Add(new MovieSubImages
                    {
                        Img = fileName,
                        MovieId = movie.Id
                    });
                }
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int id)
        {
            var movie = _context.Movies
                .Include(m => m.SubImages)
                .FirstOrDefault(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }
            // Delete Main Image
            if (!string.IsNullOrEmpty(movie.MainImg))
            {
                _movieService.RemoveFile(movie.MainImg);
            }
            // Delete Sub Images Files
            foreach (var image in movie.SubImages)
            {
                _movieService.RemoveFile(image.Img);
            }
            // Delete Sub Images Records
            _context.MovieSubImages.RemoveRange(movie.SubImages);
            // Delete Movie
            _context.Movies.Remove(movie);
            _context.SaveChanges();
            TempData["Success"] = "Movie deleted successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
