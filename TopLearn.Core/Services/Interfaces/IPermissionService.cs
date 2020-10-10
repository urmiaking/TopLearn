using System.Collections.Generic;
using System.Threading.Tasks;
using TopLearn.DataLayer.Entities.User;

namespace TopLearn.Core.Services.Interfaces
{
    public interface IPermissionService
    {
        Task<List<Role>> GetRolesAsync();

        Task AddUserRoleAsync(UserRole userRole);
    }
}