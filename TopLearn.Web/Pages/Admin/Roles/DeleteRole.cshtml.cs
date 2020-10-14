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
    public class DeleteRoleModel : PageModel
    {
        private readonly IPermissionService _permissionService;

        public DeleteRoleModel(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        public Role Role { get; set; }

        public async Task<IActionResult> OnGetAsync(int id = 0)
        {
            if (id is 0)
                return NotFound();

            var role = await _permissionService.GetRoleByIdAsync(id);

            if (role is null)
                return NotFound();

            Role = role;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Role role)
        {
            var isRemoved = await _permissionService.RemoveRoleAsync(role.Id);

            if (!isRemoved)
            {
                TempData["Error"] = "حذف این نقش امکان پذیر نمی باشد";
                return RedirectToPage("/Admin/Roles/DeleteRole", new {id = role.Id});
            }

            TempData["Success"] = "نقش مورد نظر با موفقیت حذف شد";
            return RedirectToPage("/Admin/Roles/Index");

        }
    }
}
