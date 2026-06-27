using Cinema.Models;
using Cinema.Utilities;
using Cinema.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Areas.Identity.Controllers
{
    [Area(CD.IDENTITY_AREA)]
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public ProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;

        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();
            var userVM = new ApplicationUserVM()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email
            };
            return View(userVM);
        }
        public async Task<IActionResult> UpdateUser(ApplicationUserVM applicationUserVM)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();
            user.FirstName = applicationUserVM.FirstName;
            user.LastName = applicationUserVM.LastName;
            user.Address = applicationUserVM.Address;
            user.PhoneNumber = applicationUserVM.PhoneNumber;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                TempData["Error_Notification"] = string.Join(", ", result.Errors.Select(e => e.Description));
                return View(nameof(Index), applicationUserVM);
            }
            TempData["Success_Notification"] = "user Updated Successfully";
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> UpdatePassword(ApplicationUserVM applicationUserVM)
        {
            if(!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index));
            }
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();
            var result =  await _userManager.ChangePasswordAsync(user, applicationUserVM.CurrentPassword, applicationUserVM.NewPassword);
            if (!result.Succeeded)
            {
                TempData["Error_Notification"] = string.Join(", ", result.Errors.Select(e => e.Description));
                return RedirectToAction(nameof(Index));
            }
            TempData["Success_Notification"] = "Password Updated Successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
