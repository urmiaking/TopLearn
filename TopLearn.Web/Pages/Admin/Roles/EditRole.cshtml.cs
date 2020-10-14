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
    public class EditRoleModel : PageModel
    {
        private readonly IPermissionService _permissionService;

        public EditRoleModel(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        public Role Role { get; set; }


        public async Task<IActionResult> OnGetAsync(int id = 0)
        {
            if (id == 0)
                return NotFound();
            

            var role = await _permissionService.GetRoleByIdAsync(id);

            if (role is null)
                return NotFound();

            Role = role;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Role role)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _permissionService.EditRoleAsync(role);
            TempData["Success"] = "اطلاعات نقش با موفقیت بروز شد";
            return RedirectToPage("/Admin/Roles/Index");
        }
    }
}
