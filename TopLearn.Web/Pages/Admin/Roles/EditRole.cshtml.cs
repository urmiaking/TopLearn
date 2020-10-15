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
    public class EditRoleModel : PageModel
    {
        private readonly IPermissionService _permissionService;

        public EditRoleModel(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [BindProperty]
        public Role Role { get; set; }

        public List<Permission> Permissions { get; set; }

        public List<int> SelectedPermissions { get; set; }

        public async Task<IActionResult> OnGetAsync(int id = 0)
        {
            if (id == 0)
                return NotFound();

            var role = await _permissionService.GetRoleByIdAsync(id);

            if (role is null)
                return NotFound();

            Role = role;
            Permissions = await _permissionService.GetPermissionsAsync();
            SelectedPermissions = await _permissionService.GetSelectedPermissionsByRoleIdAsync(id);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Role role, List<int> selectedPermissions)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _permissionService.EditRoleAsync(role);

            await _permissionService.RemovePermissionRolesByRoleIdAsync(role.Id);
            foreach (var permissionId in selectedPermissions)
            {
                var rolePermission = new RolePermission
                {
                    PermissionId = permissionId,
                    RoleId = role.Id
                };
                await _permissionService.AddPermissionRoleAsync(rolePermission);
            }


            TempData["Success"] = "اطلاعات نقش با موفقیت بروز شد";
            return RedirectToPage("/Admin/Roles/Index");
        }
    }
}
