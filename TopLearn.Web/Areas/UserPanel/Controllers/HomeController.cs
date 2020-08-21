using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TopLearn.Core.DTOs;
using TopLearn.Core.Services.Interfaces;

namespace TopLearn.Web.Areas.UserPanel.Controllers
{
    [Authorize]
    [Area("UserPanel")]
    public class HomeController : Controller
    {
        private readonly IUserService _userService;

        public HomeController(IUserService userService)
        {
            _userService = userService;
        }
        public async Task<IActionResult> Index()
        {
            var userProfile = await _userService.GetUserProfileByEmailAsync(User.Identity.Name);

            if (userProfile == null)
            {
                return NotFound();
            }

            return View(userProfile);
        }

        [Route("[area]/[action]")]
        public async Task<IActionResult> EditProfile()
        {
            var userProfile = await _userService.GetEditProfileDataByEmailAsync(User.Identity?.Name);

            if (userProfile == null)
            {
                return NotFound();
            }
            

            return View(userProfile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("[area]/[action]")]
        public async Task<IActionResult> EditProfile(EditProfileViewModel profile)
        {
            if (!ModelState.IsValid)
            {
                return View(profile);
            }

            var userEditedSuccessfully = await _userService.EditUserProfileAsync(profile);

            if (!userEditedSuccessfully)
            {
                TempData["Error"] = "حجم عکس بیشتر از ۱ مگابایت می باشد";
                return View(profile);
            }

            TempData["Success"] = "حساب کاربری شما با موفقیت به روز شد";
            return RedirectToAction("Index");
        }
    }
}
