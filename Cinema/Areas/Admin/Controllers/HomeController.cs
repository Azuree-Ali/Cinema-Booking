using Cinema.Models;
using Cinema.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Cinema.Controllers
{
    [Area(CD.ADMIN_AREA)]
    [Authorize(Roles = $" {CD.SUPER_ADMIN_ROLE} , {CD.ADMIN_ROLE}  , {CD.EMPLOYEE_ROLE}")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
