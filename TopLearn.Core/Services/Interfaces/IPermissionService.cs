using System.Collections.Generic;
using System.Threading.Tasks;
using TopLearn.DataLayer.Entities.User;

namespace TopLearn.Core.Services.Interfaces
{
    public interface IPermissionService
    {
        #region Role

        Task<List<Role>> GetRolesAsync();

        Task<Role> GetRoleByIdAsync(int id);

        Task AddRoleAsync(Role role);

        Task EditRoleAsync(Role role);

        Task<bool> RemoveRoleAsync(int id);

        #endregion

        #region UserRole

        Task<List<UserRole>> GetUserRolesByUserIdAsync(int id);

        Task AddUserRoleAsync(UserRole userRole);

        Task RemoveUserRoleAsync(List<UserRole> userRoles);

        #endregion

    }
}