using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Entities.Permissions;
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

        public List<Permission> Permissions { get; set; }

        public async Task OnGetAsync()
        {
            Permissions = await _permissionService.GetPermissionsAsync();
        }

        public async Task<IActionResult> OnPostAsync(Role role, List<int> selectedPermissions)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (!selectedPermissions.Any())
            {
                TempData["Error"] = "حداقل یک سطح دسترسی باید انتخاب گردد";
                return RedirectToPage("CreateRole");
            }

            var roleId = await _permissionService.AddRoleAsync(role);

            foreach (var permissionId in selectedPermissions)
            {
                var rolePermission = new RolePermission
                {
                    PermissionId = permissionId,
                    RoleId = roleId
                };
                await _permissionService.AddPermissionRoleAsync(rolePermission);
            }

            TempData["Success"] = "نقش با موفقیت افزوده شد";
            return RedirectToPage("/Admin/Roles/Index");
        }
    }
}
