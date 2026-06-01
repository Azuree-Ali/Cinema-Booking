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
    public class ActorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ActorService _actorService;

        public ActorController()
        {
            _context = new ApplicationDbContext();
            _actorService = new ActorService();
        }
        public IActionResult Index(string actorName, int page = 1)
        {
            var actors = _context.Actors.AsQueryable();
            //filter 
            if (actorName != null)
            {
                actors = actors.Where(c => c.Name.Contains(actorName));
                ViewBag.actorName = actorName;
            }
            // pagination 
            int totalPages = (int)Math.Ceiling(actors.Count() / 5.0);
            actors = actors.Skip((page - 1) * 5).Take(5);
            return View(new ActorVM()
            {
                Actors = actors.AsEnumerable(),
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
        public IActionResult Create(CreateActorVM createActorVM)
        {
            if (!ModelState.IsValid)
            {
                return View(createActorVM);
            }
            var actor = new Actor()
            {
                Name = createActorVM.Name,
                Gender = createActorVM.Gender
            };
            if (createActorVM.ImageFile != null && createActorVM.ImageFile.Length > 0)
            {
                var fileName = _actorService.SaveFile(createActorVM.ImageFile);
                actor.Img = fileName;
            }
            _context.Actors.Add(actor);
            _context.SaveChanges();
            TempData["Success"] = "Actor created successfully";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var actor = _context.Actors.FirstOrDefault(c => c.Id == id);
            if (actor is null)
            {
                return NotFound();
            }
            return View(actor);
        }
        [HttpPost]
        public IActionResult Edit(Actor actor , IFormFile ImageFile)
        {
         
            var actorInDb = _context.Actors.AsNoTracking().FirstOrDefault(b => b.Id == actor.Id);
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = _actorService.SaveFile(ImageFile);
                actor.Img = fileName;
                _actorService.RemoveFile(actorInDb.Img);
            }
            else
            {
                actor.Img = actorInDb.Img;
            }

            _context.Actors.Update(actor);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int id)
        {
            var actor = _context.Actors.FirstOrDefault(c => c.Id == id);

            if (actor is null)
            {
                return NotFound();
            }
            _actorService.RemoveFile(actor.Img);
            _context.Actors.Remove(actor);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));

        }
    }
}
