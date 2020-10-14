using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Entities.User;

namespace TopLearn.Web.Pages.Admin.Roles
{
    public class CreateRoleModel : PageModel
    {
        private readonly IPermissionService _permissionService;

        public CreateRoleModel(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [BindProperty]
        public Role Role { get; set; }

        public void OnGet()
        { }

        public async Task<IActionResult> OnPostAsync(Role role)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _permissionService.AddRoleAsync(role);

            TempData["Success"] = "نقش با موفقیت افزوده شد";
            return RedirectToPage("/Admin/Roles/Index");
        }
    }
}
