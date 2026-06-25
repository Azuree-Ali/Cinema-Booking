using Cinema.DataAccess;
using Cinema.Models;
using Cinema.Repositories;
using Cinema.Utilities;
using Cinema.ViewModels;
using Ecommerce529.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Cinema.Areas.Admin.Controllers
{
    [Area(CD.ADMIN_AREA)]
    [Authorize(Roles = $" {CD.SUPER_ADMIN_ROLE} , {CD.ADMIN_ROLE}  , {CD.EMPLOYEE_ROLE}")]
    public class MovieController : Controller
    {
        //private readonly ApplicationDbContext _context;
        private readonly IRepository<Movie> _movieRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Models.Cinema> _cinemaRepository;
        //private readonly IRepository<MovieSubImages> _movieSubImagesRepository;
        private readonly IMovieSubImagesRepository _movieSubImagesRepository;

        private readonly MovieService _movieService;

        public MovieController(IRepository<Movie> movieRepository, IRepository<Category> categoryRepository, IRepository<Models.Cinema> cinemaRepository, IMovieSubImagesRepository movieSubImagesRepository)
        { 
            _movieRepository = movieRepository;
            _categoryRepository = categoryRepository;
            _cinemaRepository = cinemaRepository;
            _movieSubImagesRepository = movieSubImagesRepository;
            _movieService = new MovieService();
        }
        public async Task<IActionResult> Index(MovieVM movieVM, int page = 1)
        {
            var movies = await _movieRepository.GetAllAsync(includes: [m => m.Category , m => m.Cinema]);
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
        [Authorize(Roles = $" {CD.SUPER_ADMIN_ROLE} , {CD.ADMIN_ROLE}")]

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new CreateMovieVM
            {
                Categories = await _categoryRepository.GetAllAsync(),
                Cinemas = await _cinemaRepository.GetAllAsync()
            };
            return View(vm);
        }
        [Authorize(Roles = $" {CD.SUPER_ADMIN_ROLE} , {CD.ADMIN_ROLE}")]

        [HttpPost]
        public async Task<IActionResult> Create(CreateMovieVM createMovieVM)
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
            await _movieRepository.CreateAsync(movie);
            await _movieRepository.CommitAsync();
            // Sub Images
            if (createMovieVM.SubImages != null)
            {
                foreach (var image in createMovieVM.SubImages)
                {
                    var fileName = _movieService.SaveFile(image , ProductImageType.SubImage);
                    await _movieSubImagesRepository.CreateAsync(new MovieSubImages()
                    {
                        Img = fileName,
                        MovieId = movie.Id
                    });
                }
                await _movieSubImagesRepository.CommitAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = $" {CD.SUPER_ADMIN_ROLE} , {CD.ADMIN_ROLE}")]

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var movie = await _movieRepository.GetOneAsync(filter: m => m.Id == id, includes: [m => m.SubImages]);
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
                Categories = await _categoryRepository.GetAllAsync(),
                Cinemas = await _cinemaRepository.GetAllAsync()
            };

            return View(vm);
        }
        [Authorize(Roles = $" {CD.SUPER_ADMIN_ROLE} , {CD.ADMIN_ROLE}")]

        [HttpPost]
        public async Task<IActionResult> Edit(EditMovieVM vm)
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

            var oldMovie = await _movieRepository.GetOneAsync(x => x.Id == vm.Id , IsTracking: true);
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
            _movieRepository.Update(movie);
            await _movieRepository.CommitAsync();

            // Sub Images
            if (vm.NewSubImages != null && vm.NewSubImages.Any())
            {
                foreach (var image in vm.NewSubImages)
                {
                    var fileName = _movieService.SaveFile(image , ProductImageType.SubImage);
                    await _movieSubImagesRepository.CreateAsync(new MovieSubImages
                    {
                        Img = fileName,
                        MovieId = movie.Id
                    });
                }
                await _movieSubImagesRepository.CommitAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = $" {CD.SUPER_ADMIN_ROLE} , {CD.ADMIN_ROLE}")]

        public async Task<IActionResult> Delete(int id)
        {
            var movie = await _movieRepository.GetOneAsync(m => m.Id == id, includes: [m => m.SubImages]);
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
                _movieService.RemoveFile(image.Img , ProductImageType.SubImage);
            }
            // Delete Sub Images Records
            _movieSubImagesRepository.DeleteRange(movie.SubImages);
            // Delete Movie
            _movieRepository.Delete(movie);
            await _movieRepository.CommitAsync();
            TempData["Success"] = "Movie deleted successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
