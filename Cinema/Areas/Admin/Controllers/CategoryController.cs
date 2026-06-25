using Cinema.DataAccess;
using Cinema.Models;
using Cinema.Repositories;
using Cinema.Services;
using Cinema.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Areas.Admin.Controllers
{
    [Area(CD.ADMIN_AREA)]
    [Authorize(Roles = $" {CD.SUPER_ADMIN_ROLE} , {CD.ADMIN_ROLE}  , {CD.EMPLOYEE_ROLE}")]
    public class CategoryController : Controller
    {
        //private readonly ApplicationDbContext _context;
        private readonly IRepository<Category> _categoryRepository; 

        public CategoryController(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<IActionResult> Index(string categoryName, int page = 1)
        {
            var categories = await _categoryRepository.GetAllAsync();
            //filter 
            if (categoryName != null)
            {
                categories = categories.Where(c => c.Name.Contains(categoryName));
                ViewBag.CategoryName = categoryName;
            }
            // pagination 
            int totalPages = (int)Math.Ceiling(categories.Count() / 5.0);
            categories = categories.Skip((page - 1) * 5).Take(5);
            return View(new CategoryVM()
            {
                Categories = categories.AsEnumerable(),
                TotalPages = totalPages,
                CurrentPage = page
            });
            
        }
        [HttpGet]
        [Authorize(Roles = $" {CD.SUPER_ADMIN_ROLE} , {CD.ADMIN_ROLE}")]

        public IActionResult Create()
        {
            return View(new Category());
        }
        [Authorize(Roles = $" {CD.SUPER_ADMIN_ROLE} , {CD.ADMIN_ROLE}")]

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }
            await _categoryRepository.CreateAsync(category);
            await _categoryRepository.CommitAsync();
            TempData["Success"] = "Category created successfully";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        [Authorize(Roles = $" {CD.SUPER_ADMIN_ROLE} , {CD.ADMIN_ROLE}")]

        public async Task<IActionResult> Edit(int id)
        {
            //var category = _context.Categories.FirstOrDefault(c=>c.Id == id);
            var category = await _categoryRepository.GetOneAsync(c => c.Id == id);
            if (category is null)
            {
                return NotFound();
            }
            return View(category);
        }
        [Authorize(Roles = $" {CD.SUPER_ADMIN_ROLE} , {CD.ADMIN_ROLE}")]

        [HttpPost]
        public async Task<IActionResult> Edit(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }
            _categoryRepository.Update(category);
            await _categoryRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = $" {CD.SUPER_ADMIN_ROLE} , {CD.ADMIN_ROLE}")]

        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryRepository.GetOneAsync(c => c.Id == id);

            if (category is null)
            {
                return NotFound();
            }
            _categoryRepository.Delete(category);
            await _categoryRepository.CommitAsync();
            return RedirectToAction(nameof(Index));

        }
    }
}
