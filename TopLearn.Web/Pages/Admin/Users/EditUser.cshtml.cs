using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopLearn.Core.DTOs;
using TopLearn.Core.Services.Interfaces;

namespace TopLearn.Web.Pages.Admin.Users
{
    public class EditUserModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IPictureService _pictureService;

        public EditUserModel(IUserService userService, IPictureService pictureService)
        {
            _userService = userService;
            _pictureService = pictureService;
        }

        [BindProperty]
        public EditUserViewModel EditUserViewModel { get; set; }

        public async Task<IActionResult> OnGetAsync(int id = 0)
        {
            if (id == 0)
            {
                return NotFound();
            }
            
            var editUserViewModel = await _userService.GetEditUserViewModelByUserId(id);

            if (editUserViewModel is null)
            {
                return NotFound();
            }

            EditUserViewModel = await _userService.GetEditUserViewModelByUserId(id);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(List<int> roleIds)
        {
            if (!ModelState.IsValid)
                return Page();

            if (!roleIds.Any())
            {
                TempData["Error"] = "لطفا حداقل یک نقش را انتخاب کنید";
                return Page();
            }

            EditUserViewModel.Roles = roleIds;

            if (EditUserViewModel.ImageFile != null)
            {
                var result = await _pictureService.SaveImageAsync(EditUserViewModel.ImageFile, "avatars");

                if (result.LimitReached)
                {
                    TempData["Error"] = "حجم عکس جدید بیش از 500 کیلوبایت می باشد";
                    return Page();
                }

                if (string.IsNullOrEmpty(result.ImageName))
                {
                    TempData["Error"] = "مشکلی در ارسال عکس به وجود آمد";
                    return Page();
                }

                _pictureService.RemoveImage(EditUserViewModel.ImageName, "avatars");

                EditUserViewModel.ImageName = result.ImageName;
            }

            await _userService.EditUserAsync(EditUserViewModel);


            TempData["Success"] = "اطلاعات کاربر با موفقیت ویرایش شد";
            return RedirectToPage("/Admin/Users/Index");
        }
    }
}
