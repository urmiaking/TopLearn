using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TopLearn.Core.Convertors;
using TopLearn.Core.DTOs;
using TopLearn.Core.Generators;
using TopLearn.Core.Security;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Entities.User;

namespace TopLearn.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        #region Register

        [HttpGet]
        [Route("/Register")]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/Register")]
        public async Task<IActionResult> Register(RegisterViewModel registerForm)
        {
            if (!ModelState.IsValid)
            {
                return View(registerForm);
            }

            var user = new User
            {
                Email = OptimizeText.OptimizeEmail(registerForm.Email),
                IsActive = false,
                Name = registerForm.Name,
                RegisterDate = DateTime.Now,
                ActivationCode = Generator.GenerationUniqueName(),
                Password = PasswordHelper.Hash(registerForm.Password),
                Avatar = "default-avatar.png",
            };

            await _userService.AddUserAsync(user);

            return View("SuccessRegister", user);
        }

        #endregion

        [HttpPost]
        [HttpGet]
        public async Task<IActionResult> IsEmailInUse(string email) => 
            await _userService.IsEmailExistAsync(email)
                ? Json($"ایمیل {email} قبلا استفاده شده است")
                : Json(true);
    }
}
