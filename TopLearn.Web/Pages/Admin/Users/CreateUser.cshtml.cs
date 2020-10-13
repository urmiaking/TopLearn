using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopLearn.Core.DTOs;
using TopLearn.Core.Generators;
using TopLearn.Core.Security;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Entities.User;

namespace TopLearn.Web.Pages.Admin.Users
{
    public class CreateUserModel : PageModel
    {
        private readonly IPictureService _pictureService;
        private readonly IUserService _userService;
        private readonly IPermissionService _permissionService;

        public CreateUserModel(IPictureService pictureService, IUserService userService, IPermissionService permissionService)
        {
            _pictureService = pictureService;
            _userService = userService;
            _permissionService = permissionService;
        }

        [BindProperty]
        public CreateUserViewModel UserViewModel { get; set; }

        public void OnGet()
        { }

        public async Task<IActionResult> OnPostAsync(List<int> roleIds)
        {
            if (!ModelState.IsValid)
                return Page();

            if (!roleIds.Any())
            {
                TempData["Error"] = "لطفا حداقل یک نقش را انتخاب کنید";
                return Page();
            }

            if (await _userService.GetUserByEmailAsync(UserViewModel.Email) != null)
            {
                TempData["Error"] = "کاربر با ایمیل وارد شده موجود است";
                return Page();
            }

            var result = await _pictureService.SaveImageAsync(UserViewModel.AvatarFile, "avatars");

            if (result.LimitReached)
            {
                TempData["Error"] = "حجم عکس بیش از 500 کیلوبایت می باشد";
                return Page();
            }

            if (string.IsNullOrEmpty(result.ImageName))
            {
                TempData["Error"] = "مشکلی در ارسال عکس به وجود آمد";
                return Page();
            }

            var user = new User
            {
                Avatar = result.ImageName,
                Email = UserViewModel.Email,
                Password = PasswordHelper.Hash(UserViewModel.Password),
                RegisterDate = DateTime.Now,
                Name = UserViewModel.Name,
                IsActive = true,
                ActivationCode = Generator.GenerationUniqueName()
            };

            await _userService.AddUserAsync(user);

            foreach (var roleId in roleIds)
            {
                var userRole = new UserRole
                {
                    RoleId = roleId,
                    UserId = (await _userService.GetUserByEmailAsync(user.Email)).Id
                };
                await _permissionService.AddUserRoleAsync(userRole);
            }

            TempData["Success"] = "کاربر جدید با موفقیت افزوده شد";
            return Redirect("/Admin/Users");
        }
    }
}
