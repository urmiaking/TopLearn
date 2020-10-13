using System.Collections.Generic;
using System.Threading.Tasks;
using TopLearn.DataLayer.Entities.User;

namespace TopLearn.Core.Services.Interfaces
{
    public interface IPermissionService
    {
        Task<List<Role>> GetRolesAsync();

        Task<Role> GetRoleByIdAsync(int id);

        Task<List<UserRole>> GetUserRolesByUserIdAsync(int id);

        Task AddUserRoleAsync(UserRole userRole);

        Task RemoveUserRoleAsync(List<UserRole> userRoles);
    }
}