using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using TopLearn.Core.Convertors;
using TopLearn.Core.DTOs;
using TopLearn.Core.Generators;
using TopLearn.Core.Security;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Entities.Mail;
using TopLearn.DataLayer.Entities.User;

namespace TopLearn.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IViewRenderService _viewRenderService;
        private readonly IMailService _mailService;

        public AccountController(IUserService userService, IViewRenderService viewRenderService, IMailService mailService)
        {
            _userService = userService;
            _viewRenderService = viewRenderService;
            _mailService = mailService;
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
                Avatar = "default-avatar.png"
            };

            await _userService.AddUserAsync(user);

            #region Send Account Activation Email

            var emailTemplateViewModel = new EmailTemplateViewModel()
            {
                Name = user.Name,
                Url = string.Concat(Request.Scheme, "://", Request.Host.ToUriComponent(), $"/Account/ActivateAccount/{user.ActivationCode}")
            };

            var body = await _viewRenderService.RenderToStringAsync("_AccountActivationTemplate", emailTemplateViewModel);

            var email = new Email()
            {
                To = user.Email,
                Subject = "فعال سازی حساب کاربری - تاپ لرن",
                Body = body
            };

            var emailSuccessfullySent = await _mailService.SendEmailAsync(email);

            if (!emailSuccessfullySent)
            {
                TempData["Error"] = "مشکلی پیش آمد، لطفا مجددا امتحان کنید";
                return View(registerForm);
            }

            #endregion

            return View("SuccessRegister", user);
        }

        #endregion

        #region Login

        [Route("/Login")]
        public IActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [Route("/Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginForm, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(loginForm);
            }

            var user = await _userService.LoginUserAsync(loginForm);

            if (user == null)
            {
                TempData["Error"] = "نام کاربری یا رمز عبور اشتباه است";
                return View(loginForm);
            }

            if (!user.IsActive)
            {
                TempData["Error"] = "حساب کاربری شما هنوز فعال نشده است";
                return View(loginForm);
            }

            /* ----- Creating Cookies ----- */
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.Name)
            };
            var claimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties()
            {
                IsPersistent = loginForm.RememberMe,
                ExpiresUtc = DateTimeOffset.Now.AddMinutes(43200)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimIdentity), authProperties);

            TempData["Success"] = $"{user.Name} عزیز خوش آمدید";

            var decodedUrl = "";
            if (!string.IsNullOrEmpty(returnUrl))
                decodedUrl = WebUtility.UrlDecode(returnUrl);

            if (Url.IsLocalUrl(decodedUrl))
            {
                return Redirect(decodedUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Logout

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Account Activation

        public async Task<IActionResult> ActivateAccount(string id) =>
            View(await _userService.ActivateAccountAsync(id));

        #endregion

        #region Remote Validation

        [HttpPost]
        [HttpGet]
        public async Task<IActionResult> IsEmailInUse(string email) =>
            await _userService.IsEmailExistAsync(email)
                ? Json($"ایمیل {email} قبلا استفاده شده است")
                : Json(true);

        #endregion
    }
}
